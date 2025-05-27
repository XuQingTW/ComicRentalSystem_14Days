// ComicRentalSystem_14Days/MainForm.cs
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Services;
using System;
using System.Windows.Forms;
// using System.ComponentModel; // 如果使用 this.DesignMode，則不需要特別為 LicenseManager 引入

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        // 將欄位宣告為可為 Null
        private readonly ILogger? _logger;
        private readonly IReloadService? _reloadService;

        public MainForm() : base()
        {
            InitializeComponent();
            if (this.DesignMode) // 使用 this.DesignMode
            {
                // this.Text = "MainForm (設計模式)";
            }
        }

        public MainForm(ILogger logger) : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            base.SetLogger(logger); // 將 logger 實例傳遞給 BaseForm
            _reloadService = new ReloadService();
            _logger.Log("MainForm initialized with logger.");
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Exit menu item clicked. Application will exit.");
            Application.Exit();
        }

        private void 漫畫管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening ComicManagementForm.");
            // 確保 _logger 不是 null 才傳遞，或者 ComicManagementForm 能處理 null logger
            if (_logger != null)
            {
                ComicManagementForm comicMgmtForm = new ComicManagementForm(_logger);
                comicMgmtForm.ShowDialog(this);
            }
            else
            {
                // 處理 logger 為 null 的情況，例如顯示錯誤或使用備用 logger
                MessageBox.Show("Logger 未初始化，無法開啟漫畫管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening MemberManagementForm.");
            if (_logger != null)
            {
                MemberManagementForm memberMgmtForm = new MemberManagementForm(_logger);
                memberMgmtForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Logger 未初始化，無法開啟會員管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rentalManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening RentalForm."); // Assuming _logger is available in MainForm
            if (_logger == null)
            {
                MessageBox.Show("Logger is not available. Cannot open Rental Form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // It's good practice to use the application-wide logger instance if available
                // Program.AppLogger can be used if it's public static, or pass the _logger from MainForm
                ILogger logger = _logger; // Or Program.AppLogger;
                IReloadService reloadService = _reloadService;

                FileHelper fileHelper = new FileHelper();
                ComicService comicService = new ComicService(fileHelper, logger);
                MemberService memberService = new MemberService(fileHelper, logger);

                RentalForm rentalForm = new RentalForm(comicService, memberService, logger,reloadService);
                rentalForm.ShowDialog(this); // ShowDialog makes it a modal dialog
            }
            catch (Exception ex)
            {
                _logger?.LogError("Failed to open RentalForm.", ex);
                MessageBox.Show($"Error opening rental form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}