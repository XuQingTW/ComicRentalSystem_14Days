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

        public RegistrationForm(ILogger logger, AuthenticationService authService)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            // Populate role ComboBox
            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = UserRole.Member; // Default to Member

            txtPassword.PasswordChar = '*';
            txtConfirmPassword.PasswordChar = '*';
            _logger.Log("RegistrationForm initialized.");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            UserRole selectedRole = (UserRole)(cmbRole.SelectedItem ?? UserRole.Member);

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("使用者名稱不能為空。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Username empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("密碼不能為空。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Registration attempt failed: Password empty.");
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

            _logger.Log($"Registration attempt for user: {username}, Role: {selectedRole}");
            bool success = _authService.Register(username, password, selectedRole);

            if (success)
            {
                _logger.Log($"User '{username}' registered successfully as {selectedRole}.");
                MessageBox.Show($"使用者 '{username}' 已成功註冊為 {selectedRole}。", "註冊成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Optionally close the form or clear fields
                txtUsername.Clear();
                txtPassword.Clear();
                txtConfirmPassword.Clear();
                cmbRole.SelectedItem = UserRole.Member;
            }
            else
            {
                _logger.Log($"Registration failed for user: {username}. Username might already exist.");
                MessageBox.Show($"註冊失敗。使用者名稱 '{username}' 可能已被使用。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            _logger.Log("RegistrationForm loaded.");
        }
    }
}
