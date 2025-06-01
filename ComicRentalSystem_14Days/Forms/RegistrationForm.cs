using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RegistrationForm : BaseForm // Changed inheritance
    {
        private readonly ILogger _logger; // Will be set by base.SetLogger
        private readonly AuthenticationService _authService;
        private readonly MemberService _memberService;
        private readonly User? _currentUser;
        private System.Windows.Forms.ErrorProvider errorProvider1;

        public RegistrationForm(ILogger logger, AuthenticationService authService, MemberService memberService, User? currentUser = null)
        {
            InitializeComponent(); // BaseForm constructor (if any) is called before this if : base() is used.
                                   // If BaseForm has parameterless, it's implicitly called.
                                   // ModernBaseForm has parameterless, BaseForm has parameterless.

            this.errorProvider1 = new System.Windows.Forms.ErrorProvider();

            // Set logger for BaseForm functionality
            // _logger field in this class is shadowed if BaseForm also has _logger. Best to use BaseForm's logger.
            // For now, we assume this class's _logger is the primary one for its direct logic,
            // and BaseForm's logger is set for its own needs.
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            base.SetLogger(this._logger); // Make sure BaseForm has its logger instance

            // Apply Modern Styling
            if (btnRegister != null) StyleModernButton(btnRegister);
            Control[] foundControls = this.Controls.Find("gbAccountCredentials", true);
            if (foundControls.Length > 0 && foundControls[0] is GroupBox gbAcc) StyleModernGroupBox(gbAcc);

            foundControls = this.Controls.Find("gbMemberInfo", true);
            if (foundControls.Length > 0 && foundControls[0] is GroupBox gbInfo) StyleModernGroupBox(gbInfo);
            // if (this.gbAccountCredentials != null) StyleModernGroupBox(this.gbAccountCredentials); // If direct field access
            // if (this.gbMemberInfo != null) StyleModernGroupBox(this.gbMemberInfo);


            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _currentUser = currentUser;

            txtPassword.PasswordChar = '*';
            txtConfirmPassword.PasswordChar = '*';
            _logger.Log("註冊表單已初始化。");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren()) {
                MessageBox.Show("Please correct the highlighted errors.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed due to validation errors.");
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string name = txtName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            // UserRole selectedRole = UserRole.Member; // Default role - Will be determined below

            UserRole selectedRole;
            if (cmbRole.Visible && cmbRole.Enabled && cmbRole.SelectedItem != null)
            {
                selectedRole = (UserRole)cmbRole.SelectedItem;
            }
            else
            {
                selectedRole = UserRole.Member; // Default if ComboBox not used or no selection
            }

            // Removed manual validation checks, now handled by Validating events + ValidateChildren()

            _logger.Log($"使用者註冊嘗試: {username}, 姓名: {name}, 電話: {phoneNumber}, 角色: {selectedRole}");
            bool success = _authService.Register(username, password, selectedRole);

            if (success)
            {
                _logger.Log($"使用者 '{username}' (來自txtUsername) 已成功註冊為 {selectedRole}。");
                try
                {
                    // Create Member object, ensuring Username is populated from txtUsername.Text
                    // Name (for display/other purposes) is from txtName.Text
                    Member newMember = new Member { Name = name, PhoneNumber = phoneNumber, Username = username };
                    _memberService.AddMember(newMember);
                    _logger.Log($"已為姓名: {name}, 使用者名稱: {username}, 電話: {phoneNumber} 建立會員記錄。");

                    MessageBox.Show($"使用者 '{username}' (姓名: {name}) 已成功註冊，會員資料也已建立。", "註冊成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Optionally close the form or clear fields
                    txtUsername.Clear();
                    txtName.Clear();
                    txtPhoneNumber.Clear();
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"成功註冊使用者 '{username}' 但建立會員記錄失敗。錯誤: {ex.Message}. 正在嘗試復原使用者註冊。", ex);
                    // Attempt to delete the orphaned user
                    bool rollbackSuccess = _authService.DeleteUser(username);
                    if (rollbackSuccess)
                    {
                        _logger.Log($"已成功復原 (刪除) 使用者帳戶 '{username}'。");
                    }
                    else
                    {
                        _logger.LogError($"無法復原 (刪除) 使用者帳戶 '{username}'。系統可能處於不一致狀態。");
                    }
                    MessageBox.Show($"註冊過程中發生錯誤，無法建立完整的會員資料。請重試或聯繫管理員。\n錯誤: {ex.Message}", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Do not clear all fields, allow user to correct if possible, but clear sensitive ones
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                }
            }
            else
            {
                _logger.Log($"使用者 '{username}' 註冊失敗。使用者名稱可能已存在。");
                MessageBox.Show("無法完成註冊。請確認所有欄位均已正確填寫，或嘗試使用不同的使用者名稱。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Clear username only if it's a duplicate issue, others can remain for correction
                txtUsername.Clear();
            }
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            _logger.Log("註冊表單已載入。");
            // Populate cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
            // The following line assumes cmbRole is declared in the designer file.
            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));

            if (_currentUser != null && _currentUser.Role == UserRole.Admin)
            {
                // Admin is using the form
                lblRole.Visible = true;
                cmbRole.Visible = true;
                cmbRole.Enabled = true;
                cmbRole.SelectedItem = UserRole.Member; // Default selection for Admin
                _logger.Log("註冊表單由管理員載入。角色選擇已啟用。");
            }
            else
            {
                // Non-admin or null currentUser (e.g., from LoginForm)
                lblRole.Visible = false;
                cmbRole.Visible = false;
                cmbRole.Enabled = false;
                _logger.Log("註冊表單由非管理員或透過登入表單載入。角色選擇已停用。");
            }
        }

        // Validating Event Handlers
        private void txtUsername_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            { errorProvider1?.SetError(txt, "Username cannot be empty."); e.Cancel = true; }
            else if (sender is TextBox txtBox) { errorProvider1?.SetError(txtBox, ""); }
        }

        private void txtPassword_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            { errorProvider1?.SetError(txt, "Password cannot be empty."); e.Cancel = true; }
            else if (sender is TextBox txtP && txtP.Text.Length < 6)
            { errorProvider1?.SetError(txtP, "Password must be at least 6 characters long."); e.Cancel = true; }
            else if (sender is TextBox txtBox) { errorProvider1?.SetError(txtBox, ""); }
        }

        private void txtConfirmPassword_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && txt.Text != txtPassword.Text) // Assumes txtPassword is accessible
            { errorProvider1?.SetError(txt, "Passwords do not match."); e.Cancel = true; }
            else if (sender is TextBox txtB) { errorProvider1?.SetError(txtB, ""); }
        }

        private void txtName_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                { errorProvider1?.SetError(txt, "Name cannot be empty."); e.Cancel = true; }
                else if (!txt.Text.Trim().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                { errorProvider1?.SetError(txt, "Name can only contain letters and spaces."); e.Cancel = true; }
                else { errorProvider1?.SetError(txt, ""); }
            }
        }

        private void txtPhoneNumber_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt)
            {
                string phoneNumber = txt.Text.Trim();
                if (string.IsNullOrWhiteSpace(phoneNumber))
                { errorProvider1?.SetError(txt, "Phone number cannot be empty."); e.Cancel = true; }
                else if (!phoneNumber.All(char.IsDigit))
                { errorProvider1?.SetError(txt, "Phone number can only contain digits."); e.Cancel = true; }
                else if (phoneNumber.Length < 7 || phoneNumber.Length > 15)
                { errorProvider1?.SetError(txt, "Phone number must be between 7 and 15 digits."); e.Cancel = true; }
                else { errorProvider1?.SetError(txt, ""); }
            }
        }
    }
}
