using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq; // 為 All() 方法加入

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
            base.SetLogger(logger); // 確保為 BaseForm 的方法 (如 StyleModern...) 設定了記錄器

            // InitializeComponent 執行後 (透過 :this() 呼叫) 套用現代樣式
            if (btnSaveMember != null) StyleModernButton(btnSaveMember);
            if (btnCancelMember != null) StyleSecondaryButton(btnCancelMember);

            // 設定 GroupBox 樣式 - 如果直接欄位存取有問題，則使用 Controls.Find 作為安全方法
            // 在典型的 WinForms 設定中，this.gbMemberDetails 可以直接存取。
            Control[] foundControls = this.Controls.Find("gbMemberDetails", true);
            if (foundControls.Length > 0 && foundControls[0] is GroupBox gb)
            {
                StyleModernGroupBox(gb);
            }
            // else if (this.gbMemberDetails != null) // 如果設計工具將其設為直接欄位，則為備用方案
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
                 MessageBox.Show("請修正標示的錯誤。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning); // "Please correct the highlighted errors." | "Validation Error"
                 LogActivity("會員編輯驗證失敗。請修正醒目提示的錯誤。");
                 return;
            }

            string name = txtName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();

            // 特定邏輯 (例如唯一性或其他業務規則) 如有需要可以保留，
            // 但基本的欄位存在/格式檢查現在由 Validating 事件處理。

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
                    // 如果 RegistrationForm 用於所有新會員的新增，則此區塊較不可能被執行。
                    // 然而，為確保 MemberEditForm 可直接用於新增非使用者會員，故保留此邏輯的完整性。
                    LogActivity("正在嘗試新增會員。");
                    Member newMember = new Member
                    {
                        Name = name,
                        PhoneNumber = phoneNumber
                        // 如果此會員同時也是使用者，通常會設定使用者名稱，
                        // 這部分由 RegistrationForm 處理。
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

        // 驗證事件處理常式
        private void txtName_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    errorProvider1?.SetError(txt, "姓名不能為空。"); // 姓名不能為空。
                    e.Cancel = true;
                }
                else if (!txt.Text.Trim().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    errorProvider1?.SetError(txt, "姓名只能包含字母和空格。"); // 姓名只能包含字母和空格。
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1?.SetError(txt, ""); // 清除錯誤
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
                    errorProvider1?.SetError(txt, "電話號碼不能為空。"); // 電話號碼不能為空。
                    e.Cancel = true;
                }
                else if (!phoneNumber.All(char.IsDigit))
                {
                    errorProvider1?.SetError(txt, "電話號碼只能包含數字。"); // 電話號碼只能包含數字。
                    e.Cancel = true;
                }
                else if (phoneNumber.Length < 7 || phoneNumber.Length > 15)
                {
                    errorProvider1?.SetError(txt, "電話號碼必須介於7到15位數字之間。"); // 電話號碼必須介於7到15位數字之間。
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1?.SetError(txt, ""); // 清除錯誤
                }
            }
        }
    }
}