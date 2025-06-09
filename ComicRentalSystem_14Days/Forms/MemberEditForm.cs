using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class MemberEditForm : ComicRentalSystem_14Days.BaseForm
    {
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
            base.SetLogger(logger);

            if (btnSaveMember != null) StyleModernButton(btnSaveMember);
            if (btnCancelMember != null) StyleSecondaryButton(btnCancelMember);

            Control[] foundControls = this.Controls.Find("gbMemberDetails", true);
            if (foundControls.Length > 0 && foundControls[0] is GroupBox gb)
            {
                StyleModernGroupBox(gb);
            }


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

        private async Task btnSaveMember_Click_Async(object sender, EventArgs e)
        {
            if (_memberService == null)
            {
                MessageBox.Show("服務未初始化，無法儲存。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("btnSaveMember_Click_Async: Save button clicked, but _memberService is null.");
                return;
            }

            LogActivity("btnSaveMember_Click_Async: Save button clicked.");

            if (!this.ValidateChildren())
            {
                MessageBox.Show("請修正標示的錯誤。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogActivity("btnSaveMember_Click_Async: MemberEdit validation failed. Please correct highlighted errors.");
                return;
            }

            string name = txtName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();

            try
            {
                if (_isEditMode && _editableMember != null)
                {
                    LogActivity($"btnSaveMember_Click_Async: Attempting to save changes for existing member ID: {_editableMember.Id}.");
                    _editableMember.Name = name;
                    _editableMember.PhoneNumber = phoneNumber;
                    await _memberService.UpdateMemberAsync(_editableMember);

                    if (!this.IsHandleCreated || this.IsDisposed)
                    {
                        LogErrorActivity($"btnSaveMember_Click_Async: Form closed or disposed after UpdateMemberAsync for ID: {_editableMember.Id}.");
                        return;
                    }
                    LogActivity($"btnSaveMember_Click_Async: Member ID: {_editableMember.Id} successfully updated.");
                    MessageBox.Show("會員資料已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Add mode
                {
                    LogActivity("btnSaveMember_Click_Async: Attempting to add a new member.");
                    Member newMember = new Member
                    {
                        Name = name,
                        PhoneNumber = phoneNumber
                    };
                    await _memberService.AddMemberAsync(newMember);

                    if (!this.IsHandleCreated || this.IsDisposed)
                    {
                        LogErrorActivity($"btnSaveMember_Click_Async: Form closed or disposed after AddMemberAsync for new member '{newMember.Name}'.");
                        return;
                    }
                    LogActivity($"btnSaveMember_Click_Async: New member '{newMember.Name}' (ID: {newMember.Id}) successfully added.");
                    MessageBox.Show("會員已成功新增。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                LogActivity("btnSaveMember_Click_Async: MemberEditForm closing with DialogResult.OK.");
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"btnSaveMember_Click_Async: Error saving member: {ex.Message}", ex);
                if (this.IsHandleCreated && !this.IsDisposed) // Check before showing MessageBox
                    MessageBox.Show($"儲存會員時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSaveMember_Click(object sender, EventArgs e)
        {
            await btnSaveMember_Click_Async(sender, e);
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

        private void txtName_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    errorProvider1?.SetError(txt, "姓名不能為空。");
                    e.Cancel = true;
                }
                else if (!txt.Text.Trim().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    errorProvider1?.SetError(txt, "姓名只能包含字母和空格。");
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1?.SetError(txt, "");
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
                    errorProvider1?.SetError(txt, "電話號碼不能為空。");
                    e.Cancel = true;
                }
                else if (!phoneNumber.All(char.IsDigit))
                {
                    errorProvider1?.SetError(txt, "電話號碼只能包含數字。");
                    e.Cancel = true;
                }
                else if (phoneNumber.Length < 7 || phoneNumber.Length > 15)
                {
                    errorProvider1?.SetError(txt, "電話號碼必須介於7到15位數字之間。");
                }
                else
                {
                    errorProvider1?.SetError(txt, "");
                }
            }
        }
    }
}