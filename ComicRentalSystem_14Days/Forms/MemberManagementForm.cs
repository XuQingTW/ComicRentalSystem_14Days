// In ComicRentalSystem_14Days/Forms/MemberManagementForm.cs

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
        private MemberService? _memberService; // 改為可為 Null

        // **新增**：為設計工具提供無參數的建構函式
        public MemberManagementForm()
        {
            InitializeComponent();
        }

        // 修改建構函式以接收 ILogger 並傳遞給 BaseForm
        public MemberManagementForm(ILogger logger) : base(logger)
        {
            InitializeComponent();
            LogActivity("MemberManagementForm initializing.");
        }

        // **修改**：將初始化和資料載入邏輯移至 Load 事件
        private void MemberManagementForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            // --- 以下為執行時期的程式碼 ---

            // 在設計模式下，Logger 會是 null，直接返回 (保留這個做為雙重保險)
            if (Logger == null) return;

            LogActivity("MemberManagementForm is loading runtime components.");
            var fileHelper = new FileHelper();
            _memberService = new MemberService(fileHelper, Logger);

            _memberService.MembersChanged += MemberService_MembersChanged;

            SetupDataGridView();
            LoadMembersData();
            LogActivity("MemberManagementForm initialized successfully.");
        }

        private void MemberService_MembersChanged(object? sender, EventArgs e)
        {
            if (_memberService == null) return;
            LogActivity("MembersChanged event received. Refreshing DataGridView.");
            LoadMembersData();
        }

        private void SetupDataGridView()
        {
            // ... (此處程式碼不變) ...
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

        // ... (btnAdd, btnEdit, btnDelete, btnRefresh 等事件處理方法維持不變) ...

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
                            _memberService.DeleteMember(selectedMember.Id);
                            LogActivity($"Member ID: {selectedMember.Id} successfully marked for deletion by service. UI will refresh via event.");
                            MessageBox.Show("會員已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException opEx)
                        {
                            LogErrorActivity($"Operation error deleting member ID: {selectedMember.Id}.", opEx);
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