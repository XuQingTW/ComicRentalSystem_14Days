using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models; // 用於 RentalDetailViewModel
using ComicRentalSystem_14Days.Services;
using System;
using System.Collections.Generic; // 用於 List
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
        ) : base(logger) // 初始化服務與記錄器
        {
            InitializeComponent();

            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _reloadService = reloadService ?? throw new ArgumentException(nameof(reloadService));

            if (_comicService != null)
            {
                _comicService.ComicsChanged += Service_DataChanged; // 訂閱 ComicsChanged 事件
            }
            if (_memberService != null)
            {
                _memberService.MembersChanged += Service_DataChanged;
            }

            LogActivity("租借表單初始化中 (使用服務)。");
        }

        private void RentalForm_Load(object sender, EventArgs e)
        {
            LogActivity("租借表單正在載入資料。");
            if (_comicService != null && _memberService != null)
            {
                // 設定 DataGridViews 並載入初始資料
                SetupRentedComicsDataGridView(); // 設定已租借漫畫的 DataGridView
                LoadMembers();
                LoadAvailableComics(); // 初始載入可租借漫畫
                LoadRentalDetails(); // 已更新呼叫 // 初始載入已租借漫畫

                _reloadService?.Start(
                    async () => // Lambda 現在是 async Task
                    {
                        LogActivity("自動重新載入資料開始 (非同步)");
                        if (_comicService != null) await _comicService.ReloadAsync(); // 等候非同步重新載入
                        if (_memberService != null) await _memberService.ReloadAsync(); // 等候非同步重新載入
                        LogActivity("自動重新載入資料完成 (非同步)");
                    },
                    TimeSpan.FromSeconds(30)
                );

                if (cmbMembers.Items.Count == 0)
                {
                    LoadAvailableComics();
                    LoadRentalDetails(); // 已更新呼叫
                }
                LogActivity("租借表單已成功載入資料。");
            }
            else
            {
                LogActivity("租借表單已載入 (設計模式或未提供服務)。正在跳過資料載入。");
            }
        }

        private void Service_DataChanged(object? sender, EventArgs e) // 當漫畫資料變更時 (例如租借或歸還後) 會引發此事件
        {
            if (_comicService == null || _memberService == null) return;

            LogActivity($"從 {(sender is ComicService ? "ComicService" : "MemberService")} 收到資料變更事件。正在更新UI。");
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

        private void RefreshUIDataSafely() // 重新整理兩個列表
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

            LoadRentalDetails(); // 已更新呼叫
        }

        private void LoadMembers()
        {
            if (_memberService == null) return;

            LogActivity("正在將會員載入到 cmbMembers。");
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
                LogActivity($"已將 {members.Count} 位會員載入到 cmbMembers。");
            }
            catch (Exception ex)
            {
                LogErrorActivity("將會員載入到 cmbMembers 時發生錯誤。", ex);
                MessageBox.Show("載入會員列表時發生錯誤，請查看日誌。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvailableComics()
        {
            if (_comicService == null) return;

            LogActivity("正在將可借閱 (未租借) 的漫畫載入到 cmbComics。");
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
                LogActivity($"已將 {availableComics.Count} 本可借閱漫畫載入到 cmbComics。");
            }
            catch (Exception ex)
            {
                LogErrorActivity("將可借閱漫畫載入到 cmbComics 時發生錯誤。", ex);
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

        LogActivity("正在為管理員視圖載入所有租借詳細資料。");
        try
        {
            var allComics = _comicService.GetAllComics();
            var allMembers = _memberService.GetAllMembers();

            var rentedComicsQuery = allComics.Where(c => c.IsRented && c.RentedToMemberId != 0);

            // 如果在 cmbMembers 中選擇了會員，則依選定會員篩選
            Member? selectedMember = cmbMembers.SelectedItem as Member;
            if (selectedMember != null)
            {
                LogActivity($"正在為選定會員篩選租借詳細資料: {selectedMember.Name} (ID: {selectedMember.Id})");
                rentedComicsQuery = rentedComicsQuery.Where(c => c.RentedToMemberId == selectedMember.Id);
            }
            else
            {
                LogActivity("未選定特定會員。正在載入所有已租借漫畫。");
            }

            var rentalDetails = rentedComicsQuery
                .Select(comic => // 對應至 ViewModel
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
                        ExpectedReturnDate = comic.ReturnDate, // 假設 Comic.ReturnDate 儲存預計歸還日期
                        ActualReturnTime = comic.ActualReturnTime
                    };
                })
                .ToList();

            if (dgvRentedComics != null)
            {
                dgvRentedComics.DataSource = null;
                dgvRentedComics.DataSource = rentalDetails;
            }
            LogActivity($"已載入 {rentalDetails.Count} 筆租借詳細資料。");
        }
        catch (Exception ex)
        {
            LogErrorActivity("載入租借詳細資料時發生錯誤。", ex);
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

        var rentalDateColumn = new DataGridViewTextBoxColumn {
            DataPropertyName = nameof(RentalDetailViewModel.RentalDate),
            HeaderText = "租借日期",
            FillWeight = 12 // 已調整
        };
        rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvRentedComics.Columns.Add(rentalDateColumn);

        var returnDateColumn = new DataGridViewTextBoxColumn {
            DataPropertyName = nameof(RentalDetailViewModel.ExpectedReturnDate),
            HeaderText = "預定歸還日",
            FillWeight = 12 // 已調整
        };
        returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvRentedComics.Columns.Add(returnDateColumn);

        var actualReturnTimeColumn = new DataGridViewTextBoxColumn { // 新增資料行
            DataPropertyName = nameof(RentalDetailViewModel.ActualReturnTime),
            HeaderText = "實際歸還時間",
            FillWeight = 11 // 已調整
        };
        actualReturnTimeColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        dgvRentedComics.Columns.Add(actualReturnTimeColumn);

        // 保留現有屬性
        dgvRentedComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRentedComics.MultiSelect = false;
        dgvRentedComics.ReadOnly = true;
        dgvRentedComics.AllowUserToAddRows = false;
        LogActivity("dgvRentedComics 管理員視圖設定完成。");
    }

        private void cmbMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comicService == null || _memberService == null) return;

            if (cmbMembers.SelectedItem is Member selectedMember)
            {
                LogActivity($"會員選擇已變更。選定會員ID: {selectedMember.Id}, 姓名: {selectedMember.Name}。正在更新相關漫畫列表。");
            }
            else
            {
                LogActivity("會員選擇已清除或無效。正在更新相關漫畫列表。");
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
            LogActivity("租借按鈕已點擊。");

            if (cmbMembers.SelectedValue == null)
            {
                MessageBox.Show("請選擇一位會員。", "租借提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("租借操作中止: 未選擇會員。");
                return;
            }
            if (cmbComics.SelectedValue == null)
            {
                MessageBox.Show("請選擇一本要租借的漫畫。", "租借提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogActivity("租借操作中止: 未選擇漫畫。");
                return;
            }

            int memberId = (int)cmbMembers.SelectedValue;
            int comicId = (int)cmbComics.SelectedValue;

            Member? selectedMember = _memberService.GetMemberById(memberId);
            Comic? selectedComic = _comicService.GetComicById(comicId);

            if (selectedMember == null)
            {
                MessageBox.Show("選擇的會員資料無效，可能已被刪除。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"租借操作失敗: 找不到ID為 {memberId} 的選定會員。");
                RefreshUIDataSafely();
                return;
            }
            if (selectedComic == null)
            {
                MessageBox.Show("選擇的漫畫資料無效，可能已被刪除或已被租借。", "租借錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"租借操作失敗: 找不到ID為 {comicId} 的選定漫畫。");
                RefreshUIDataSafely();
                return;
            }

            if (selectedComic.IsRented)
            {
                Member? currentRenter = _memberService.GetMemberById(selectedComic.RentedToMemberId);
                string renterName = currentRenter != null ? currentRenter.Name : "未知會員";
                MessageBox.Show($"漫畫 '{selectedComic.Title}' 已被會員 '{renterName}' (ID: {selectedComic.RentedToMemberId}) 租借。", "租借失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogActivity($"租借操作失敗: 漫畫 '{selectedComic.Title}' (ID: {comicId}) 已被會員ID {selectedComic.RentedToMemberId} 租借。");
                RefreshUIDataSafely();
                return;
            }

            try
            {
                // 定義租借期限約束
                DateTime today = DateTime.Today;
                DateTime minRentalReturnDate = today.AddDays(3); // 範例：最短租期3天
                DateTime maxRentalReturnDate = today.AddMonths(1); // 範例：最長租期1個月

                using (RentalPeriodForm rentalDialog = new RentalPeriodForm(minRentalReturnDate, maxRentalReturnDate)) // 顯示租借期限選取對話方塊
                {
                    LogActivity($"正在為會員 '{selectedMember.Name}' 的漫畫 '{selectedComic.Title}' 顯示租借期限表單。最小日期: {minRentalReturnDate:yyyy-MM-dd}, 最大日期: {maxRentalReturnDate:yyyy-MM-dd}");
                    if (rentalDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        DateTime selectedReturnDate = rentalDialog.SelectedReturnDate;
                        LogActivity($"租借期限表單已接受。選定歸還日期: {selectedReturnDate:yyyy-MM-dd}");

                        selectedComic.IsRented = true;
                        selectedComic.RentedToMemberId = selectedMember.Id;
                        selectedComic.RentalDate = DateTime.Now; // 設定目前日期/時間為租借日期
                        selectedComic.ReturnDate = selectedReturnDate; // 從對話方塊設定預計歸還日期
                        selectedComic.ActualReturnTime = null; // 明確設為 null

                        _comicService.UpdateComic(selectedComic); // 更新漫畫狀態 // 儲存變更

                        LogActivity($"漫畫 '{selectedComic.Title}' (ID: {comicId}) 已成功租借給會員 '{selectedMember.Name}' (ID: {memberId})。租借日期: {selectedComic.RentalDate:yyyy-MM-dd HH:mm}, 預計歸還日期: {selectedComic.ReturnDate:yyyy-MM-dd}。");
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' 已成功租借給會員 '{selectedMember.Name}'。\n租借日期: {selectedComic.RentalDate:yyyy-MM-dd}\n預計歸還日期: {selectedComic.ReturnDate:yyyy-MM-dd}", "租借成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRentalDetails(); // 重新整理列表
                        LoadAvailableComics(); // 重新整理列表
                    }
                    else
                    {
                        LogActivity($"使用者已為會員 '{selectedMember.Name}' 的漫畫 '{selectedComic.Title}' 取消租借期限表單。租借流程中止。");
                        // 如果不明顯，可選擇性地通知使用者租借已取消
                        // MessageBox.Show("租借操作已取消。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorActivity($"為會員ID: {memberId} 租借漫畫ID: {comicId} 時發生錯誤。", ex);
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
                LogActivity($"偵錯: SelectedRows[0] - ComicId: {selectedRentalDetailForDebug.ComicId}, 書名: {selectedRentalDetailForDebug.ComicTitle}");
            }
            else
            {
                LogActivity("偵錯: SelectedRows[0].DataBoundItem 為空或不是 RentalDetailViewModel。");
            }

            for (int i = 0; i < dgvRentedComics.SelectedRows.Count; i++)
            {
                RentalDetailViewModel? rowDetail = dgvRentedComics.SelectedRows[i].DataBoundItem as RentalDetailViewModel;
                if (rowDetail != null)
                {
                    LogActivity($"偵錯: SelectedRows[{i}] - ComicId: {rowDetail.ComicId}, 書名: {rowDetail.ComicTitle}");
                }
            }
        }

        // ... (initial checks for services)
        if (_comicService == null || _memberService == null)
        {
            MessageBox.Show("服務未初始化，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        LogActivity("歸還按鈕已點擊。"); // This log is now after the debug logs, which is fine.

        if (dgvRentedComics.SelectedRows.Count == 0)
        {
            MessageBox.Show("請從下方列表選擇一筆要歸還的租借記錄。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LogActivity("歸還操作中止: 未從 dgvRentedComics 選取記錄。");
            return;
        }

        RentalDetailViewModel? selectedRentalDetail = dgvRentedComics.SelectedRows[0].DataBoundItem as RentalDetailViewModel;

        if (selectedRentalDetail == null)
        {
            MessageBox.Show("無法獲取選定的租借資料，請重試。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogErrorActivity("歸還操作失敗: dgvRentedComics 中的選定項目無效、為空或不是 RentalDetailViewModel。");
            return;
        }

        Comic? comicFromService = _comicService?.GetComicById(selectedRentalDetail.ComicId); // Use ComicId

        if (comicFromService == null)
        {
            MessageBox.Show($"漫畫 '{selectedRentalDetail.ComicTitle}' (ID: {selectedRentalDetail.ComicId}) 在系統中已不存在，可能已被刪除。", "歸還錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogErrorActivity($"歸還操作失敗: 在服務層找不到漫畫 '{selectedRentalDetail.ComicTitle}' (ID: {selectedRentalDetail.ComicId})。");
            LoadRentalDetails(); // Or LoadRentalDetails()
            return;
        }

        // ... (rest of the logic for checking if comic is rented, who rented it, etc.)
            // 比較 RentedToMemberId 的邏輯可能需要使用 selectedRentalDetail.MemberId
            // 如果您想確保歸還是針對該列中顯示的會員，
            // 儘管通常管理員可以歸還任何書籍。

        if (!comicFromService.IsRented)
        {
            MessageBox.Show($"漫畫 '{comicFromService.Title}' (ID: {comicFromService.Id}) 的狀態已為未租借。", "歸還提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LogActivity($"歸還操作註記: 漫畫 '{comicFromService.Title}' (ID: {comicFromService.Id}) 已標記為未租借。");
                LoadRentalDetails(); // 重新整理列表
            return;
        }

            // 如果您想確保正在為格線中顯示的正確會員內容處理歸還：
            // (如果會員是透過這個完全相同的介面歸還項目，則此檢查可能更相關，
            // 但對於管理員，他們可能會覆寫或代表任何人歸還)
            // 對於管理員來說，通常只需找到漫畫並將其標記為已歸還即可。
            // cmbMembers 中選定的會員可能與格線中所選漫畫的 RentedToMemberId 不符
            // 如果「顯示全部」已啟用。

        try
        {
                int previouslyRentedByMemberId = comicFromService.RentedToMemberId; // 這是實際的租借者。
            comicFromService.IsRented = false;
            comicFromService.RentedToMemberId = 0;
                // comicFromService.RentalDate = null; // 保留原始租借日期
                // comicFromService.ReturnDate = null; // 保留預計歸還日期 (如果它代表該日期)
                comicFromService.ActualReturnTime = dtpActualReturnTime.Value; // 從 DateTimePicker 設定實際歸還時間
                _comicService?.UpdateComic(comicFromService); // 儲存變更

            Member? returningMember = _memberService?.GetMemberById(previouslyRentedByMemberId);
            string returningMemberName = returningMember?.Name ?? $"ID: {previouslyRentedByMemberId}";

            LogActivity($"漫畫 '{comicFromService.Title}' (ID: {comicFromService.Id}) 已成功歸還 (由會員 '{returningMemberName}' 租借)。");
            MessageBox.Show($"漫畫 '{comicFromService.Title}' 已成功歸還。", "歸還成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRentalDetails(); // 重新整理列表
        }
        catch (Exception ex)
        {
            LogErrorActivity($"歸還漫畫ID: {comicFromService.Id} 時發生錯誤。", ex);
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
