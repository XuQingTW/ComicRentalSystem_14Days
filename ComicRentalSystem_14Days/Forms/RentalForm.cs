using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models; // For RentalDetailViewModel
using ComicRentalSystem_14Days.Services;
using System;
using System.Collections.Generic; // For List
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RentalForm : BaseForm
    {
        // 將欄位宣告為可為 Null
        private readonly ComicService? _comicService;
        private readonly MemberService? _memberService;
        private readonly IReloadService? _reloadService;
        // Logger 繼承自 BaseForm (protected ILogger? Logger)

        public RentalForm() : base()
        {
            InitializeComponent();
        }

        public RentalForm(
            ComicService comicService,
            MemberService memberService,
            ILogger logger,
            IReloadService reloadService
        ) : base(logger)
        {
            InitializeComponent();

            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _reloadService = reloadService ?? throw new ArgumentException(nameof(reloadService));

            if (_comicService != null)
            {
                _comicService.ComicsChanged += Service_DataChanged;
            }
            if (_memberService != null)
            {
                _memberService.MembersChanged += Service_DataChanged;
            }

            LogActivity("RentalForm initializing with services.");
        }

        private void RentalForm_Load(object sender, EventArgs e)
        {
            LogActivity("RentalForm loading data.");
            if (_comicService != null && _memberService != null)
            {
                SetupRentedComicsDataGridView();
                LoadMembers();
                LoadAvailableComics();
                LoadRentalDetails(); // Updated call

                _reloadService?.Start(
                    async () =>
                    {
                        LogActivity("auto reloading data start");
                        _comicService?.Reload();
                        _memberService?.Reload(); // Added call to MemberService.Reload
                        await Task.CompletedTask;
                    },
                    TimeSpan.FromSeconds(30)
                );

                if (cmbMembers.Items.Count == 0)
                {
                    LoadAvailableComics();
                    LoadRentalDetails(); // Updated call
                }
                LogActivity("RentalForm loaded successfully with data.");
            }
            else
            {
                LogActivity("RentalForm loaded (design mode or services not provided). Skipping data load.");
            }
        }

        private void Service_DataChanged(object? sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null) return;

            LogActivity($"DataChanged event received from {(sender is ComicService ? "ComicService" : "MemberService")}. Refreshing UI.");
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(RefreshUIDataSafely));
                }
                else
                {
                    RefreshUIDataSafely();
                }
            }
        }

        private void RefreshUIDataSafely()
        {
            if (_comicService == null || _memberService == null) return;

            object? selectedMemberVal = cmbMembers.SelectedValue;
            object? selectedComicVal = cmbComics.SelectedValue;

            LoadMembers();
            if (selectedMemberVal != null && cmbMembers.Items.Cast<Member>().Any(m => m.Id == (int)selectedMemberVal))
            {
                cmbMembers.SelectedValue = selectedMemberVal;
            }

            LoadAvailableComics();
            if (selectedComicVal != null && cmbComics.Items.Cast<Comic>().Any(c => c.Id == (int)selectedComicVal))
            {
                cmbComics.SelectedValue = selectedComicVal;
            }

            LoadRentalDetails(); // Updated call
        }

        private void LoadMembers()
        {
            if (_memberService == null) return;

            LogActivity("Loading members into cmbMembers.");
            try
            {
                var members = _memberService.GetAllMembers();
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
                LogActivity($"Loaded {members.Count} members into cmbMembers.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading members into cmbMembers.", ex);
                MessageBox.Show("載入會員列表時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvailableComics()
        {
            if (_comicService == null) return;

            LogActivity("Loading available (not rented) comics into cmbComics.");
            try
            {
                var availableComics = _comicService.GetAllComics().Where(c => !c.IsRented).ToList();
                cmbComics.DataSource = null;
                cmbComics.DataSource = availableComics;
                cmbComics.DisplayMember = nameof(Comic.Title);
                cmbComics.ValueMember = nameof(Comic.Id);

                if (!availableComics.Any())
                {
                    cmbComics.SelectedIndex = -1;
                    cmbComics.Text = " (無可用漫畫) ";
                }
                else
                {
                    if (cmbComics.SelectedIndex == -1) cmbComics.SelectedIndex = 0;
                }
                LogActivity($"Loaded {availableComics.Count} available comics into cmbComics.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading available comics into cmbComics.", ex);
                MessageBox.Show("載入可用漫畫列表時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    private void LoadRentalDetails() // Renamed and logic adjusted
    {
        if (_comicService == null || _memberService == null)
        {
            if (dgvRentedComics != null) dgvRentedComics.DataSource = null;
            return;
        }

        LogActivity("Loading all rental details for Admin View.");
        try
        {
            var allComics = _comicService.GetAllComics();
            var allMembers = _memberService.GetAllMembers();

            var rentedComicsQuery = allComics.Where(c => c.IsRented && c.RentedToMemberId != 0);

            // Filter by selected member if a member is chosen in cmbMembers
            Member? selectedMember = cmbMembers.SelectedItem as Member;
            if (selectedMember != null)
            {
                LogActivity($"Filtering rental details for selected member: {selectedMember.Name} (ID: {selectedMember.Id})");
                rentedComicsQuery = rentedComicsQuery.Where(c => c.RentedToMemberId == selectedMember.Id);
            }
            else
            {
                LogActivity("No specific member selected. Loading all rented comics.");
            }

            var rentalDetails = rentedComicsQuery
                .Select(comic =>
                {
                    var member = allMembers.FirstOrDefault(m => m.Id == comic.RentedToMemberId);
                    return new RentalDetailViewModel
                    {
                        MemberId = member?.Id ?? 0,
                        MemberName = member?.Name ?? "未知會員",
                        MemberPhoneNumber = member?.PhoneNumber ?? "N/A",
                        ComicId = comic.Id,
                        ComicTitle = comic.Title,
                        RentalDate = comic.RentalDate,
                        ExpectedReturnDate = comic.ReturnDate, // Assuming Comic.ReturnDate stores the expected return date
                        ActualReturnTime = comic.ActualReturnTime
                    };
                })
                .ToList();

            if (dgvRentedComics != null)
            {
                dgvRentedComics.DataSource = null;
                dgvRentedComics.DataSource = rentalDetails;
            }
            LogActivity($"Loaded {rentalDetails.Count} rental details.");
        }
        catch (Exception ex)
        {
            LogErrorActivity("Error loading rental details.", ex);
            MessageBox.Show("載入租借詳細資料時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetupRentedComicsDataGridView()
    {
        LogActivity("Setting up dgvRentedComics for Admin View.");
        dgvRentedComics.AutoGenerateColumns = false;
        dgvRentedComics.Columns.Clear();
        dgvRentedComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.MemberName), HeaderText = "會員姓名", FillWeight = 18 }); // Adjusted
        dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.MemberId), HeaderText = "會員ID", FillWeight = 9 }); // Adjusted
        dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.MemberPhoneNumber), HeaderText = "會員電話", FillWeight = 18 }); // Adjusted
        dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RentalDetailViewModel.ComicTitle), HeaderText = "租借書名", FillWeight = 20 }); // Adjusted

        var rentalDateColumn = new DataGridViewTextBoxColumn {
            DataPropertyName = nameof(RentalDetailViewModel.RentalDate),
            HeaderText = "租借日期",
            FillWeight = 12 // Adjusted
        };
        rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvRentedComics.Columns.Add(rentalDateColumn);

        var returnDateColumn = new DataGridViewTextBoxColumn {
            DataPropertyName = nameof(RentalDetailViewModel.ExpectedReturnDate),
            HeaderText = "預定歸還日",
            FillWeight = 12 // Adjusted
        };
        returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvRentedComics.Columns.Add(returnDateColumn);

        var actualReturnTimeColumn = new DataGridViewTextBoxColumn { // New Column
            DataPropertyName = nameof(RentalDetailViewModel.ActualReturnTime),
            HeaderText = "實際歸還時間",
            FillWeight = 11 // Adjusted
        };
        actualReturnTimeColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        dgvRentedComics.Columns.Add(actualReturnTimeColumn);

        // Keep existing properties
        dgvRentedComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRentedComics.MultiSelect = false;
        dgvRentedComics.ReadOnly = true;
        dgvRentedComics.AllowUserToAddRows = false;
        LogActivity("dgvRentedComics setup complete for Admin View.");
    }

        private void cmbMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null) return;

            if (cmbMembers.SelectedItem is Member selectedMember)
            {
                LogActivity($"Member selection changed. Selected Member ID: {selectedMember.Id}, Name: {selectedMember.Name}. Refreshing related comic lists.");
            }
            else
            {
                LogActivity("Member selection cleared or invalid. Refreshing related comic lists.");
            }
            LoadAvailableComics();
        LoadRentalDetails(); // Updated call
        }

        private void btnRent_Click(object sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null)
            {
                MessageBox.Show("服務未初始化，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("Rent button clicked.");

            if (cmbMembers.SelectedValue == null)
            {
                MessageBox.Show("請選擇一位會員。", "租借提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("Rent action aborted: No member selected.");
                return;
            }
            if (cmbComics.SelectedValue == null)
            {
                MessageBox.Show("請選擇一本要租借的漫畫。", "租借提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("Rent action aborted: No comic selected.");
                return;
            }

            int memberId = (int)cmbMembers.SelectedValue;
            int comicId = (int)cmbComics.SelectedValue;

            Member? selectedMember = _memberService.GetMemberById(memberId);
            Comic? selectedComic = _comicService.GetComicById(comicId);

            if (selectedMember == null)
            {
                MessageBox.Show("選擇的會員資料無效，可能已被刪除。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"Rent action failed: Selected member with ID {memberId} not found.");
                RefreshUIDataSafely();
                return;
            }
            if (selectedComic == null)
            {
                MessageBox.Show("選擇的漫畫資料無效，可能已被刪除或已被租借。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"Rent action failed: Selected comic with ID {comicId} not found.");
                RefreshUIDataSafely();
                return;
            }

            if (selectedComic.IsRented)
            {
                Member? currentRenter = _memberService.GetMemberById(selectedComic.RentedToMemberId);
                string renterName = currentRenter != null ? currentRenter.Name : "未知會員";
                MessageBox.Show($"漫畫 '{selectedComic.Title}' 已被會員 '{renterName}' (ID: {selectedComic.RentedToMemberId}) 租借。", "租借失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogActivity($"Rent action failed: Comic '{selectedComic.Title}' (ID: {comicId}) is already rented by member ID {selectedComic.RentedToMemberId}.");
                RefreshUIDataSafely();
                return;
            }

            try
            {
                selectedComic.IsRented = true;
                selectedComic.RentedToMemberId = selectedMember.Id;
                _comicService.UpdateComic(selectedComic);

                LogActivity($"Comic '{selectedComic.Title}' (ID: {comicId}) successfully rented to member '{selectedMember.Name}' (ID: {memberId}).");
                MessageBox.Show($"漫畫 '{selectedComic.Title}' 已成功租借給會員 '{selectedMember.Name}'。", "租借成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRentalDetails(); // Updated call
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error during rent operation for comic ID: {comicId} to member ID: {memberId}.", ex);
                MessageBox.Show($"租借漫畫時發生錯誤: {ex.Message}\n請查看日誌以獲取詳細資訊。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    private void btnReturn_Click(object sender, EventArgs e)
    {
        LogActivity($"dgvRentedComics.SelectedRows.Count: {dgvRentedComics.SelectedRows.Count}");
        if (dgvRentedComics.SelectedRows.Count > 0)
        {
            RentalDetailViewModel? selectedRentalDetailForDebug = dgvRentedComics.SelectedRows[0].DataBoundItem as RentalDetailViewModel;
            if (selectedRentalDetailForDebug != null)
            {
                LogActivity($"DEBUG: SelectedRows[0] - ComicId: {selectedRentalDetailForDebug.ComicId}, Title: {selectedRentalDetailForDebug.ComicTitle}");
            }
            else
            {
                LogActivity("DEBUG: SelectedRows[0].DataBoundItem is null or not a RentalDetailViewModel.");
            }

            for (int i = 0; i < dgvRentedComics.SelectedRows.Count; i++)
            {
                RentalDetailViewModel? rowDetail = dgvRentedComics.SelectedRows[i].DataBoundItem as RentalDetailViewModel;
                if (rowDetail != null)
                {
                    LogActivity($"DEBUG: SelectedRows[{i}] - ComicId: {rowDetail.ComicId}, Title: {rowDetail.ComicTitle}");
                }
            }
        }

        // ... (initial checks for services)
        if (_comicService == null || _memberService == null)
        {
            MessageBox.Show("服務未初始化，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        LogActivity("Return button clicked."); // This log is now after the debug logs, which is fine.

        if (dgvRentedComics.SelectedRows.Count == 0)
        {
            MessageBox.Show("請從下方列表選擇一筆要歸還的租借記錄。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LogActivity("Return action aborted: No record selected from dgvRentedComics.");
            return;
        }

        RentalDetailViewModel? selectedRentalDetail = dgvRentedComics.SelectedRows[0].DataBoundItem as RentalDetailViewModel;

        if (selectedRentalDetail == null)
        {
            MessageBox.Show("無法獲取選定的租借資料，請重試。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogErrorActivity("Return action failed: Selected item in dgvRentedComics is not a valid RentalDetailViewModel or is null.");
            return;
        }

        Comic? comicFromService = _comicService?.GetComicById(selectedRentalDetail.ComicId); // Use ComicId

        if (comicFromService == null)
        {
            MessageBox.Show($"漫畫 '{selectedRentalDetail.ComicTitle}' (ID: {selectedRentalDetail.ComicId}) 在系統中已不存在，可能已被刪除。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogErrorActivity($"Return action failed: Comic '{selectedRentalDetail.ComicTitle}' (ID: {selectedRentalDetail.ComicId}) not found in service layer.");
            LoadRentalDetails(); // Or LoadRentalDetails()
            return;
        }

        // ... (rest of the logic for checking if comic is rented, who rented it, etc.)
        // The logic for comparing RentedToMemberId might need to use selectedRentalDetail.MemberId
        // if you want to ensure the return is for the member shown in that row,
        // though typically an admin can return any book.

        if (!comicFromService.IsRented)
        {
            MessageBox.Show($"漫畫 '{comicFromService.Title}' (ID: {comicFromService.Id}) 的狀態已為未租借。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LogActivity($"Return action noted: Comic '{comicFromService.Title}' (ID: {comicFromService.Id}) is already marked as not rented.");
            LoadRentalDetails(); // Refresh
            return;
        }

        // If you want to ensure the return is being processed for the correct member context shown in the grid:
        // (This check might be more relevant if members were returning items through this exact same interface,
        // but for admins, they might be overriding or returning on behalf of anyone)
        // For an admin, simply finding the comic and marking it as returned is usually sufficient.
        // The selected member in cmbMembers might not align with the RentedToMemberId of the selected comic in the grid
        // if "Show All" is active.

        try
        {
            int previouslyRentedByMemberId = comicFromService.RentedToMemberId; // This is the actual renter.
            comicFromService.IsRented = false;
            comicFromService.RentedToMemberId = 0;
            // comicFromService.RentalDate = null; // Retain original rental date
            // comicFromService.ReturnDate = null; // Retain expected return date (if it represents that)
            comicFromService.ActualReturnTime = dtpActualReturnTime.Value; // Set the actual return time from DateTimePicker
            _comicService?.UpdateComic(comicFromService);

            Member? returningMember = _memberService?.GetMemberById(previouslyRentedByMemberId);
            string returningMemberName = returningMember?.Name ?? $"ID: {previouslyRentedByMemberId}";

            LogActivity($"Comic '{comicFromService.Title}' (ID: {comicFromService.Id}) successfully returned (was rented by member '{returningMemberName}').");
            MessageBox.Show($"漫畫 '{comicFromService.Title}' 已成功歸還。", "歸還成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRentalDetails(); // Refresh the list
        }
        catch (Exception ex)
        {
            LogErrorActivity($"Error during return operation for comic ID: {comicFromService.Id}.", ex);
            MessageBox.Show($"歸還漫畫時發生錯誤: {ex.Message}\n請查看日誌以獲取詳細資訊。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity("RentalForm closing. Unsubscribing from service events.");

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
