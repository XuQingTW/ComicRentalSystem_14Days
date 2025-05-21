// ComicRentalSystem_14Days/Forms/MemberEditForm.cs
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;
// using System.ComponentModel; // 如果使用 this.DesignMode，則不需要特別為 LicenseManager 引入
using System.Text.RegularExpressions;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class MemberEditForm : ComicRentalSystem_14Days.BaseForm
    {
        // 將欄位宣告為可為 Null
        private readonly MemberService? _memberService;
        private Member? _editableMember;
        private bool _isEditMode;

        public MemberEditForm() : base()
        {
            InitializeComponent();
            if (this.DesignMode) // 使用 this.DesignMode
            {
                // txtName.Text = "設計模式姓名";
                // btnSaveMember.Enabled = false;
            }
        }

        public MemberEditForm(Member? memberToEdit, MemberService memberService, ILogger logger) : this()
        {
            base.SetLogger(logger);

            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _editableMember = memberToEdit;
            _isEditMode = (_editableMember != null);

            LogActivity($"MemberEditForm initializing. Mode: {(_isEditMode ? "Edit" : "Add")}" +
                        (_isEditMode && _editableMember != null ? $", MemberID: {_editableMember.Id}" : ""));

            if (_isEditMode && _editableMember != null)
            {
                this.Text = "編輯會員";
                LoadMemberData();
            }
            else
            {
                this.Text = "新增會員";
            }
            LogActivity("MemberEditForm initialized with services.");
        }

        private void LoadMemberData()
        {
            if (_editableMember == null)
            {
                LogActivity("LoadMemberData called but _editableMember is null (should not happen in edit mode).");
                return;
            }

            LogActivity($"Loading data for member ID: {_editableMember.Id}, Name: '{_editableMember.Name}'.");
            txtName.Text = _editableMember.Name;
            txtPhoneNumber.Text = _editableMember.PhoneNumber;
            LogActivity("Member data loaded into form controls.");
        }

        private void btnSaveMember_Click(object sender, EventArgs e)
        {
            if (_memberService == null)
            {
                MessageBox.Show("服務未初始化，無法儲存。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("Save Member button clicked, but _memberService is null.");
                return;
            }

            LogActivity("Save Member button clicked.");

            // ... (輸入驗證) ...

            try
            {
                if (_isEditMode && _editableMember != null)
                {
                    LogActivity($"Attempting to save changes for existing member ID: {_editableMember.Id}.");
                    _editableMember.Name = txtName.Text.Trim();
                    _editableMember.PhoneNumber = txtPhoneNumber.Text.Trim();
                    _memberService.UpdateMember(_editableMember);
                    LogActivity($"Member ID: {_editableMember.Id} updated successfully.");
                    MessageBox.Show("會員資料已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LogActivity("Attempting to add new member.");
                    Member newMember = new Member
                    {
                        Name = txtName.Text.Trim(),
                        PhoneNumber = txtPhoneNumber.Text.Trim()
                    };
                    _memberService.AddMember(newMember);
                    LogActivity($"New member '{newMember.Name}' (ID: {newMember.Id}) added successfully.");
                    MessageBox.Show("會員已成功新增。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                LogActivity("MemberEditForm closing with DialogResult.OK.");
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error while saving member: {ex.Message}", ex);
                MessageBox.Show($"儲存會員時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelMember_Click(object sender, EventArgs e)
        {
            LogActivity("Cancel Member button clicked. MemberEditForm closing with DialogResult.Cancel.");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void MemberEditForm_Load(object sender, EventArgs e)
        {
            LogActivity("MemberEditForm finished loading.");
        }
    }
}