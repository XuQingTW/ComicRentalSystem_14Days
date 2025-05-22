using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces; // 技術點3: 引用介面命名空間
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ComicManagementForm : ComicRentalSystem_14Days.BaseForm // 技術點3: 繼承 BaseForm
    {
        private readonly ComicService _comicService;
        // Logger is inherited from BaseForm (protected ILogger? Logger)

        // 修改建構函式以接收 ILogger 並傳遞給 BaseForm
        public ComicManagementForm(ILogger logger) : base(logger) // 技術點4: 多型 (傳遞 ILogger 給 BaseForm)
        {
            InitializeComponent();
            LogActivity("ComicManagementForm initializing."); // 使用 BaseForm 的 LogActivity (內部會使用 Logger)

            var fileHelper = new FileHelper(); // 應該考慮是否也透過依賴注入傳入 FileHelper
            // 將 Logger 傳遞給 ComicService
            _comicService = new ComicService(fileHelper, Logger!); // 使用 BaseForm 的 Logger (確保非 null)

            // 訂閱 ComicService 的 ComicsChanged 事件 (技術點 #5 委派/事件)
            _comicService.ComicsChanged += ComicService_ComicsChanged;

            SetupDataGridView();
            LoadComicsData();
            LogActivity("ComicManagementForm initialized successfully.");
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            // 當 ComicService 中的資料變更時，這個方法會被呼叫
            LogActivity("ComicsChanged event received from ComicService. Refreshing DataGridView.");
            LoadComicsData();
            // LogActivity("漫畫資料已更新，列表已刷新。"); // 已在 LoadComicsData 內部記錄
        }

        private void SetupDataGridView()
        {
            LogActivity("Setting up DataGridView columns for comics.");
            dgvComics.AutoGenerateColumns = false; // 我們手動定義欄位
            dgvComics.Columns.Clear();

            // 技術點 #7: 清單控制項 (DataGridView)
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", Width = 150 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "類型", Width = 100 });
            dgvComics.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsRented", HeaderText = "已租借", Width = 70 }); // Day 9-10
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RentedToMemberId", HeaderText = "租借會員ID", Width = 100 }); // Day 9-10
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", Width = 120 });

            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.MultiSelect = false;
            dgvComics.ReadOnly = true; // 使用者不能直接編輯 DataGridView
            dgvComics.AllowUserToAddRows = false; // 不允許使用者直接在 DataGridView 中新增列

            LogActivity("DataGridView setup complete.");
        }

        private void LoadComicsData()
        {
            LogActivity("Attempting to load comics data into DataGridView.");
            try // 技術點 #5: 例外處理
            {
                List<Comic> comics = _comicService.GetAllComics(); // Service 層已有日誌
                dgvComics.DataSource = null; // 先清空，避免重複資料或更新問題
                dgvComics.DataSource = comics; // 將資料綁定到 DataGridView
                LogActivity($"Successfully loaded {comics.Count} comics into DataGridView.");
            }
            catch (Exception ex)
            {
                // 使用 BaseForm 的 LogErrorActivity
                LogErrorActivity("Error loading comics data into DataGridView.", ex);
                MessageBox.Show($"載入漫畫資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 技術點 #6: 視窗應用程式的事件處理
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LogActivity("Refresh button clicked. Will reload comics from file.");
            try
            {
                _comicService.Reload(); // 強制重新讀取 comics.csv
                                        // 由於 Reload() 內已觸發 ComicsChanged，DataGridView 會自動刷新
                                        // 若要立即重刷，也可補呼叫 LoadComicsData();
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error refreshing comics data from file.", ex);
                MessageBox.Show($"重新載入漫畫資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddComic_Click(object sender, EventArgs e)
        {
            LogActivity("Add Comic button clicked. Opening ComicEditForm for new comic.");
            // 將 Logger 傳遞給 ComicEditForm
            // 技術點7: 多表單視窗應用程式
            using (ComicEditForm editForm = new ComicEditForm(null, _comicService, Logger!)) // 傳遞 Logger
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    LogActivity("ComicEditForm (Add Mode) closed with OK. Data refresh handled by ComicsChanged event.");
                    // 資料刷新會由 ComicService 的 ComicsChanged 事件觸發，無需手動 LoadComicsData()
                }
                else
                {
                    LogActivity("ComicEditForm (Add Mode) closed with Cancel or other.");
                }
            }
        }

        private void btnEditComic_Click(object sender, EventArgs e)
        {
            LogActivity("Edit Comic button clicked.");
            if (dgvComics.SelectedRows.Count > 0)
            {
                Comic? selectedComic = dgvComics.SelectedRows[0].DataBoundItem as Comic;

                if (selectedComic != null)
                {
                    LogActivity($"Opening ComicEditForm for editing comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}'.");
                    // 技術點7: 多表單視窗應用程式
                    using (ComicEditForm editForm = new ComicEditForm(selectedComic, _comicService, Logger!)) // 傳遞 Logger
                    {
                        if (editForm.ShowDialog(this) == DialogResult.OK)
                        {
                            LogActivity($"ComicEditForm (Edit Mode) for comic ID: {selectedComic.Id} closed with OK. Data refresh handled by ComicsChanged event.");
                        }
                        else
                        {
                            LogActivity($"ComicEditForm (Edit Mode) for comic ID: {selectedComic.Id} closed with Cancel or other.");
                        }
                    }
                }
                else
                {
                    LogErrorActivity("Could not retrieve selected comic data from DataGridView for editing.");
                    MessageBox.Show("無法取得選定的漫畫資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogActivity("Edit Comic button clicked, but no comic was selected.");
                MessageBox.Show("請先選擇一本要編輯的漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteComic_Click(object sender, EventArgs e)
        {
            LogActivity("Delete Comic button clicked.");
            if (dgvComics.SelectedRows.Count > 0)
            {
                Comic? selectedComic = dgvComics.SelectedRows[0].DataBoundItem as Comic;
                if (selectedComic != null)
                {
                    LogActivity($"Attempting to delete comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}'. Showing confirmation dialog.");
                    var confirmResult = MessageBox.Show($"您確定要刪除漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 嗎？\n此操作無法復原。",
                                                 "確認刪除",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        LogActivity($"User confirmed deletion for comic ID: {selectedComic.Id}.");
                        try // 技術點 #5: 例外處理
                        {
                            _comicService.DeleteComic(selectedComic.Id); // Service 層已有日誌
                            // 資料刷新會由 ComicService 的 ComicsChanged 事件觸發
                            LogActivity($"Comic ID: {selectedComic.Id} successfully marked for deletion by service. UI will refresh via event.");
                            MessageBox.Show("漫畫已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException opEx) // 更具體的例外
                        {
                            LogErrorActivity($"Operation error deleting comic ID: {selectedComic.Id}.", opEx);
                            MessageBox.Show(opEx.Message, "操作錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex) // 一般例外
                        {
                            LogErrorActivity($"Generic error deleting comic ID: {selectedComic.Id}.", ex);
                            MessageBox.Show($"刪除漫畫時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        LogActivity($"User cancelled deletion for comic ID: {selectedComic.Id}.");
                    }
                }
                else
                {
                    LogErrorActivity("Could not retrieve selected comic data from DataGridView for deletion.");
                    MessageBox.Show("無法取得選定的漫畫資料以進行刪除。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogActivity("Delete Comic button clicked, but no comic was selected.");
                MessageBox.Show("請先選擇一本要刪除的漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 如果允許雙擊列來編輯
        private void dgvComics_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 確保不是點擊標頭列 (e.RowIndex == -1)
            if (e.RowIndex >= 0)
            {
                LogActivity($"DataGridView cell double-clicked at row {e.RowIndex}. Triggering edit action.");
                btnEditComic_Click(sender, e); // 觸發編輯按鈕的 Click 事件，或直接執行編輯邏輯
            }
        }

        // 在表單載入時執行
        private void ComicManagementForm_Load(object sender, EventArgs e)
        {
            // LoadComicsData() 已在建構函式中呼叫
            LogActivity("ComicManagementForm finished loading.");
        }
    }
}