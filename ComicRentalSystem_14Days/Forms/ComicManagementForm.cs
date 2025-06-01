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

            LogActivity("ComicManagementForm is loading runtime components.");

            _comicService.ComicsChanged += ComicService_ComicsChanged;

            // Event handlers for btnSearchComics and btnClearSearchComics
            // are now wired up by the designer in InitializeComponent.
            // The following Control.Find logic is no longer needed.

            SetupDataGridView(); // Already called in constructor, consider if needed here too.
                                 // If LoadComicsData in constructor is sufficient, this might be redundant.
                                 // For now, keeping it as per original structure.
            LoadComicsData(); // Already called in constructor
            LogActivity("ComicManagementForm initialized successfully.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity($"ComicManagementForm closing. User: {_currentUser?.Username ?? "N/A"}");
            if (_comicService != null)
            {
                _comicService.ComicsChanged -= ComicService_ComicsChanged;
                LogActivity("Unsubscribed from ComicService.ComicsChanged event.");
            }
            base.OnFormClosing(e);
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            if (_comicService == null) return;
            LogActivity("ComicsChanged event received. Refreshing DataGridView.");
            LoadComicsData();
        }

        private void SetupDataGridView()
        {
            LogActivity("Setting up DataGridView columns for comics.");
            dgvComics.AutoGenerateColumns = false;
            dgvComics.Columns.Clear();

            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", Width = 150 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "類型", Width = 100 });
            dgvComics.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsRented", HeaderText = "已租借", Width = 70 });
            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RentedToMemberId", HeaderText = "租借會員ID", Width = 100 });

            // Add RentalDate column
            var rentalDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.RentalDate), // "RentalDate"
                HeaderText = "租借日期",
                Width = 110 // Or another appropriate width
            };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvComics.Columns.Add(rentalDateColumn);

            // Add ReturnDate column
            var returnDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ReturnDate), // "ReturnDate"
                HeaderText = "預計歸還時間",
                Width = 110 // Or another appropriate width
            };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvComics.Columns.Add(returnDateColumn);

            // Add ActualReturnTime column
            var actualReturnTimeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ActualReturnTime),
                HeaderText = "實際歸還時間",
                Width = 120 // Or an appropriate width
            };
            actualReturnTimeColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; // Include time
            actualReturnTimeColumn.DefaultCellStyle.NullValue = ""; // Display empty if null
            dgvComics.Columns.Add(actualReturnTimeColumn);

            dgvComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", Width = 120 });

            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.MultiSelect = false;
            dgvComics.ReadOnly = true;
            dgvComics.AllowUserToAddRows = false;
            LogActivity("DataGridView setup complete.");
        }

        private void LoadComicsData()
        {
            if (_comicService == null) return;

            // txtSearchComics is now a member of the form, directly accessible.
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

                if (dgvComics.IsHandleCreated && !dgvComics.IsDisposed) // Check if control handle is created
                {
                    if (dgvComics.InvokeRequired)
                    {
                        dgvComics.Invoke(updateGrid);
                        if (dgvComics.Rows.Count > 0)
                        {
                            int firstVisibleColumnIndex = -1;
                            foreach (DataGridViewColumn col in dgvComics.Columns)
                            {
                                if (col.Visible && col.DisplayIndex >= 0) // Ensure column is visible and has a valid display index
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
                            else // Fallback if no visible columns, though unlikely for a populated grid
                            {
                                 // Optional: Log a warning if no visible column is found, though CurrentCell can't be set.
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
                                if (col.Visible && col.DisplayIndex >= 0) // Ensure column is visible and has a valid display index
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
                            else // Fallback if no visible columns, though unlikely for a populated grid
                            {
                                 // Optional: Log a warning if no visible column is found, though CurrentCell can't be set.
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
            LogActivity("Search Comics button clicked.");
            LoadComicsData(); // LoadComicsData will now use the search term
        }

        private void btnClearSearchComics_Click(object? sender, EventArgs e)
        {
            LogActivity("Clear Search Comics button clicked.");
            // txtSearchComics is now a member of the form, directly accessible.
            this.txtSearchComics.Text = string.Empty;
            LoadComicsData(); // Reload all comics
        }

        private async void btnRefresh_Click(object sender, EventArgs e) // Made async void
        {
            if (_comicService == null) return;
            LogActivity("Refresh button clicked. Will reload comics from file asynchronously.");
            try
            {
                await _comicService.ReloadAsync(); // Call async version
                // UI update will be handled by the ComicsChanged event calling LoadComicsData
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
            LogActivity("Add Comic button clicked. Opening ComicEditForm for new comic.");
            using (ComicEditForm editForm = new ComicEditForm(null, _comicService, Logger, _currentUser))
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    LogActivity("ComicEditForm (Add Mode) closed with OK. Data refresh handled by ComicsChanged event.");
                }
                else
                {
                    LogActivity("ComicEditForm (Add Mode) closed with Cancel or other.");
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
                LogActivity("Edit Comic button clicked, but no comic was selected or service not ready.");
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
                    // Check if the comic is currently rented
                    if (selectedComic.IsRented)
                    {
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 目前已租借中，無法刪除。\n請先處理歸還事宜。", "刪除錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogActivity($"Attempt to delete comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}' failed: Comic is currently rented.");
                        return; // Abort deletion
                    }

                    // If not rented, proceed with the existing confirmation dialog:
                    LogActivity($"Attempting to delete comic ID: {selectedComic.Id}, Title: '{selectedComic.Title}'. Showing confirmation dialog.");
                    var confirmResult = MessageBox.Show($"您確定要刪除漫畫 '{selectedComic.Title}' (ID: {selectedComic.Id}) 嗎？\n此操作無法復原。",
                                                 "確認刪除",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        LogActivity($"User confirmed deletion for comic ID: {selectedComic.Id}.");
                        try
                        {
                            _comicService.DeleteComic(selectedComic.Id);
                            LogActivity($"Comic ID: {selectedComic.Id} successfully marked for deletion by service. UI will refresh via event.");
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
                        LogActivity($"User cancelled deletion for comic ID: {selectedComic.Id}.");
                    }
                }
            }
            else
            {
                LogActivity("Delete Comic button clicked, but no comic was selected or service not ready.");
                MessageBox.Show("請先選擇一本要刪除的漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvComics_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LogActivity($"DataGridView cell double-clicked at row {e.RowIndex}. Triggering edit action.");
                btnEditComic_Click(sender, e);
            }
        }
    }
}