using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComicRentalSystem_14Days;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Interfaces;

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm // 讓 MainForm 也繼承 BaseForm
    {
        private readonly ILogger _logger; // 技術點3: 介面

        // 修改建構函式以接收 ILogger
        public MainForm(ILogger logger) // 技術點4: 多型 (可以傳入任何實作 ILogger 的物件)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            // 現在 MainForm 可以使用 _logger 來記錄日誌
            _logger.Log("MainForm initialized."); // 技術點3: 透過介面呼叫 Log()
        }

        // 移除 MainForm 中的 LogActivity，因為 BaseForm 已經有了，並且我們現在使用 ILogger

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.Log("Exit menu item clicked. Application will exit.");
            Application.Exit();
        }

        private void 漫畫管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.Log("Opening ComicManagementForm.");
            // 將 logger 傳遞給 ComicManagementForm
            ComicManagementForm comicMgmtForm = new ComicManagementForm(_logger);
            comicMgmtForm.ShowDialog(this);
            // LogActivity("開啟漫畫管理表單。"); // 改用 _logger
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.Log("Opening MemberManagementForm.");
            // 將 logger 傳遞給 MemberManagementForm
            MemberManagementForm memberMgmtForm = new MemberManagementForm(_logger);
            memberMgmtForm.ShowDialog(this);
            // LogActivity("開啟會員管理表單。"); // 改用 _logger
        }

        // 如果 MainForm 需要直接使用 LogActivity, 它可以呼叫 BaseForm 的版本
        // 或者更好的方式是 BaseForm 也依賴 ILogger
    }
}
