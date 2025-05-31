using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class RegistrationForm : Form
    {
        private readonly ILogger _logger;
        private readonly AuthenticationService _authService;
        private readonly MemberService _memberService;

        public RegistrationForm(ILogger logger, AuthenticationService authService, MemberService memberService)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));

            txtPassword.PasswordChar = '*';
            txtConfirmPassword.PasswordChar = '*';
            _logger.Log("RegistrationForm initialized.");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string name = txtName.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            UserRole selectedRole = UserRole.Member; // Default role

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("使用者名稱不能為空。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Username empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("姓名不能為空。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Name empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("電話號碼不能為空。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Phone number empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("密碼不能為空。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Password empty.");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("密碼長度至少需6個字元。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Password too short.");
                txtPassword.Clear();
                txtConfirmPassword.Clear();
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("密碼與確認密碼不相符。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Passwords do not match.");
                txtPassword.Clear();
                txtConfirmPassword.Clear();
                return;
            }

            _logger.Log($"Registration attempt for user: {username}, Name: {name}, Phone: {phoneNumber}, Role: {selectedRole}");
            bool success = _authService.Register(username, password, selectedRole);

            if (success)
            {
                _logger.Log($"User '{username}' (from txtUsername) registered successfully as {selectedRole}.");
                try
                {
                    // Create Member object, ensuring Username is populated from txtUsername.Text
                    // Name (for display/other purposes) is from txtName.Text
                    Member newMember = new Member { Name = name, PhoneNumber = phoneNumber, Username = username };
                    _memberService.AddMember(newMember);
                    _logger.Log($"Member record created for Name: {name}, Username: {username}, Phone: {phoneNumber}");

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
                    _logger.LogError($"Successfully registered user '{username}' but failed to create member record. Error: {ex.Message}", ex);
                    MessageBox.Show($"使用者 '{username}' 註冊成功，但建立會員資料時發生錯誤。請聯繫管理員。\n錯誤: {ex.Message}", "註冊部分成功", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // User is registered, but member creation failed. Decide on cleanup or leave as is.
                    // For now, clear fields as user registration part was successful.
                    txtUsername.Clear();
                    txtName.Clear();
                    txtPhoneNumber.Clear();
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                }
            }
            else
            {
                _logger.Log($"Registration failed for user: {username}. Username might already exist.");
                MessageBox.Show($"註冊失敗。使用者名稱 '{username}' 可能已被使用。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Clear username only if it's a duplicate issue, others can remain for correction
                txtUsername.Clear();
            }
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            _logger.Log("RegistrationForm loaded.");
        }
    }
}
