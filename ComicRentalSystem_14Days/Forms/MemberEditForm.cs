using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq; // Added for All() method

namespace ComicRentalSystem_14Days.Forms
{
    public partial class MemberEditForm : ComicRentalSystem_14Days.BaseForm
    {
        // 將欄位宣告為可為 Null
        private readonly MemberService? _memberService;
        private Member? _editableMember;
        private bool _isEditMode;
        private System.Windows.Forms.ErrorProvider errorProvider1;

        public MemberEditForm() : base()
        {
            InitializeComponent();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
            if (this.DesignMode)
            {
            }
        }

        public MemberEditForm(Member? memberToEdit, MemberService memberService, ILogger logger) : this()
        {
            base.SetLogger(logger); // Ensure logger is set for BaseForm methods like StyleModern...

            // Apply Modern Styling after InitializeComponent has run (called from :this())
            if (btnSaveMember != null) StyleModernButton(btnSaveMember);
            if (btnCancelMember != null) StyleSecondaryButton(btnCancelMember);

            // Style GroupBox - using Controls.Find as a safe way if direct field access is an issue
            // In a typical WinForms setup, this.gbMemberDetails would be directly accessible.
            Control[] foundControls = this.Controls.Find("gbMemberDetails", true);
            if (foundControls.Length > 0 && foundControls[0] is GroupBox gb)
            {
                StyleModernGroupBox(gb);
            }
            // else if (this.gbMemberDetails != null) // Fallback if designer made it a direct field
            // {
            // StyleModernGroupBox(this.gbMemberDetails);
            // }


            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _editableMember = memberToEdit;
            _isEditMode = (_editableMember != null);

            LogActivity($"會員編輯表單初始化中。模式: {(_isEditMode ? "編輯" : "新增")}" +
                        (_isEditMode && _editableMember != null ? $", 會員ID: {_editableMember.Id}" : ""));

            if (_isEditMode && _editableMember != null)
            {
                this.Text = "編輯會員";
                LoadMemberData();
            }
            else
            {
                this.Text = "新增會員";
            }
            LogActivity("會員編輯表單已使用服務初始化。");
        }

        private void LoadMemberData()
        {
            if (_editableMember == null)
            {
                LogActivity("LoadMemberData 已呼叫，但 _editableMember 為空 (在編輯模式下不應發生)。");
                return;
            }

            LogActivity($"正在載入會員ID: {_editableMember.Id}, 姓名: '{_editableMember.Name}' 的資料。");
            txtName.Text = _editableMember.Name;
            txtPhoneNumber.Text = _editableMember.PhoneNumber;
            LogActivity("會員資料已載入表單控制項。");
        }

        private void btnSaveMember_Click(object sender, EventArgs e)
        {
            if (_memberService == null)
            {
                MessageBox.Show("服務未初始化，無法儲存。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("儲存會員按鈕已點擊，但 _memberService 為空。");
                return;
            }

            LogActivity("儲存會員按鈕已點擊。");

            if (!this.ValidateChildren()) {
                 MessageBox.Show("Please correct the highlighted errors.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 LogActivity("Validation failed for member edit. Highlighted errors present.");
                 return;
            }

            string name = txtName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();

            // Specific logic for uniqueness or other business rules can remain if needed,
            // but basic presence/format checks are now handled by Validating events.

            try
            {
                if (_isEditMode && _editableMember != null)
                {
                    LogActivity($"正在嘗試儲存現有會員ID: {_editableMember.Id} 的變更。");
                    _editableMember.Name = name;
                    _editableMember.PhoneNumber = phoneNumber;
                    _memberService.UpdateMember(_editableMember);
                    LogActivity($"會員ID: {_editableMember.Id} 已成功更新。");
                    MessageBox.Show("會員資料已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // This block is less likely to be hit if RegistrationForm is used for all new member additions.
                    // However, keeping the logic sound in case MemberEditForm is used for adding non-user members directly.
                    LogActivity("正在嘗試新增會員。");
                    Member newMember = new Member
                    {
                        Name = name,
                        PhoneNumber = phoneNumber
                        // Username would typically be set if this member was also a User,
                        // which is handled by RegistrationForm.
                    };
                    _memberService.AddMember(newMember);
                    LogActivity($"新會員 '{newMember.Name}' (ID: {newMember.Id}) 已成功新增。");
                    MessageBox.Show("會員已成功新增。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                LogActivity("會員編輯表單正在以 DialogResult.OK 關閉。");
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"儲存會員時發生錯誤: {ex.Message}", ex);
                MessageBox.Show($"儲存會員時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelMember_Click(object sender, EventArgs e)
        {
            LogActivity("取消會員按鈕已點擊。會員編輯表單正在以 DialogResult.Cancel 關閉。");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void MemberEditForm_Load(object sender, EventArgs e)
        {
            LogActivity("會員編輯表單已完成載入。");
        }

        // Validating Event Handlers
        private void txtName_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    errorProvider1?.SetError(txt, "Name cannot be empty.");
                    e.Cancel = true;
                }
                else if (!txt.Text.Trim().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    errorProvider1?.SetError(txt, "Name can only contain letters and spaces.");
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1?.SetError(txt, ""); // Clear error
                }
            }
        }

        private void txtPhoneNumber_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt)
            {
                string phoneNumber = txt.Text.Trim();
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    errorProvider1?.SetError(txt, "Phone number cannot be empty.");
                    e.Cancel = true;
                }
                else if (!phoneNumber.All(char.IsDigit))
                {
                    errorProvider1?.SetError(txt, "Phone number can only contain digits.");
                    e.Cancel = true;
                }
                else if (phoneNumber.Length < 7 || phoneNumber.Length > 15)
                {
                    errorProvider1?.SetError(txt, "Phone number must be between 7 and 15 digits.");
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1?.SetError(txt, ""); // Clear error
                }
            }
        }
    }
}