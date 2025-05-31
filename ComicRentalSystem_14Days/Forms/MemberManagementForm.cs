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

        public MemberManagementForm()
        {
            InitializeComponent();
        }

        // Updated constructor to include AuthenticationService
        public MemberManagementForm(ILogger logger, MemberService memberService, AuthenticationService authenticationService) : base(logger)
        {
            InitializeComponent();
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            LogActivity("MemberManagementForm initializing with MemberService and AuthenticationService.");
        }

        private void MemberManagementForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || Logger == null || _memberService == null || _authenticationService == null) // Added null check for _authenticationService
            {
                return;
            }

            LogActivity("MemberManagementForm is loading runtime components.");

            _memberService.MembersChanged += MemberService_MembersChanged; // _memberService is confirmed not null here

            SetupDataGridView();
            LoadMembersData();
            LogActivity("MemberManagementForm initialized successfully.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogActivity($"MemberManagementForm closing. User: {CurrentUser?.Username ?? "N/A"}");
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
            LogActivity("Attempting to load members data into DataGridView.");
            try
            {
                List<Member> members = _memberService.GetAllMembers();
                dgvMembers.DataSource = null;
                dgvMembers.DataSource = members;
                LogActivity($"Successfully loaded {members.Count} members into DataGridView.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading members data into DataGridView.", ex);
                MessageBox.Show($"載入會員資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefreshMembers_Click(object sender, EventArgs e)
        {
            LogActivity("Refresh Members button clicked.");
            LoadMembersData();
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            if (_memberService == null || Logger == null) return;
            LogActivity("Add Member button clicked. Opening MemberEditForm for new member.");
            using (MemberEditForm editForm = new MemberEditForm(null, _memberService, Logger))
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    LogActivity("MemberEditForm (Add Mode) closed with OK. Data refresh handled by MembersChanged event.");
                }
                else
                {
                    LogActivity("MemberEditForm (Add Mode) closed with Cancel or other.");
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
                    LogActivity($"Attempting to delete member ID: {selectedMember.Id}, Name: '{selectedMember.Name}'. Showing confirmation dialog.");
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
    }
}