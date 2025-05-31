using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class MemberManagementForm : ComicRentalSystem_14Days.BaseForm
    {
        private MemberService? _memberService;
        private AuthenticationService? _authenticationService; // Added AuthenticationService
        private readonly ComicService? _comicService;
        private readonly User? _currentUser;

        // Conceptual private fields for new controls (designer would add these)
        // private System.Windows.Forms.TextBox txtSearchMembers;
        // private System.Windows.Forms.Button btnSearchMembers;
        // private System.Windows.Forms.Button btnClearSearchMembers;

        public MemberManagementForm()
        {
            InitializeComponent();
        }

        // Updated constructor to include AuthenticationService and ComicService
        public MemberManagementForm(ILogger logger, MemberService memberService, AuthenticationService authenticationService, ComicService comicService, User? currentUser) : base(logger)
        {
            InitializeComponent();
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _currentUser = currentUser;
            LogActivity("MemberManagementForm initializing with MemberService, AuthenticationService, and ComicService.");
        }

        private void MemberManagementForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || Logger == null || _memberService == null || _authenticationService == null || _comicService == null) // Added null check for _authenticationService and _comicService
            {
                return;
            }

            LogActivity("MemberManagementForm is loading runtime components.");

            _memberService.MembersChanged += MemberService_MembersChanged; // _memberService is confirmed not null here

            // Wire up event handlers for new search buttons
            Control? btnSearchMembersCtrl = this.Controls.Find("btnSearchMembers", true).FirstOrDefault();
            if (btnSearchMembersCtrl is Button btnSearch)
            {
                btnSearch.Click += new System.EventHandler(this.btnSearchMembers_Click);
            }

            Control? btnClearSearchMembersCtrl = this.Controls.Find("btnClearSearchMembers", true).FirstOrDefault();
            if (btnClearSearchMembersCtrl is Button btnClear)
            {
                btnClear.Click += new System.EventHandler(this.btnClearSearchMembers_Click);
            }

            SetupDataGridView(); // Already called in constructor
            LoadMembersData(); // Already called in constructor

            // Conceptual: Wire up btnChangeUserRole_Click. Assuming a button named btnChangeUserRole exists.
            Control? btnChangeRoleCtrl = this.Controls.Find("btnChangeUserRole", true).FirstOrDefault();
            if (btnChangeRoleCtrl is Button btnChangeRole)
            {
                btnChangeRole.Click += new System.EventHandler(this.btnChangeUserRole_Click);
            }
            LogActivity("MemberManagementForm initialized successfully.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity($"MemberManagementForm closing. User: {_currentUser?.Username ?? "N/A"}");
            if (_memberService != null)
            {
                _memberService.MembersChanged -= MemberService_MembersChanged;
                LogActivity("Unsubscribed from MemberService.MembersChanged event.");
            }
            base.OnFormClosing(e);
        }

        private void MemberService_MembersChanged(object? sender, EventArgs e)
        {
            if (_memberService == null) return;
            LogActivity("MembersChanged event received. Refreshing DataGridView.");
            LoadMembersData();
        }

        private void SetupDataGridView()
        {
            LogActivity("Setting up DataGridView columns for members.");
            dgvMembers.AutoGenerateColumns = false;
            dgvMembers.Columns.Clear();

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "姓名", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "電話號碼", Width = 150 });

            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.MultiSelect = false;
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            LogActivity("DataGridView setup complete for members.");
        }

        private void LoadMembersData()
        {
            if (_memberService == null) return;

            string searchTerm = string.Empty;
            Control? txtSearchMembersCtrl = this.Controls.Find("txtSearchMembers", true).FirstOrDefault();
            if (txtSearchMembersCtrl is TextBox txtSearch)
            {
                searchTerm = txtSearch.Text.Trim();
            }

            LogActivity($"Attempting to load members data. Search term: '{searchTerm}'.");

            try
            {
                List<Member> members;
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    members = _memberService.GetAllMembers();
                }
                else
                {
                    members = _memberService.SearchMembers(searchTerm);
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
                LogActivity($"Successfully loaded {members.Count} members into DataGridView with search term '{searchTerm}'.");
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error loading members data with search term '{searchTerm}'.", ex);
                Action showError = () => MessageBox.Show($"載入會員資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    if (this.InvokeRequired) { this.Invoke(showError); } else { showError(); }
                }
            }
        }

        private void btnSearchMembers_Click(object? sender, EventArgs e)
        {
            LogActivity("Search Members button clicked.");
            LoadMembersData();
        }

        private void btnClearSearchMembers_Click(object? sender, EventArgs e)
        {
            LogActivity("Clear Search Members button clicked.");
            Control? txtSearchMembersCtrl = this.Controls.Find("txtSearchMembers", true).FirstOrDefault();
            if (txtSearchMembersCtrl is TextBox txtSearch)
            {
                 txtSearch.Text = string.Empty;
            }
            LoadMembersData();
        }

        private async void btnRefreshMembers_Click(object sender, EventArgs e) // Made async void
        {
            if (_memberService == null) return;
            LogActivity("Refresh Members button clicked. Will reload members from file asynchronously.");
            try
            {
                await _memberService.ReloadAsync(); // Call async version
                // The MembersChanged event handler (MemberService_MembersChanged)
                // already calls LoadMembersData(), so no explicit call needed here.
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
                // Log an error or show a message if essential services are missing
                LogErrorActivity("Essential services not available for adding a member.", new InvalidOperationException("Services not initialized."));
                MessageBox.Show("無法開啟註冊表單，必要的服務未初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("Add Member button clicked by admin. Opening RegistrationForm for new user/member.");
            // Pass the current user (_currentUser, who is the admin) and other services to RegistrationForm
            using (RegistrationForm regForm = new RegistrationForm(Logger, _authenticationService, _memberService, _currentUser))
            {
                if (regForm.ShowDialog(this) == DialogResult.OK)
                {
                    LogActivity("RegistrationForm (Admin-initiated Add Mode) closed with OK.");
                    // Data refresh should be handled by events from AuthenticationService or MemberService if they trigger them upon saving.
                    // Explicitly calling LoadMembersData() might still be needed if events are not guaranteed or for immediate UI update.
                    LoadMembersData(); // Consider if this is redundant given events. For safety, keep it for now.
                }
                else
                {
                    LogActivity("RegistrationForm (Admin-initiated Add Mode) closed with Cancel or other.");
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
                    LogActivity($"Opening MemberEditForm for editing member ID: {selectedMember.Id}, Name: '{selectedMember.Name}'.");
                    using (MemberEditForm editForm = new MemberEditForm(selectedMember, _memberService, Logger))
                    {
                        if (editForm.ShowDialog(this) == DialogResult.OK)
                        {
                            LogActivity($"MemberEditForm (Edit Mode) for member ID: {selectedMember.Id} closed with OK. Data refresh handled by MembersChanged event.");
                        }
                        else
                        {
                            LogActivity($"MemberEditForm (Edit Mode) for member ID: {selectedMember.Id} closed with Cancel or other.");
                        }
                    }
                }
                else
                {
                    LogErrorActivity("Could not retrieve selected member data from DataGridView for editing.");
                    MessageBox.Show("無法取得選定的會員資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogActivity("Edit Member button clicked, but no member was selected or service not ready.");
                MessageBox.Show("請先選擇一位要編輯的會員。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteMember_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0 && _memberService != null)
            {
                Member? selectedMember = dgvMembers.SelectedRows[0].DataBoundItem as Member;
                if (selectedMember != null)
                {
                    LogActivity($"Attempting to delete member ID: {selectedMember.Id}, Name: '{selectedMember.Name}'.");

                    // Check for active rentals
                    if (_comicService != null)
                    {
                        bool hasActiveRentals = _comicService.GetAllComics().Any(c => c.IsRented && c.RentedToMemberId == selectedMember.Id);
                        if (hasActiveRentals)
                        {
                            MessageBox.Show($"會員 '{selectedMember.Name}' (ID: {selectedMember.Id}) 尚有未歸還的漫畫，無法刪除。", "刪除錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            LogActivity($"Attempt to delete member ID: {selectedMember.Id} failed: Member has active rentals.");
                            return; // Abort deletion
                        }
                    }
                    else
                    {
                        // This case should ideally not happen if constructor enforces non-null _comicService
                        MessageBox.Show("無法檢查會員租借狀態，漫畫服務未初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogErrorActivity("Could not check member rental status: _comicService is null.");
                        return; // Abort deletion
                    }

                    // If no active rentals, proceed with the existing confirmation dialog
                    LogActivity($"No active rentals for member ID: {selectedMember.Id}. Showing confirmation dialog for deletion.");
                    var confirmResult = MessageBox.Show($"您確定要刪除會員 '{selectedMember.Name}' (ID: {selectedMember.Id}) 嗎？\n此操作無法復原。",
                                                 "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirmResult == DialogResult.Yes)
                    {
                        LogActivity($"User confirmed deletion for member ID: {selectedMember.Id}.");
                        try
                        {
                            _memberService.DeleteMember(selectedMember.Id); // Assuming this is successful before deleting the user account
                            LogActivity($"Member ID: {selectedMember.Id}, Name: '{selectedMember.Name}' deleted from MemberService.");

                            // Now, delete the associated user account
                            // Use selectedMember.Username as the key for deletion
                            string usernameToDelete = selectedMember.Username; // Changed from selectedMember.Name
                            LogActivity($"Attempting to delete user account for Username: '{usernameToDelete}' (Member Name: '{selectedMember.Name}').");
                            if (_authenticationService != null) // Null check for safety, though it should be initialized
                            {
                                bool userDeleted = _authenticationService.DeleteUser(usernameToDelete);
                                if (userDeleted)
                                {
                                    LogActivity($"User account for Username: '{usernameToDelete}' (Member Name: '{selectedMember.Name}') successfully deleted by AuthenticationService.");
                                    MessageBox.Show($"會員 '{selectedMember.Name}' (使用者名稱: '{usernameToDelete}') 及其使用者帳戶已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    // This case might indicate an inconsistency if Username was expected to exist.

                                    Logger?.LogWarning($"User account for Username: '{usernameToDelete}' (Member Name: '{selectedMember.Name}') not found by AuthenticationService, though member record was deleted. Possible data inconsistency if a user account was expected.");

                                    MessageBox.Show($"會員 '{selectedMember.Name}' 已從會員列表中刪除，但對應的使用者帳戶 '{usernameToDelete}' 未找到或無法刪除。", "部分成功", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else
                            {
                                LogErrorActivity("AuthenticationService is null. Cannot delete user account.", new InvalidOperationException("_authenticationService is null"));
                                MessageBox.Show($"會員 '{selectedMember.Name}' 已從會員列表中刪除，但由於內部錯誤無法刪除其使用者帳戶。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (InvalidOperationException opEx)
                        {
                            LogErrorActivity($"Operation error deleting member ID: {selectedMember.Id} or associated user.", opEx);
                            MessageBox.Show(opEx.Message, "操作錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            LogErrorActivity($"Generic error deleting member ID: {selectedMember.Id}.", ex);
                            MessageBox.Show($"刪除會員時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        LogActivity($"User cancelled deletion for member ID: {selectedMember.Id}.");
                    }
                }
            }
            else
            {
                LogActivity("Delete Member button clicked, but no member was selected or service not ready.");
                MessageBox.Show("請先選擇一位要刪除的會員。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvMembers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LogActivity($"DataGridView (Members) cell double-clicked at row {e.RowIndex}. Triggering edit action.");
                btnEditMember_Click(sender, e);
            }
        }

        private void btnChangeUserRole_Click(object? sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member.", "No Member Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_authenticationService == null || Logger == null) // Logger comes from BaseForm
            {
                 MessageBox.Show("Required services not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 LogErrorActivity("ChangeUserRole button clicked but AuthenticationService or Logger is null.");
                 return;
            }

            Member? selectedMember = dgvMembers.SelectedRows[0].DataBoundItem as Member;
            if (selectedMember == null || string.IsNullOrEmpty(selectedMember.Username))
            {
                MessageBox.Show("Selected member has no associated username or data is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("Selected member for role change has no username or is invalid.");
                return;
            }

            User? userToEdit = _authenticationService.GetUserByUsername(selectedMember.Username);
            if (userToEdit == null)
            {
                MessageBox.Show($"User account for '{selectedMember.Username}' not found.", "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity($"User account for member '{selectedMember.Name}' (username: {selectedMember.Username}) not found for role change.");
                return;
            }

            LogActivity($"Opening ChangeUserRoleForm for member '{selectedMember.Name}', user '{userToEdit.Username}'.");
            using (ChangeUserRoleForm changeRoleForm = new ChangeUserRoleForm(userToEdit, _authenticationService, Logger))
            {
                changeRoleForm.ShowDialog(this);
                // No specific refresh needed here for dgvMembers as role is not displayed.
                // The user object in _authenticationService._users list is modified by reference,
                // and SaveUsers() in ChangeUserRoleForm persists this.
            }
        }
    }
}