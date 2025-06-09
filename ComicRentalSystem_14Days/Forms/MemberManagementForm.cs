using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class MemberManagementForm : ComicRentalSystem_14Days.BaseForm
    {
        private MemberService? _memberService;
        private AuthenticationService? _authenticationService;
        private readonly IComicService? _comicService;
        private readonly User? _currentUser;

        public MemberManagementForm()
        {
            InitializeComponent();
        }

        public MemberManagementForm(ILogger logger, MemberService memberService, AuthenticationService authenticationService, IComicService comicService, User? currentUser) : base(logger)
        {
            InitializeComponent();
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _currentUser = currentUser;
            LogActivity("會員管理表單正在使用 MemberService、AuthenticationService 和 ComicService 初始化。");
        }

        private async void MemberManagementForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || Logger == null || _memberService == null || _authenticationService == null || _comicService == null)
            {
                return;
            }

            LogActivity("MemberManagementForm_Load: Form loading, attaching event handlers and loading initial data.");

            // Ensure event is not subscribed multiple times
            _memberService.MembersChanged -= MemberService_MembersChanged; // Defensive
            _memberService.MembersChanged += MemberService_MembersChanged;

            SetupDataGridView();
            await LoadMembersDataAsync();

            LogActivity("MemberManagementForm_Load: Form successfully initialized with async data load.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity($"MemberManagementForm closing. User: {_currentUser?.Username ?? "N/A"}");
            if (_memberService != null)
            {
                _memberService.MembersChanged -= MemberService_MembersChanged;
                LogActivity("已取消訂閱 MemberService.MembersChanged 事件。");
            }
            base.OnFormClosing(e);
        }

        private async void MemberService_MembersChanged(object? sender, EventArgs e)
        {
            if (_memberService == null) return;
            LogActivity("MemberService_MembersChanged: Received MembersChanged event. Reloading data asynchronously.");
            await LoadMembersDataAsync();
        }

        private void SetupDataGridView()
        {
            LogActivity("正在設定會員的 DataGridView 資料行。");
            dgvMembers.AutoGenerateColumns = false;
            dgvMembers.Columns.Clear();

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "姓名", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "電話號碼", Width = 150 });

            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.MultiSelect = false;
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            LogActivity("會員的 DataGridView 設定完成。");
        }

        private async Task LoadMembersDataAsync()
        {
            if (_memberService == null) return;

            string searchTerm = this.txtSearchMembers.Text.Trim();

            LogActivity($"LoadMembersDataAsync: Attempting to load members data. Search term: '{searchTerm}'.");

            try
            {
                List<Member> members;
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    members = await _memberService.GetAllMembersAsync();
                }
                else
                {
                    members = await _memberService.SearchMembersAsync(searchTerm);
                }

                Action updateGrid = () => {
                    dgvMembers.DataSource = null;
                    dgvMembers.DataSource = members;
                };

                if (dgvMembers.IsHandleCreated && !dgvMembers.IsDisposed)
                {
                    if (dgvMembers.InvokeRequired)
                    {
                        dgvMembers.Invoke(updateGrid);
                    }
                    else
                    {
                        updateGrid();
                    }
                }
                LogActivity($"已成功使用搜尋關鍵字 '{searchTerm}' 將 {members.Count} 位會員載入 DataGridView。");
            }
            catch (Exception ex)
            {
                LogErrorActivity($"使用搜尋關鍵字 '{searchTerm}' 載入會員資料時發生錯誤。", ex);
                Action showError = () => MessageBox.Show($"載入會員資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    if (this.InvokeRequired) { this.Invoke(showError); } else { showError(); }
                }
            }
        }

        private async void btnSearchMembers_Click(object? sender, EventArgs e)
        {
            LogActivity("btnSearchMembers_Click: Search button clicked. Reloading data asynchronously.");
            await LoadMembersDataAsync();
        }

        private async void btnClearSearchMembers_Click(object? sender, EventArgs e)
        {
            LogActivity("btnClearSearchMembers_Click: Clear search button clicked. Reloading data asynchronously.");
            this.txtSearchMembers.Text = string.Empty;
            await LoadMembersDataAsync();
        }

        private async void btnRefreshMembers_Click(object sender, EventArgs e)
        {
            if (_memberService == null) return;
            LogActivity("Refresh Members button clicked. Will reload members from file asynchronously.");
            try
            {
                await _memberService.ReloadAsync();
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error refreshing members data from file.", ex);
                MessageBox.Show($"重新載入會員資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            if (_memberService == null || Logger == null || _authenticationService == null)
            {
                LogErrorActivity("新增會員所需的基本服務不可用。", new InvalidOperationException("服務未初始化。"));
                MessageBox.Show("無法開啟註冊表單，必要的服務未初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("新增會員按鈕已點擊。正在為新會員開啟 RegistrationForm。");
            using (RegistrationForm regForm = new RegistrationForm(Logger, _authenticationService, _memberService, _currentUser))
            {
                if (regForm.ShowDialog(this) == DialogResult.OK)
                {
                    LogActivity("btnAddMember_Click: RegistrationForm (add member) closed with OK. Data reload will be handled by MembersChanged event, or explicitly if needed.");
                    // MembersChanged should ideally handle the reload.
                    // However, if an explicit reload is desired here for immediate effect:
                    // await LoadMembersDataAsync(); 
                    // For now, relying on MembersChanged event from service.
                }
                else
                {
                    LogActivity("RegistrationForm (新增會員) 已關閉並回傳 Cancel 或其他。");
                }
            }
        }

        private void btnEditMember_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0 && _memberService != null && Logger != null)
            {
                Member? selectedMember = dgvMembers.SelectedRows[0].DataBoundItem as Member;

                if (selectedMember != null)
                {
                    LogActivity($"正在為編輯會員 ID: {selectedMember.Id}，姓名: '{selectedMember.Name}' 開啟 MemberEditForm。");
                    using (MemberEditForm editForm = new MemberEditForm(selectedMember, _memberService, Logger))
                    {
                        if (editForm.ShowDialog(this) == DialogResult.OK)
                        {
                            LogActivity($"會員 ID: {selectedMember.Id} 的 MemberEditForm (編輯模式) 已關閉並回傳 OK。資料重新整理將由 MembersChanged 事件處理。");
                        }
                        else
                        {
                            LogActivity($"會員 ID: {selectedMember.Id} 的 MemberEditForm (編輯模式) 已關閉並回傳 Cancel 或其他。");
                        }
                    }
                }
                else
                {
                    LogErrorActivity("無法從 DataGridView 擷取選定的會員資料進行編輯。");
                    MessageBox.Show("無法取得選定的會員資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogActivity("編輯會員按鈕已點擊，但未選取任何會員或服務未就緒。");
                MessageBox.Show("請先選擇一位要編輯的會員。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnDeleteMember_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0 && _memberService != null && _comicService != null)
            {
                Member? selectedMember = dgvMembers.SelectedRows[0].DataBoundItem as Member;
                if (selectedMember != null)
                {
                    LogActivity($"btnDeleteMember_Click: Attempting to delete member ID: {selectedMember.Id}, Name: '{selectedMember.Name}'.");

                    // Asynchronous check for active rentals using the new specific method
                    bool hasActiveRentals = await _comicService.HasComicsRentedByMemberAsync(selectedMember.Id);
                    if (hasActiveRentals)
                    {
                        MessageBox.Show($"會員 '{selectedMember.Name}' (ID: {selectedMember.Id}) 尚有未歸還的漫畫，無法刪除。", "刪除錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogActivity($"btnDeleteMember_Click: Attempt to delete member ID: {selectedMember.Id}, Name: '{selectedMember.Name}' failed: Member has active rentals.");
                        return;
                    }
                    // Removed the _comicService == null check here as it's part of the initial if condition.

                    LogActivity($"btnDeleteMember_Click: Showing confirmation dialog for deleting member ID: {selectedMember.Id}, Name: '{selectedMember.Name}'.");
                    var confirmResult = MessageBox.Show($"您確定要刪除會員 '{selectedMember.Name}' (ID: {selectedMember.Id}) 嗎？\n此操作無法復原。", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        LogActivity($"btnDeleteMember_Click: User confirmed deletion for member ID: {selectedMember.Id}.");
                        try
                        {
                            await _memberService.DeleteMemberAsync(selectedMember.Id);
                            string usernameToDelete = selectedMember.Username;

                            // AuthenticationService interaction remains synchronous as per subtask scope
                            if (_authenticationService != null)
                            {
                                bool userDeleted = _authenticationService.DeleteUser(usernameToDelete);
                                if (userDeleted)
                                {
                                    LogActivity($"btnDeleteMember_Click: Member ID: {selectedMember.Id} and associated user account '{selectedMember.Username}' successfully marked for deletion by services. UI should refresh via MembersChanged event.");
                                    MessageBox.Show($"會員 '{selectedMember.Name}' (使用者名稱: '{usernameToDelete}') 及其使用者帳戶已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    Logger?.LogWarning($"btnDeleteMember_Click: User account for Username: '{usernameToDelete}' (Member Name: '{selectedMember.Name}') not found by AuthenticationService, though member record was deleted.");
                                    LogActivity($"btnDeleteMember_Click: Member ID: {selectedMember.Id} (no user account or not found) marked for deletion. UI should refresh via event.");
                                    MessageBox.Show($"會員 '{selectedMember.Name}' 已從會員列表中刪除，但對應的使用者帳戶 '{usernameToDelete}' 未找到或無法刪除。", "部分成功", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else
                            {
                                LogErrorActivity("btnDeleteMember_Click: AuthenticationService is null. Cannot delete user account.", new InvalidOperationException("_authenticationService is null"));
                                MessageBox.Show($"會員 '{selectedMember.Name}' 已從會員列表中刪除，但由於內部錯誤無法刪除其使用者帳戶。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (InvalidOperationException opEx)
                        {
                            LogErrorActivity($"btnDeleteMember_Click: Operation error deleting member ID: {selectedMember.Id}.", opEx);
                            MessageBox.Show(opEx.Message, "操作錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            LogErrorActivity($"刪除會員 ID: {selectedMember.Id} 時發生一般錯誤。", ex);
                            MessageBox.Show($"刪除會員時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        LogActivity($"使用者已取消刪除會員 ID: {selectedMember.Id}。");
                    }
                }
            }
            else
            {
                LogActivity("刪除會員按鈕已點擊，但未選取任何會員或服務未就緒。");
                MessageBox.Show("請先選擇一位要刪除的會員。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvMembers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LogActivity($"DataGridView 儲存格在資料列 {e.RowIndex} 被雙擊。觸發會員編輯動作。");
                btnEditMember_Click(sender, e);
            }
        }

        private void btnChangeUserRole_Click(object? sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("請選擇一位會員。", "未選擇會員", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_authenticationService == null || Logger == null)
            {
                MessageBox.Show("必要的服務不可用。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("變更使用者角色所需服務不可用。", new InvalidOperationException("服務未初始化。"));
                return;
            }

            Member? selectedMember = dgvMembers.SelectedRows[0].DataBoundItem as Member;
            if (selectedMember == null || string.IsNullOrEmpty(selectedMember.Username))
            {
                MessageBox.Show("選定的會員沒有關聯的使用者名稱或資料無效。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("Selected member for role change has no username or is invalid.");
                return;
            }

            User? userToEdit = _authenticationService.GetUserByUsername(selectedMember.Username);
            if (userToEdit == null)
            {
                MessageBox.Show($"找不到使用者帳戶 '{selectedMember.Username}'。", "找不到使用者", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"User account for member '{selectedMember.Name}' (username: {selectedMember.Username}) not found for role change.");
                return;
            }

            LogActivity($"Opening ChangeUserRoleForm for member '{selectedMember.Name}', user '{userToEdit.Username}'.");
            using (ChangeUserRoleForm changeRoleForm = new ChangeUserRoleForm(userToEdit, _authenticationService, Logger))
            {
                changeRoleForm.ShowDialog(this);
            }
        }
    }
}