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
        private readonly IComicService _comicService;
        private readonly MemberService _memberService;
        private readonly IReloadService _reloadService;

        public LoginForm(
            ILogger logger,
            AuthenticationService authService,
            IComicService comicService,
            MemberService memberService,
            IReloadService reloadService)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            _logger.Log("登入表單已初始化。");

            txtPassword.PasswordChar = '*';

            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
        }

        private void btnRegister_Click(object? sender, EventArgs e)

        {
            _logger.Log("註冊按鈕已點擊。");
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
                _logger.Log("登入嘗試失敗: 使用者名稱或密碼為空。");
                return;
            }

            _logger.Log($"使用者登入嘗試: {username}");
            User? user = _authService.Login(username, password);

            if (user != null)
            {
                _logger.Log($"使用者 '{username}' 成功登入。角色: {user.Role}。");
                this.Hide(); 
                MainForm mainForm = new MainForm(_logger, _comicService, _memberService, _reloadService, _authService, user);
                mainForm.FormClosed += (s, args) => this.Close(); 
                mainForm.Show();
            }
            else
            {
                _logger.Log($"使用者 '{username}' 登入失敗。無效的憑證。");
                MessageBox.Show("使用者名稱或密碼錯誤。", "登入失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Clear(); 
                txtPassword.Clear(); 
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            _logger.Log("登入表單已載入。");
        }
    }
}
