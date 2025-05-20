using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces; // 技術點3: 引用介面命名空間
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class MemberManagementForm : ComicRentalSystem_14Days.BaseForm // 技術點3: 繼承 BaseForm
    {
        private readonly MemberService _memberService;
        // Logger is inherited from BaseForm (protected ILogger? Logger)

        // 修改建構函式以接收 ILogger 並傳遞給 BaseForm
        public MemberManagementForm(ILogger logger) : base(logger) // 技術點4: 多型 (傳遞 ILogger 給 BaseForm)
        {
            InitializeComponent();
            LogActivity("MemberManagementForm initializing."); // 使用 BaseForm 的 LogActivity

            // 應該考慮是否也透過依賴注入傳入 FileHelper
            var fileHelper = new FileHelper();
            // 將 Logger 傳遞給 MemberService
            _memberService = new MemberService(fileHelper, Logger!); // 使用 BaseForm 的 Logger (確保非 null)

            // 訂閱 MemberService 的 MembersChanged 事件 (技術點 #5 委派/事件)
            _memberService.MembersChanged += MemberService_MembersChanged;

            SetupDataGridView();
            LoadMembersData();
            LogActivity("MemberManagementForm initialized successfully.");
        }

        private void MemberService_MembersChanged(object? sender, EventArgs e)
        {
            // 當 MemberService 中的資料變更時，這個方法會被呼叫
            LogActivity("MembersChanged event received from MemberService. Refreshing DataGridView.");
            LoadMembersData();
            // LogActivity("會員資料已更新，列表已刷新。"); // 已在 LoadMembersData 內部記錄
        }

        private void SetupDataGridView()
        {
            LogActivity("Setting up DataGridView columns for members.");
            dgvMembers.AutoGenerateColumns = false;
            dgvMembers.Columns.Clear();

            // 技術點 #7: 清單控制項 (DataGridView)
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
            LogActivity("Attempting to load members data into DataGridView.");
            try // 技術點 #5: 例外處理
            {
                List<Member> members = _memberService.GetAllMembers(); // Service 層已有日誌
                dgvMembers.DataSource = null;
                dgvMembers.DataSource = members;
                LogActivity($"Successfully loaded {members.Count} members into DataGridView.");
            }
            catch (Exception ex)
            {
                // 使用 BaseForm 的 LogErrorActivity
                LogErrorActivity("Error loading members data into DataGridView.", ex);
                MessageBox.Show($"載入會員資料時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 技術點 #6: 視窗應用程式的事件處理
        private void btnRefreshMembers_Click(object sender, EventArgs e) // Renamed from btnRefresh_Click
        {
            LogActivity("Refresh Members button clicked.");
            LoadMembersData();
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            LogActivity("Add Member button clicked. Opening MemberEditForm for new member.");
            // 將 Logger 傳遞給 MemberEditForm
            // 技術點7: 多表單視窗應用程式
            using (MemberEditForm editForm = new MemberEditForm(null, _memberService, Logger!)) // 傳遞 Logger
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
            LogActivity("Edit Member button clicked.");
            if (dgvMembers.SelectedRows.Count > 0)
            {
                Member? selectedMember = dgvMembers.SelectedRows[0].DataBoundItem as Member;

                if (selectedMember != null)
                {
                    LogActivity($"Opening MemberEditForm for editing member ID: {selectedMember.Id}, Name: '{selectedMember.Name}'.");
                    // 技術點7: 多表單視窗應用程式
                    using (MemberEditForm editForm = new MemberEditForm(selectedMember, _memberService, Logger!)) // 傳遞 Logger
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
                LogActivity("Edit Member button clicked, but no member was selected.");
                MessageBox.Show("請先選擇一位要編輯的會員。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteMember_Click(object sender, EventArgs e)
        {
            LogActivity("Delete Member button clicked.");
            if (dgvMembers.SelectedRows.Count > 0)
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
                        try // 技術點 #5: 例外處理
                        {
                            // 在Day 9-10實現租借邏輯後，這裡應該檢查會員是否有未歸還的漫畫
                            // bool canDelete = _rentalService.CanDeleteMember(selectedMember.Id); // 假設有 RentalService
                            // if (!canDelete)
                            // {
                            //    LogActivity($"Member ID: {selectedMember.Id} cannot be deleted due to open rentals.");
                            //    MessageBox.Show($"會員 '{selectedMember.Name}'尚有未歸還的漫畫，無法刪除。", "刪除失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    return;
                            // }

                            _memberService.DeleteMember(selectedMember.Id); // Service 層已有日誌
                            LogActivity($"Member ID: {selectedMember.Id} successfully marked for deletion by service. UI will refresh via event.");
                            MessageBox.Show("會員已刪除。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException opEx) // 特定業務邏輯錯誤 (例如：會員有未歸還書籍)
                        {
                            LogErrorActivity($"Operation error deleting member ID: {selectedMember.Id}.", opEx);
                            MessageBox.Show(opEx.Message, "操作錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex) // 一般例外
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
                else
                {
                    LogErrorActivity("Could not retrieve selected member data from DataGridView for deletion.");
                    MessageBox.Show("無法取得選定的會員資料以進行刪除。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogActivity("Delete Member button clicked, but no member was selected.");
                MessageBox.Show("請先選擇一位要刪除的會員。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvMembers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // 確保不是點擊標頭
            {
                LogActivity($"DataGridView (Members) cell double-clicked at row {e.RowIndex}. Triggering edit action.");
                btnEditMember_Click(sender, e); // 觸發編輯
            }
        }

        private void MemberManagementForm_Load(object sender, EventArgs e)
        {
            // LoadMembersData() 已在建構函式中呼叫
            LogActivity("MemberManagementForm finished loading.");
        }
    }
}