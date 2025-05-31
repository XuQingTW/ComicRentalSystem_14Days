using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class LoginForm : Form
    {
        private readonly ILogger _logger;
        private readonly AuthenticationService _authService;
        private readonly ComicService _comicService;
        private readonly MemberService _memberService;
        private readonly IReloadService _reloadService;

        public LoginForm(
            ILogger logger,
            AuthenticationService authService,
            ComicService comicService,
            MemberService memberService,
            IReloadService reloadService)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            _logger.Log("LoginForm initialized.");

            // Set password char
            txtPassword.PasswordChar = '*';

            // Attach event handler for btnRegister
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
        }

        private void btnRegister_Click(object? sender, EventArgs e)

        {
            _logger.Log("Register button clicked.");
            RegistrationForm regForm = new RegistrationForm(_logger, _authService, _memberService);
            regForm.ShowDialog(this);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("請輸入使用者名稱和密碼。", "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger.Log("Login attempt failed: Username or password empty.");
                return;
            }

            _logger.Log($"Login attempt for user: {username}");
            User? user = _authService.Login(username, password);

            if (user != null)
            {
                _logger.Log($"User '{username}' logged in successfully. Role: {user.Role}.");
                this.Hide(); // Hide login form
                MainForm mainForm = new MainForm(_logger, _comicService, _memberService, _reloadService, user);
                mainForm.FormClosed += (s, args) => this.Close(); // Close login form when main form closes
                mainForm.Show();
            }
            else
            {
                _logger.Log($"Login failed for user: {username}. Invalid credentials.");
                MessageBox.Show("使用者名稱或密碼錯誤。", "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Clear(); // Clear username field
                txtPassword.Clear(); // Clear password field
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            _logger.Log("LoginForm loaded.");
            // You can add any initialization logic here if needed when the form loads
        }
    }
}
