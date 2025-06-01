using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ComicManagementForm : ComicRentalSystem_14Days.BaseForm
    {
        private ComicService? _comicService;
        private readonly User? _currentUser;

        public ComicManagementForm(ILogger logger, ComicService comicService, User? currentUser) : base(logger)
        {
            InitializeComponent();
            _comicService = comicService;
            _currentUser = currentUser;
            SetupDataGridView();
            LoadComicsData();
            _comicService.ComicsChanged += ComicService_ComicsChanged;
        }

        private void ComicManagementForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || Logger == null || _comicService == null)
            {
                return;
            }

            LogActivity("漫畫管理表單正在載入執行階段元件。");

            _comicService.ComicsChanged += ComicService_ComicsChanged;

            // btnSearchComics 和 btnClearSearchComics 的事件處理常式
            // 現在由設計工具在 InitializeComponent 中連接。
            //不再需要以下的 Control.Find 邏輯。

            SetupDataGridView(); // 已在建構函式中呼叫，請考慮此處是否也需要。
                                 // 如果建構函式中的 LoadComicsData 已足夠，這可能是多餘的。
                                 // 目前，依照原始結構保留。
            LoadComicsData(); // 已在建構函式中呼叫
            LogActivity("漫畫管理表單已成功初始化。");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity($"ComicManagementForm closing. User: {_currentUser?.Username ?? "N/A"}");
            if (_comicService != null)
            {
                _comicService.ComicsChanged -= ComicService_ComicsChanged;
                LogActivity("已取消訂閱 ComicService.ComicsChanged 事件。");
            }
            base.OnFormClosing(e);
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            if (_comicService == null) return;
            LogActivity("已收到 ComicsChanged 事件。正在重新整理 DataGridView。");
            LoadComicsData();
        }

        private void SetupDataGridView()
        {
            LogActivity("正在設定漫畫的 DataGridView 資料行。");
            dgvComics.AutoGenerateColumns = false;
            dgvComics.Columns.Clear();

            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", Width = 150 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "類型", Width = 100 });
            dgvComics.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsRented", HeaderText = "已租借", Width = 70 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RentedToMemberId", HeaderText = "租借會員ID", Width = 100 });

            // 新增 RentalDate 資料行
            var rentalDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.RentalDate), // "RentalDate"
                HeaderText = "租借日期",
                Width = 110 // 或其他適當的寬度
            };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvComics.Columns.Add(rentalDateColumn);

            // 新增 ReturnDate 資料行
            var returnDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ReturnDate), // "ReturnDate"
                HeaderText = "預計歸還時間",
                Width = 110 // 或其他適當的寬度
            };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvComics.Columns.Add(returnDateColumn);

            // 新增 ActualReturnTime 資料行
            var actualReturnTimeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ActualReturnTime),
                HeaderText = "實際歸還時間",
                Width = 120 // 或適當的寬度
            };
            actualReturnTimeColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; // 包含時間
            actualReturnTimeColumn.DefaultCellStyle.NullValue = ""; // 若為空則顯示空白
            dgvComics.Columns.Add(actualReturnTimeColumn);

            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", Width = 120 });

            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.MultiSelect = false;
            dgvComics.ReadOnly = true;
            dgvComics.AllowUserToAddRows = false;
            LogActivity("DataGridView 設定完成。");
        }

        private void LoadComicsData()
        {
            if (_comicService == null) return;

            // txtSearchComics 現在是表單的成員，可直接存取。
            string searchTerm = this.txtSearchComics.Text.Trim();

            LogActivity($"Attempting to load comics data. Search term: '{searchTerm}'.");

            try
            {
                List<Comic> comics;
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    comics = _comicService.GetAllComics();
                }
                else
                {
                    comics = _comicService.SearchComics(searchTerm);
                }

                Action updateGrid = () => {
                    dgvComics.ClearSelection();
                    dgvComics.DataSource = null;
                    dgvComics.DataSource = comics;
                };

                if (dgvComics.IsHandleCreated && !dgvComics.IsDisposed) // 檢查控制代碼是否已建立
                {
                    if (dgvComics.InvokeRequired)
                    {
                        dgvComics.Invoke(updateGrid);
                        if (dgvComics.Rows.Count > 0)
                        {
                            int firstVisibleColumnIndex = -1;
                            foreach (DataGridViewColumn col in dgvComics.Columns)
                            {
                                if (col.Visible && col.DisplayIndex >= 0) // 確保資料行可見且具有有效的顯示索引
                                {
                                    if (firstVisibleColumnIndex == -1 || col.DisplayIndex < dgvComics.Columns[firstVisibleColumnIndex].DisplayIndex)
                                    {
                                        firstVisibleColumnIndex = col.Index;
                                    }
                                }
                            }
                            if (firstVisibleColumnIndex != -1)
                            {
                                dgvComics.CurrentCell = dgvComics.Rows[0].Cells[firstVisibleColumnIndex];
                            }
                            else // 如果沒有可見的資料行則備用（儘管對於已填入資料的格線來說不太可能）
                            {
                                 // 可選：如果找不到可見的資料行，則記錄警告（儘管無法設定 CurrentCell）。
                                 // Logger?.LogWarning("LoadComicsData: No visible columns found in dgvComics to set CurrentCell.");
                            }
                        }
                    }
                    else
                    {
                        updateGrid();
                        if (dgvComics.Rows.Count > 0)
                        {
                            int firstVisibleColumnIndex = -1;
                            foreach (DataGridViewColumn col in dgvComics.Columns)
                            {
                                if (col.Visible && col.DisplayIndex >= 0) // 確保資料行可見且具有有效的顯示索引
                                {
                                    if (firstVisibleColumnIndex == -1 || col.DisplayIndex < dgvComics.Columns[firstVisibleColumnIndex].DisplayIndex)
                                    {
                                        firstVisibleColumnIndex = col.Index;
                                    }
                                }
                            }
    if (firstVisibleColumnIndex != -1 && dgvComics.Rows.Count > 0 && dgvComics.Rows[0].Cells.Count > firstVisibleColumnIndex)
                            {
        dgvComics.CurrentCell = dgvComics.Rows[0].Cells[firstVisibleColumnIndex];
                            }
                            else // 如果沒有可見的資料行則備用（儘管對於已填入資料的格線來說不太可能）
                            {
                                 // 可選：如果找不到可見的資料行，則記錄警告（儘管無法設定 CurrentCell）。
                                 // Logger?.LogWarning("LoadComicsData: No visible columns found in dgvComics to set CurrentCell.");
                            }
                        }
                    }
                }
                LogActivity($"Successfully loaded {comics.Count} comics into DataGridView with search term '{searchTerm}'.");
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error loading comics data with search term '{searchTerm}'.", ex);
                 Action showError = () => MessageBox.Show($"載入漫畫資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    if (this.InvokeRequired) { this.Invoke(showError); } else { showError(); }
                }
            }
        }

        private void btnSearchComics_Click(object? sender, EventArgs e)
        {
            LogActivity("搜尋漫畫按鈕已點擊。");
            LoadComicsData(); // LoadComicsData 現在將使用搜尋詞彙
        }

        private void btnClearSearchComics_Click(object? sender, EventArgs e)
        {
            LogActivity("清除搜尋漫畫按鈕已點擊。");
            // txtSearchComics 現在是表單的成員，可直接存取。
            this.txtSearchComics.Text = string.Empty;
            LoadComicsData(); // 重新載入所有漫畫
        }

        private async void btnRefresh_Click(object sender, EventArgs e) // 設為 async void
        {
            if (_comicService == null) return;
            LogActivity("重新整理按鈕已點擊。將非同步從檔案重新載入漫畫。");
            try
            {
                await _comicService.ReloadAsync(); // 呼叫非同步版本
                // UI 更新將由呼叫 LoadComicsData 的 ComicsChanged 事件處理
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error refreshing comics data from file.", ex);
                MessageBox.Show($"重新載入漫畫資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddComic_Click(object sender, EventArgs e)
        {
            if (_comicService == null || Logger == null) return;
            LogActivity("新增漫畫按鈕已點擊。正在為新漫畫開啟 ComicEditForm。");
            using (ComicEditForm editForm = new ComicEditForm(null, _comicService, Logger, _currentUser))
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    LogActivity("ComicEditForm (新增模式) 已關閉並回傳 OK。資料重新整理將由 ComicsChanged 事件處理。");
                }
                else
                {
                    LogActivity("ComicEditForm (新增模式) 已關閉並回傳 Cancel 或其他。");
                }
            }
        }

        private void btnEditComic_Click(object sender, EventArgs e)
        {
            if (dgvComics.SelectedRows.Count > 0 && _comicService != null && Logger != null)
            {
                Comic? selectedComic = dgvComics.SelectedRows[0].DataBoundItem as Comic;

                if (selectedComic != null)
                {
                    LogActivity($"Opening ComicEditForm for editing comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}'.");
                    using (ComicEditForm editForm = new ComicEditForm(selectedComic, _comicService, Logger, _currentUser))
                    {
                        if (editForm.ShowDialog(this) == DialogResult.OK)
                        {
                            LogActivity($"漫畫 ID: {selectedComic.Id} 的 ComicEditForm (編輯模式) 已關閉並回傳 OK。資料重新整理將由 ComicsChanged 事件處理。");
                        }
                        else
                        {
                            LogActivity($"漫畫 ID: {selectedComic.Id} 的 ComicEditForm (編輯模式) 已關閉並回傳 Cancel 或其他。");
                        }
                    }
                }
                else
                {
                    LogErrorActivity("無法從 DataGridView 擷取選定的漫畫資料進行編輯。");
                    MessageBox.Show("無法取得選定的漫畫資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogActivity("編輯漫畫按鈕已點擊，但未選取任何漫畫或服務未就緒。");
                MessageBox.Show("請先選擇一本要編輯的漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteComic_Click(object sender, EventArgs e)
        {
            if (dgvComics.SelectedRows.Count > 0 && _comicService != null)
            {
                Comic? selectedComic = dgvComics.SelectedRows[0].DataBoundItem as Comic;
                if (selectedComic != null)
                {
                    // 檢查漫畫目前是否已租借
                    if (selectedComic.IsRented)
                    {
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 目前已租借中，無法刪除。\n請先處理歸還事宜。", "刪除錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogActivity($"嘗試刪除漫畫 ID: {selectedComic.Id}，書名: '{selectedComic.Title}' 失敗：漫畫目前已租借。");
                        return; // 中止刪除
                    }

                    // 若未租借，則繼續進行現有的確認對話方塊：
                    LogActivity($"Attempting to delete comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}'. Showing confirmation dialog.");
                    var confirmResult = MessageBox.Show($"您確定要刪除漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 嗎？\n此操作無法復原。",
                                                 "確認刪除",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        LogActivity($"使用者已確認刪除漫畫 ID: {selectedComic.Id}。");
                        try
                        {
                            _comicService.DeleteComic(selectedComic.Id);
                            LogActivity($"漫畫 ID: {selectedComic.Id} 已由服務成功標記為待刪除。UI 將透過事件重新整理。");
                            MessageBox.Show("漫畫已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException opEx)
                        {
                            LogErrorActivity($"Operation error deleting comic ID: {selectedComic.Id}.", opEx);
                            MessageBox.Show(opEx.Message, "操作錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            LogErrorActivity($"Generic error deleting comic ID: {selectedComic.Id}.", ex);
                            MessageBox.Show($"刪除漫畫時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        LogActivity($"使用者已取消刪除漫畫 ID: {selectedComic.Id}。");
                    }
                }
            }
            else
            {
                LogActivity("刪除漫畫按鈕已點擊，但未選取任何漫畫或服務未就緒。");
                MessageBox.Show("請先選擇一本要刪除的漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvComics_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LogActivity($"DataGridView 儲存格在資料列 {e.RowIndex} 被雙擊。觸發編輯動作。");
                btnEditComic_Click(sender, e);
            }
        }
    }
}