using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
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
                LoadRentedComicsForSelectedMember();

                _reloadService?.Start(
                    async () =>
                    {
                        LogActivity("auto reloading data start");
                        _comicService?.Reload();
                        _memberService?.Reload(); // Added call to MemberService.Reload
                        await Task.CompletedTask;
                    },
                    TimeSpan.FromSeconds(1)
                );

                if (cmbMembers.Items.Count == 0)
                {
                    LoadAvailableComics();
                    LoadRentedComicsForSelectedMember();
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

            LoadRentedComicsForSelectedMember();
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

        private void LoadRentedComicsForSelectedMember()
        {
            if (_comicService == null || cmbMembers.SelectedValue == null)
            {
                if (dgvRentedComics != null) dgvRentedComics.DataSource = null; // 確保 dgvRentedComics 已初始化
                return;
            }

            LogActivity("Loading rented comics for the selected member into dgvRentedComics.");
            if (cmbMembers.SelectedValue is int memberId && memberId > 0)
            {
                try
                {
                    var rentedComics = _comicService.GetAllComics()
                                                    .Where(c => c.IsRented && c.RentedToMemberId == memberId)
                                                    .ToList();
                    if (dgvRentedComics != null)
                    {
                        dgvRentedComics.DataSource = null;
                        dgvRentedComics.DataSource = rentedComics;
                    }
                    LogActivity($"Loaded {rentedComics.Count} rented comics for member ID: {memberId}.");
                }
                catch (Exception ex)
                {
                    LogErrorActivity($"Error loading rented comics for member ID: {memberId}.", ex);
                    MessageBox.Show("載入該會員已租借漫畫列表時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (dgvRentedComics != null) dgvRentedComics.DataSource = null;
                LogActivity("No member selected or invalid member ID, dgvRentedComics cleared.");
            }
        }

        private void SetupRentedComicsDataGridView()
        {
            LogActivity("Setting up dgvRentedComics.");
            dgvRentedComics.AutoGenerateColumns = false;
            dgvRentedComics.Columns.Clear();

            dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.Id),
                HeaderText = "漫畫ID",
                Name = "colComicId",
                Width = 80
            });
            dgvRentedComics.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.Title),
                HeaderText = "書名",
                Name = "colComicTitle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // Add RentalDate column
            var rentalDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.RentalDate),
                HeaderText = "租借日期",
                Name = "colRentalDate",
                Width = 120 // Or another appropriate width
            };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvRentedComics.Columns.Add(rentalDateColumn);

            // Add ReturnDate column
            var returnDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Comic.ReturnDate),
                HeaderText = "歸還期限",
                Name = "colReturnDate",
                Width = 120 // Or another appropriate width
            };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvRentedComics.Columns.Add(returnDateColumn);

            dgvRentedComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRentedComics.MultiSelect = false;
            dgvRentedComics.ReadOnly = true;
            dgvRentedComics.AllowUserToAddRows = false;
            LogActivity("dgvRentedComics setup complete.");
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
            LoadRentedComicsForSelectedMember();
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
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error during rent operation for comic ID: {comicId} to member ID: {memberId}.", ex);
                MessageBox.Show($"租借漫畫時發生錯誤: {ex.Message}\n請查看日誌以獲取詳細資訊。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null)
            {
                MessageBox.Show("服務未初始化，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("Return button clicked.");

            if (dgvRentedComics.SelectedRows.Count == 0)
            {
                MessageBox.Show("請從下方列表選擇一本要歸還的漫畫。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("Return action aborted: No comic selected from dgvRentedComics.");
                return;
            }

            Comic? selectedComicToReturn = dgvRentedComics.SelectedRows[0].DataBoundItem as Comic;

            if (selectedComicToReturn == null)
            {
                MessageBox.Show("無法獲取選定的漫畫資料，請重試。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("Return action failed: Selected item in dgvRentedComics is not a valid Comic object or is null.");
                return;
            }

            Comic? comicFromService = _comicService.GetComicById(selectedComicToReturn.Id);

            if (comicFromService == null)
            {
                MessageBox.Show($"漫畫 '{selectedComicToReturn.Title}' (ID: {selectedComicToReturn.Id}) 在系統中已不存在，可能已被刪除。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogErrorActivity($"Return action failed: Comic '{selectedComicToReturn.Title}' (ID: {selectedComicToReturn.Id}) not found in service layer. It might have been deleted.");
                RefreshUIDataSafely();
                return;
            }

            if (!comicFromService.IsRented)
            {
                MessageBox.Show($"漫畫 '{comicFromService.Title}' (ID: {comicFromService.Id}) 的狀態已為未租借。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity($"Return action noted: Comic '{comicFromService.Title}' (ID: {comicFromService.Id}) is already marked as not rented.");
                RefreshUIDataSafely();
                return;
            }

            if (cmbMembers.SelectedItem is Member currentSelectedMember && comicFromService.RentedToMemberId != currentSelectedMember.Id)
            {
                Member? actualRenter = _memberService.GetMemberById(comicFromService.RentedToMemberId);
                string actualRenterName = actualRenter?.Name ?? "未知會員";
                MessageBox.Show($"漫畫 '{comicFromService.Title}' 目前由會員 '{actualRenterName}' (ID: {comicFromService.RentedToMemberId}) 租借，並非當前選定的會員 '{currentSelectedMember.Name}'。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogActivity($"Return action aborted: Comic '{comicFromService.Title}' is rented by member ID {comicFromService.RentedToMemberId}, not selected member ID {currentSelectedMember.Id}.");
                return;
            }

            try
            {
                int previouslyRentedByMemberId = comicFromService.RentedToMemberId;
                comicFromService.IsRented = false;
                comicFromService.RentedToMemberId = 0;
                _comicService.UpdateComic(comicFromService);

                Member? returningMember = _memberService.GetMemberById(previouslyRentedByMemberId);
                string returningMemberName = returningMember?.Name ?? $"ID: {previouslyRentedByMemberId}";

                LogActivity($"Comic '{comicFromService.Title}' (ID: {comicFromService.Id}) successfully returned by member '{returningMemberName}'.");
                MessageBox.Show($"漫畫 '{comicFromService.Title}' 已成功歸還。", "歸還成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
