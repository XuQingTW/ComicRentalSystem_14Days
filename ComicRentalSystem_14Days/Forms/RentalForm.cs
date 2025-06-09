using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RentalForm : BaseForm
    {
        private readonly IComicService? _comicService;
        private readonly MemberService? _memberService;
        private readonly IReloadService? _reloadService;

        public RentalForm() : base()
        {
            InitializeComponent();
        }

        public RentalForm(
            IComicService comicService,
            MemberService memberService,
            ILogger logger,
            IReloadService reloadService
        ) : base(logger)
        {
            InitializeComponent();

            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));

            if (_comicService != null)
            {
                _comicService.ComicsChanged += Service_DataChanged;
            }
            if (_memberService != null)
            {
                _memberService.MembersChanged += Service_DataChanged;
            }

            LogActivity("租借表單初始化中 (使用服務)。");
        }

        private async void RentalForm_Load(object sender, EventArgs e)
        {
            LogActivity("RentalForm_Load: Form loading data asynchronously.");
            if (_comicService != null && _memberService != null)
            {
                SetupRentedComicsDataGridView();
                await LoadMembersAsync();
                await LoadAvailableComicsAsync();
                await LoadRentalDetailsAsync();

                // _reloadService interaction remains the same as it already uses an async lambda
                _reloadService?.Start(
                    async () =>
                    {
                        LogActivity("RentalForm_Load: Auto-reload starting (async).");
                        // Assuming ReloadAsync in services are designed to be safe if called concurrently or frequently.
                        // They should ideally trigger their respective XxxChanged events upon completion.
                        if (_comicService != null) await _comicService.ReloadAsync();
                        if (_memberService != null) await _memberService.ReloadAsync();
                        LogActivity("RentalForm_Load: Auto-reload completed (async).");
                    },
                    TimeSpan.FromSeconds(30) // Consider making this configurable
                );

                // This check might be redundant if LoadMembersAsync populates cmbMembers correctly.
                // If cmbMembers is still empty, it might indicate an issue with initial data load or service availability.
                if (cmbMembers.Items.Count == 0)
                {
                    LogActivity("RentalForm_Load: cmbMembers is empty after initial load, attempting one more refresh of comics and rental details.");
                    await LoadAvailableComicsAsync(); // Re-load available comics if members were initially empty
                    await LoadRentalDetailsAsync();
                }
                LogActivity("RentalForm_Load: Form successfully loaded data asynchronously.");
            }
            else
            {
                LogActivity("RentalForm_Load: Loaded in Design Mode or services not available. Skipping data load.");
            }
        }

        private async void Service_DataChanged(object? sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null) return;

            LogActivity($"Service_DataChanged: Received data changed event from {(sender is IComicService ? "ComicService" : "MemberService")}. Updating UI asynchronously.");
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                // While await often marshals back to UI context, explicit Invoke can be safer
                // for event handlers that might be triggered rapidly or from unexpected threads.
                // However, for simplicity and typical WinForms async/await behavior,
                // direct await is often sufficient. Let's try direct await first.
                // If InvokeRequired is true, it means we are on a non-UI thread.
                if (this.InvokeRequired)
                {
                    // Consider if Invoke is truly needed if RefreshUIDataSafely is fully async
                    // and handles its own UI marshalling if necessary (though usually not needed with await).
                    // For now, keeping Invoke for safety if called from a truly background thread.
                    this.Invoke(async () => await RefreshUIDataSafelyAsync());
                }
                else
                {
                    await RefreshUIDataSafelyAsync();
                }
            }
        }

        private async Task RefreshUIDataSafelyAsync()
        {
            if (_comicService == null || _memberService == null) return;
            LogActivity("RefreshUIDataSafelyAsync: Refreshing UI data.");

            object? selectedMemberVal = cmbMembers.SelectedValue;
            object? selectedComicVal = cmbComics.SelectedValue;

            await LoadMembersAsync();
            // Check if handle is still valid after await
            if (!cmbMembers.IsHandleCreated || cmbMembers.IsDisposed) return;
            if (selectedMemberVal != null && cmbMembers.Items.Cast<Member>().Any(m => m.Id == (int)selectedMemberVal))
            {
                cmbMembers.SelectedValue = selectedMemberVal;
            }

            await LoadAvailableComicsAsync();
            if (!cmbComics.IsHandleCreated || cmbComics.IsDisposed) return;
            if (selectedComicVal != null && cmbComics.Items.Cast<Comic>().Any(c => c.Id == (int)selectedComicVal))
            {
                cmbComics.SelectedValue = selectedComicVal;
            }

            await LoadRentalDetailsAsync();
            LogActivity("RefreshUIDataSafelyAsync: UI data refresh complete.");
        }

        private async Task LoadMembersAsync()
        {
            if (_memberService == null) return;

            LogActivity("LoadMembersAsync: Loading members into cmbMembers.");
            try
            {
                var members = await _memberService.GetAllMembersAsync();
                if (!cmbMembers.IsHandleCreated || cmbMembers.IsDisposed) return; // Check after await

                cmbMembers.DataSource = null;
                cmbMembers.DataSource = members;
                cmbMembers.DisplayMember = nameof(Member.Name);
                cmbMembers.ValueMember = nameof(Member.Id);

                if (members.Any())
                {
                    if (cmbMembers.SelectedIndex == -1) cmbMembers.SelectedIndex = 0;
                }
                else
                {
                    cmbMembers.SelectedIndex = -1;
                }
                LogActivity($"LoadMembersAsync: Loaded {members.Count} members into cmbMembers.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("LoadMembersAsync: Error loading members into cmbMembers.", ex);
                if (this.IsHandleCreated && !this.IsDisposed)
                    MessageBox.Show("載入會員列表時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAvailableComicsAsync()
        {
            if (_comicService == null) return;

            LogActivity("LoadAvailableComicsAsync: Loading available (not rented) comics into cmbComics.");
            try
            {
                var allComics = await _comicService.GetAllComicsAsync();
                if (!cmbComics.IsHandleCreated || cmbComics.IsDisposed) return; // Check after await

                var availableComics = allComics.Where(c => !c.IsRented).ToList();

                cmbComics.DataSource = null;
                cmbComics.DataSource = availableComics;
                cmbComics.DisplayMember = nameof(Comic.Title);
                cmbComics.ValueMember = nameof(Comic.Id);

                if (!availableComics.Any())
                {
                    cmbComics.SelectedIndex = -1;
                    cmbComics.Text = " (無可用漫畫) "; // Keep this for user feedback
                }
                else
                {
                    if (cmbComics.SelectedIndex == -1) cmbComics.SelectedIndex = 0;
                }
                LogActivity($"LoadAvailableComicsAsync: Loaded {availableComics.Count} available comics into cmbComics.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("LoadAvailableComicsAsync: Error loading available comics into cmbComics.", ex);
                if (this.IsHandleCreated && !this.IsDisposed)
                    MessageBox.Show("載入可用漫畫列表時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadRentalDetailsAsync()
        {
            if (_comicService == null || _memberService == null)
            {
                if (dgvRentedComics != null && dgvRentedComics.IsHandleCreated && !dgvRentedComics.IsDisposed)
                    dgvRentedComics.DataSource = null;
                return;
            }

            LogActivity("LoadRentalDetailsAsync: Loading rental details for admin view.");
            try
            {
                var allComics = await _comicService.GetAllComicsAsync();
                // Check handle validity after first await
                if (this.IsDisposed || !this.IsHandleCreated) return;

                var allMembers = await _memberService.GetAllMembersAsync();
                // Check handle validity after second await
                if (this.IsDisposed || !this.IsHandleCreated) return;

                var rentedComicsQuery = allComics.Where(c => c.IsRented && c.RentedToMemberId != 0);

                Member? selectedMember = null;
                if (cmbMembers.IsHandleCreated && !cmbMembers.IsDisposed && cmbMembers.SelectedItem != null)
                {
                    selectedMember = cmbMembers.SelectedItem as Member;
                }

                if (selectedMember != null)
                {
                    LogActivity($"LoadRentalDetailsAsync: Filtering rental details for selected member: {selectedMember.Name} (ID: {selectedMember.Id})");
                    rentedComicsQuery = rentedComicsQuery.Where(c => c.RentedToMemberId == selectedMember.Id);
                }
                else
                {
                    LogActivity("LoadRentalDetailsAsync: No specific member selected. Loading all rented comics.");
                }

                var rentalDetails = rentedComicsQuery
                    .Select(comic =>
                    {
                        var member = allMembers.FirstOrDefault(m => m.Id == comic.RentedToMemberId);
                        return new RentalDetailViewModel
                        {
                            MemberId = member?.Id ?? 0,
                            MemberName = member?.Name ?? "未知會員",
                            MemberPhoneNumber = member?.PhoneNumber ?? "無",
                            ComicId = comic.Id,
                            ComicTitle = comic.Title,
                            RentalDate = comic.RentalDate,
                            ExpectedReturnDate = comic.ReturnDate,
                            ActualReturnTime = comic.ActualReturnTime
                        };
                    })
                    .ToList();

                if (dgvRentedComics != null && dgvRentedComics.IsHandleCreated && !dgvRentedComics.IsDisposed)
                {
                    dgvRentedComics.DataSource = null; // Clear previous data
                    dgvRentedComics.DataSource = rentalDetails;
                }
                LogActivity($"LoadRentalDetailsAsync: Loaded {rentalDetails.Count} rental details.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("LoadRentalDetailsAsync: Error loading rental details.", ex);
                if (this.IsHandleCreated && !this.IsDisposed)
                    MessageBox.Show("載入租借詳細資料時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupRentedComicsDataGridView()
        {
            LogActivity("正在為管理員視圖設定 dgvRentedComics。");
            dgvRentedComics.AutoGenerateColumns = false;
            dgvRentedComics.Columns.Clear();
            dgvRentedComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.MemberName), HeaderText = "會員姓名", FillWeight = 18 }); // Adjusted
            dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.MemberId), HeaderText = "會員ID", FillWeight = 9 }); // 已調整
            dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.MemberPhoneNumber), HeaderText = "會員電話", FillWeight = 18 }); // 已調整
            dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.ComicTitle), HeaderText = "租借書名", FillWeight = 20 }); // 已調整

            var rentalDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RentalDetailViewModel.RentalDate),
                HeaderText = "租借日期",
                FillWeight = 12
            };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvRentedComics.Columns.Add(rentalDateColumn);

            var returnDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RentalDetailViewModel.ExpectedReturnDate),
                HeaderText = "預定歸還日",
                FillWeight = 12
            };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvRentedComics.Columns.Add(returnDateColumn);

            var actualReturnTimeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RentalDetailViewModel.ActualReturnTime),
                HeaderText = "實際歸還時間",
                FillWeight = 11
            };
            actualReturnTimeColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            dgvRentedComics.Columns.Add(actualReturnTimeColumn);

            dgvRentedComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRentedComics.MultiSelect = false;
            dgvRentedComics.ReadOnly = true;
            dgvRentedComics.AllowUserToAddRows = false;
            LogActivity("dgvRentedComics 管理員視圖設定完成。");
        }

        private async void cmbMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null) return;
            if (!cmbMembers.IsHandleCreated || cmbMembers.IsDisposed) return; // Check handle

            if (cmbMembers.SelectedItem is Member selectedMember)
            {
                LogActivity($"cmbMembers_SelectedIndexChanged: Member selection changed. Selected Member ID: {selectedMember.Id}, Name: {selectedMember.Name}. Updating related comic lists asynchronously.");
            }
            else
            {
                LogActivity("cmbMembers_SelectedIndexChanged: Member selection cleared or invalid. Updating related comic lists asynchronously.");
            }
            await LoadAvailableComicsAsync();
            // Check handle again before next await, in case form closed during LoadAvailableComicsAsync
            if (!this.IsHandleCreated || this.IsDisposed) return;
            await LoadRentalDetailsAsync();
        }

        private async void btnRent_Click(object sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null)
            {
                MessageBox.Show("服務未初始化，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("btnRent_Click: Rent button clicked.");

            if (cmbMembers.SelectedValue == null)
            {
                MessageBox.Show("請選擇一位會員。", "租借提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("btnRent_Click: Rent operation aborted: No member selected.");
                return;
            }
            if (cmbComics.SelectedValue == null)
            {
                MessageBox.Show("請選擇一本要租借的漫畫。", "租借提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("btnRent_Click: Rent operation aborted: No comic selected.");
                return;
            }

            int memberId = (int)cmbMembers.SelectedValue;
            int comicId = (int)cmbComics.SelectedValue;

            Member? selectedMember = await _memberService.GetMemberByIdAsync(memberId);
            if (!this.IsHandleCreated || this.IsDisposed) return; // Check after await

            Comic? selectedComic = await _comicService.GetComicByIdAsync(comicId);
            if (!this.IsHandleCreated || this.IsDisposed) return; // Check after await

            if (selectedMember == null)
            {
                MessageBox.Show("選擇的會員資料無效，可能已被刪除。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"btnRent_Click: Rent operation failed: Selected member with ID {memberId} not found.");
                await RefreshUIDataSafelyAsync();
                return;
            }
            if (selectedComic == null)
            {
                MessageBox.Show("選擇的漫畫資料無效，可能已被刪除或已被租借。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"btnRent_Click: Rent operation failed: Selected comic with ID {comicId} not found.");
                await RefreshUIDataSafelyAsync();
                return;
            }

            if (selectedComic.IsRented)
            {
                Member? currentRenter = await _memberService.GetMemberByIdAsync(selectedComic.RentedToMemberId);
                if (!this.IsHandleCreated || this.IsDisposed) return; // Check after await
                string renterName = currentRenter != null ? currentRenter.Name : "未知會員";
                MessageBox.Show($"漫畫 '{selectedComic.Title}' 已被會員 '{renterName}' (ID: {selectedComic.RentedToMemberId}) 租借。", "租借失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogActivity($"btnRent_Click: Rent operation failed: Comic '{selectedComic.Title}' (ID: {comicId}) is already rented to Member ID {selectedComic.RentedToMemberId}.");
                await RefreshUIDataSafelyAsync();
                return;
            }

            try
            {
                DateTime today = DateTime.Today;
                DateTime minRentalReturnDate = today.AddDays(3);
                DateTime maxRentalReturnDate = today.AddMonths(1);

                using (RentalPeriodForm rentalDialog = new RentalPeriodForm(minRentalReturnDate, maxRentalReturnDate))
                {
                    LogActivity($"btnRent_Click: Displaying RentalPeriodForm for member '{selectedMember.Name}' and comic '{selectedComic.Title}'. MinDate: {minRentalReturnDate:yyyy-MM-dd}, MaxDate: {maxRentalReturnDate:yyyy-MM-dd}");
                    if (rentalDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        DateTime selectedReturnDate = rentalDialog.SelectedReturnDate;
                        LogActivity($"btnRent_Click: RentalPeriodForm accepted. Selected return date: {selectedReturnDate:yyyy-MM-dd}");

                        selectedComic.IsRented = true;
                        selectedComic.RentedToMemberId = selectedMember.Id;
                        selectedComic.RentalDate = DateTime.Now;
                        selectedComic.ReturnDate = selectedReturnDate;
                        selectedComic.ActualReturnTime = null;

                        await _comicService.UpdateComicAsync(selectedComic);
                        if (!this.IsHandleCreated || this.IsDisposed) return; // Check after await

                        LogActivity($"btnRent_Click: Comic '{selectedComic.Title}' (ID: {comicId}) successfully rented to member '{selectedMember.Name}' (ID: {memberId}). Rental Date: {selectedComic.RentalDate:yyyy-MM-dd HH:mm}, Expected Return: {selectedComic.ReturnDate:yyyy-MM-dd}.");
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' 已成功租借給會員 '{selectedMember.Name}'。\n租借日期: {selectedComic.RentalDate:yyyy-MM-dd}\n預計歸還日期: {selectedComic.ReturnDate:yyyy-MM-dd}", "租借成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh data - MembersChanged/ComicsChanged should ideally trigger this via Service_DataChanged.
                        // However, explicit refresh here ensures UI updates immediately after this specific operation.
                        await LoadRentalDetailsAsync();
                        if (!this.IsHandleCreated || this.IsDisposed) return;
                        await LoadAvailableComicsAsync();
                    }
                    else
                    {
                        LogActivity($"btnRent_Click: User cancelled RentalPeriodForm for member '{selectedMember.Name}' and comic '{selectedComic.Title}'. Rent process aborted.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorActivity($"btnRent_Click: Error renting comic ID: {comicId} for member ID: {memberId}.", ex);
                MessageBox.Show($"租借漫畫時發生錯誤: {ex.Message}\n請查看日誌以獲取詳細資訊。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnReturn_Click(object sender, EventArgs e)
        {
            // Debug logging for selection
            if (dgvRentedComics.SelectedRows.Count > 0)
            {
                if (dgvRentedComics.SelectedRows[0].DataBoundItem is RentalDetailViewModel selectedRentalDetailForDebug)
                    LogActivity($"btnReturn_Click Debug: SelectedRows[0] - ComicId: {selectedRentalDetailForDebug.ComicId}, Title: {selectedRentalDetailForDebug.ComicTitle}");
                else
                    LogActivity("btnReturn_Click Debug: SelectedRows[0].DataBoundItem is null or not a RentalDetailViewModel.");
            }
            else
            {
                LogActivity("btnReturn_Click Debug: No rows selected in dgvRentedComics.");
            }


            if (_comicService == null || _memberService == null)
            {
                MessageBox.Show("服務未初始化，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("btnReturn_Click: Return button clicked.");

            if (dgvRentedComics.SelectedRows.Count == 0)
            {
                MessageBox.Show("請從下方列表選擇一筆要歸還的租借記錄。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("btnReturn_Click: Return operation aborted: No record selected from dgvRentedComics.");
                return;
            }

            if (dgvRentedComics.SelectedRows[0].DataBoundItem is not RentalDetailViewModel selectedRentalDetail)
            {
                MessageBox.Show("無法獲取選定的租借資料，請重試。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("btnReturn_Click: Return operation failed: Selected item in dgvRentedComics is invalid or not a RentalDetailViewModel.");
                return;
            }

            Comic? comicFromService = await _comicService.GetComicByIdAsync(selectedRentalDetail.ComicId);
            if (!this.IsHandleCreated || this.IsDisposed) return;

            if (comicFromService == null)
            {
                MessageBox.Show($"漫畫 '{selectedRentalDetail.ComicTitle}' (ID: {selectedRentalDetail.ComicId}) 在系統中已不存在，可能已被刪除。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogErrorActivity($"btnReturn_Click: Return operation failed: Comic '{selectedRentalDetail.ComicTitle}' (ID: {selectedRentalDetail.ComicId}) not found in service layer.");
                await LoadRentalDetailsAsync();
                return;
            }

            if (!comicFromService.IsRented)
            {
                MessageBox.Show($"漫畫 '{comicFromService.Title}' (ID: {comicFromService.Id}) 的狀態已為未租借。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity($"btnReturn_Click: Return operation noted: Comic '{comicFromService.Title}' (ID: {comicFromService.Id}) is already marked as not rented.");
                await LoadRentalDetailsAsync();
                return;
            }

            try
            {
                int previouslyRentedByMemberId = comicFromService.RentedToMemberId;
                comicFromService.IsRented = false;
                comicFromService.RentedToMemberId = 0;
                comicFromService.ActualReturnTime = dtpActualReturnTime.Value;

                await _comicService.UpdateComicAsync(comicFromService);
                if (!this.IsHandleCreated || this.IsDisposed) return;

                Member? returningMember = await _memberService.GetMemberByIdAsync(previouslyRentedByMemberId);
                if (!this.IsHandleCreated || this.IsDisposed && returningMember == null) { /* Minor check, if form closed and member also not found, just exit */ }


                string returningMemberName = returningMember?.Name ?? $"ID: {previouslyRentedByMemberId}";

                LogActivity($"btnReturn_Click: Comic '{comicFromService.Title}' (ID: {comicFromService.Id}) successfully returned (was rented by member '{returningMemberName}').");
                MessageBox.Show($"漫畫 '{comicFromService.Title}' 已成功歸還。", "歸還成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh data - similar to btnRent_Click, explicit refresh for immediate UI update.
                await LoadRentalDetailsAsync();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"btnReturn_Click: Error returning comic ID: {comicFromService.Id}.", ex);
                MessageBox.Show($"歸還漫畫時發生錯誤: {ex.Message}\n請查看日誌以獲取詳細資訊。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity("租借表單正在關閉。正在取消訂閱服務事件。");

            _reloadService?.Stop();

            if (_comicService != null)
            {
                _comicService.ComicsChanged -= Service_DataChanged;
            }
            if (_memberService != null)
            {
                _memberService.MembersChanged -= Service_DataChanged;
            }
            base.OnFormClosing(e);
        }
    }
}
