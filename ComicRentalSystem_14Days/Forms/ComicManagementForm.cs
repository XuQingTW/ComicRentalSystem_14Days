﻿using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ComicManagementForm : ComicRentalSystem_14Days.BaseForm
    {
        private IComicService? _comicService;
        private readonly User? _currentUser;

        public ComicManagementForm(ILogger logger, IComicService comicService, User? currentUser) : base(logger)
        {
            InitializeComponent();
            _comicService = comicService;
            _currentUser = currentUser;
            SetupDataGridView();
            // LoadComicsData(); // Will be called by Form_Load
            if (_comicService != null) _comicService.ComicsChanged += ComicService_ComicsChanged;
        }

        private async void ComicManagementForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || Logger == null || _comicService == null)
            {
                return;
            }

            LogActivity("ComicManagementForm_Load: Form loading, attaching event handlers and loading initial data.");

            // Ensure event is not subscribed multiple times if Load is called again, though typically it's once.
            _comicService.ComicsChanged -= ComicService_ComicsChanged; // Defensive
            _comicService.ComicsChanged += ComicService_ComicsChanged;

            SetupDataGridView();

            await LoadComicsDataAsync();
            LogActivity("ComicManagementForm_Load: Form successfully initialized with async data load.");
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

        private async void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            if (_comicService == null) return;
            LogActivity("ComicService_ComicsChanged: Received ComicsChanged event. Reloading data asynchronously.");
            await LoadComicsDataAsync();
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

            var rentalDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.RentalDate),
                HeaderText = "租借日期",
                Width = 110
            };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvComics.Columns.Add(rentalDateColumn);

            var returnDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ReturnDate),
                HeaderText = "預計歸還時間",
                Width = 110
            };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvComics.Columns.Add(returnDateColumn);

            var actualReturnTimeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ActualReturnTime),
                HeaderText = "實際歸還時間",
                Width = 120
            };
            actualReturnTimeColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            actualReturnTimeColumn.DefaultCellStyle.NullValue = "";
            dgvComics.Columns.Add(actualReturnTimeColumn);

            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", Width = 120 });

            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.MultiSelect = false;
            dgvComics.ReadOnly = true;
            dgvComics.AllowUserToAddRows = false;
            LogActivity("DataGridView 設定完成。");
        }

        private async Task LoadComicsDataAsync()
        {
            if (_comicService == null) return;

            string searchTerm = this.txtSearchComics.Text.Trim();

            LogActivity($"LoadComicsDataAsync: Attempting to load comics data. Search term: '{searchTerm}'.");

            try
            {
                List<Comic> comics;
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    comics = await _comicService.GetAllComicsAsync();
                }
                else
                {
                    comics = await _comicService.SearchComicsAsync(searchTerm);
                }

                Action updateGrid = () => {
                    dgvComics.ClearSelection();
                    dgvComics.DataSource = null;
                    dgvComics.DataSource = comics;
                };

                if (dgvComics.IsHandleCreated && !dgvComics.IsDisposed)
                {
                    if (dgvComics.InvokeRequired)
                    {
                        dgvComics.Invoke(updateGrid);
                        if (dgvComics.Rows.Count > 0)
                        {
                            int firstVisibleColumnIndex = -1;
                            foreach (DataGridViewColumn col in dgvComics.Columns)
                            {
                                if (col.Visible && col.DisplayIndex >= 0)
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
                            else
                            {
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
                                if (col.Visible && col.DisplayIndex >= 0)
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
                            else
                            {

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

        private async void btnSearchComics_Click(object? sender, EventArgs e)
        {
            LogActivity("btnSearchComics_Click: Search button clicked. Reloading data asynchronously.");
            await LoadComicsDataAsync();
        }

        private async void btnClearSearchComics_Click(object? sender, EventArgs e)
        {
            LogActivity("btnClearSearchComics_Click: Clear search button clicked. Reloading data asynchronously.");
            this.txtSearchComics.Text = string.Empty;
            await LoadComicsDataAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_comicService == null) return;
            LogActivity("重新整理按鈕已點擊。將非同步從檔案重新載入漫畫。");
            try
            {
                await _comicService.ReloadAsync();
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

        private async void btnDeleteComic_Click(object sender, EventArgs e)
        {
            if (dgvComics.SelectedRows.Count > 0 && _comicService != null)
            {
                Comic? selectedComic = dgvComics.SelectedRows[0].DataBoundItem as Comic;
                if (selectedComic != null)
                {
                    // This check for IsRented is a quick UI check. The service layer (DeleteComicAsync)
                    // should have the authoritative check, especially in a multi-user or more complex scenario.
                    if (selectedComic.IsRented)
                    {
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 目前已租借中，無法刪除。\n請先處理歸還事宜。", "刪除錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogActivity($"btnDeleteComic_Click: Attempt to delete rented comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}' blocked by UI check.");
                        return;
                    }

                    LogActivity($"btnDeleteComic_Click: Attempting to delete comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}'. Showing confirmation dialog.");
                    var confirmResult = MessageBox.Show($"您確定要刪除漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 嗎？\n此操作無法復原。",
                                                 "確認刪除",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        LogActivity($"btnDeleteComic_Click: User confirmed deletion for comic ID: {selectedComic.Id}.");
                        try
                        {
                            await _comicService.DeleteComicAsync(selectedComic.Id);
                            LogActivity($"btnDeleteComic_Click: Comic ID: {selectedComic.Id} successfully deleted by service. UI should refresh via ComicsChanged event.");
                            // MessageBox.Show("漫畫已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                            // The above message might be premature if ComicsChanged event takes time or if other errors occur.
                            // It's often better to rely on the event to confirm UI state.
                            // However, for immediate feedback, it can be kept, or changed to "Delete command sent".
                        }
                        catch (InvalidOperationException opEx)
                        {
                            LogErrorActivity($"btnDeleteComic_Click: Operation error deleting comic ID: {selectedComic.Id}.", opEx);
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