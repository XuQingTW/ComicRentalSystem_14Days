// ComicRentalSystem_14Days/MainForm.cs
using System;
using System.Windows.Forms;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Interfaces;
// using System.ComponentModel; // 如果使用 this.DesignMode，則不需要特別為 LicenseManager 引入

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        // 將欄位宣告為可為 Null
        private readonly ILogger? _logger;

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
    }
}