
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days
{
    partial class MainForm
    {
        private IContainer components = null;

        #region Designer fields

        private Panel leftNavPanel;
        private Panel mainContentPanel;

        private Button btnNavDashboard;
        private Button btnNavComicMgmt;
        private Button btnNavMemberMgmt;
        private Button btnNavRentalMgmt;
        private Button btnNavUserReg;
        private Button btnNavLogs;

        private Panel pnlAdminComicMgmt;          
        private Label lblAvailableComics;         
        private ComboBox cmbAdminComicFilterStatus; 
        private DataGridView dgvAvailableComics;   

        private TabControl memberViewTabControl;
        private TabPage availableComicsTabPage;
        private TabPage myRentalsTabPage;

        private Panel availableComicsFilterPanel;
        private TextBox txtSearchAvailableComics;
        private ComboBox cmbGenreFilter;

        private Label lblMyRentedComicsHeader;
        private DataGridView dgvMyRentedComics;

        private Label lblAvailableLegend;
        private Label lblMyRentalsLegend;

        private MenuStrip menuStrip1;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem 檔案ToolStripMenuItem;
        private ToolStripMenuItem 離開ToolStripMenuItem;
        private ToolStripMenuItem 管理ToolStripMenuItem;
        private ToolStripMenuItem 漫畫管理ToolStripMenuItem;
        private ToolStripMenuItem 會員管理ToolStripMenuItem;
        private ToolStripMenuItem rentalManagementToolStripMenuItem;
        private ToolStripMenuItem 工具ToolStripMenuItem;
        private ToolStripMenuItem 檢視日誌ToolStripMenuItem;
        private ToolStripMenuItem 日誌管理ToolStripMenuItem;
        private ToolStripMenuItem 使用者註冊ToolStripMenuItem;
        private ToolStripMenuItem logoutToolStripMenuItem;

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelUser;

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            leftNavPanel = new Panel();
            btnNavLogs = new Button();
            btnNavUserReg = new Button();
            btnNavRentalMgmt = new Button();
            btnNavMemberMgmt = new Button();
            btnNavComicMgmt = new Button();
            btnNavDashboard = new Button();
            pnlAdminComicMgmt = new Panel();
            cmbAdminComicFilterStatus = new ComboBox();
            lblAvailableComics = new Label();
            dgvAvailableComics = new DataGridView();
            mainContentPanel = new Panel();
            memberViewTabControl = new TabControl();
            availableComicsTabPage = new TabPage();
            availableComicsFilterPanel = new Panel();
            txtSearchAvailableComics = new TextBox();
            cmbGenreFilter = new ComboBox();
            lblAvailableLegend = new Label();
            myRentalsTabPage = new TabPage();
            dgvMyRentedComics = new DataGridView();
            lblMyRentedComicsHeader = new Label();
            lblMyRentalsLegend = new Label();
            menuStrip1 = new MenuStrip();
            menuStrip2 = new MenuStrip();
            檔案ToolStripMenuItem = new ToolStripMenuItem();
            離開ToolStripMenuItem = new ToolStripMenuItem();
            管理ToolStripMenuItem = new ToolStripMenuItem();
            漫畫管理ToolStripMenuItem = new ToolStripMenuItem();
            會員管理ToolStripMenuItem = new ToolStripMenuItem();
            rentalManagementToolStripMenuItem = new ToolStripMenuItem();
            工具ToolStripMenuItem = new ToolStripMenuItem();
            檢視日誌ToolStripMenuItem = new ToolStripMenuItem();
            日誌管理ToolStripMenuItem = new ToolStripMenuItem();
            使用者註冊ToolStripMenuItem = new ToolStripMenuItem();
            logoutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelUser = new ToolStripStatusLabel();
            leftNavPanel.SuspendLayout();
            pnlAdminComicMgmt.SuspendLayout();
            ((ISupportInitialize)dgvAvailableComics).BeginInit();
            mainContentPanel.SuspendLayout();
            memberViewTabControl.SuspendLayout();
            availableComicsTabPage.SuspendLayout();
            availableComicsFilterPanel.SuspendLayout();
            myRentalsTabPage.SuspendLayout();
            ((ISupportInitialize)dgvMyRentedComics).BeginInit();
            menuStrip2.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // leftNavPanel
            // 
            leftNavPanel.BackColor = Color.FromArgb(248, 249, 250);
            leftNavPanel.Controls.Add(btnNavLogs);
            leftNavPanel.Controls.Add(btnNavUserReg);
            leftNavPanel.Controls.Add(btnNavRentalMgmt);
            leftNavPanel.Controls.Add(btnNavMemberMgmt);
            leftNavPanel.Controls.Add(btnNavComicMgmt);
            leftNavPanel.Controls.Add(btnNavDashboard);
            leftNavPanel.Dock = DockStyle.Left;
            leftNavPanel.Location = new Point(8, 56);
            leftNavPanel.Name = "leftNavPanel";
            leftNavPanel.Size = new Size(140, 589);
            leftNavPanel.TabIndex = 0;
            leftNavPanel.Visible = false;
            // 
            // btnNavLogs
            // 
            btnNavLogs.BackColor = Color.FromArgb(248, 249, 250);
            btnNavLogs.Dock = DockStyle.Top;
            btnNavLogs.FlatAppearance.BorderSize = 0;
            btnNavLogs.FlatStyle = FlatStyle.Flat;
            btnNavLogs.Font = new Font("Segoe UI Semibold", 9.75F);
            btnNavLogs.ForeColor = Color.FromArgb(33, 37, 41);
            btnNavLogs.Location = new Point(0, 180);
            btnNavLogs.Name = "btnNavLogs";
            btnNavLogs.Padding = new Padding(8, 0, 0, 0);
            btnNavLogs.Size = new Size(140, 36);
            btnNavLogs.TabIndex = 5;
            btnNavLogs.Text = "📄 日誌管理";
            btnNavLogs.TextAlign = ContentAlignment.MiddleLeft;
            btnNavLogs.UseVisualStyleBackColor = false;
            // 
            // btnNavUserReg
            // 
            btnNavUserReg.BackColor = Color.FromArgb(248, 249, 250);
            btnNavUserReg.Dock = DockStyle.Top;
            btnNavUserReg.FlatAppearance.BorderSize = 0;
            btnNavUserReg.FlatStyle = FlatStyle.Flat;
            btnNavUserReg.Font = new Font("Segoe UI Semibold", 9.75F);
            btnNavUserReg.ForeColor = Color.FromArgb(33, 37, 41);
            btnNavUserReg.Location = new Point(0, 144);
            btnNavUserReg.Name = "btnNavUserReg";
            btnNavUserReg.Padding = new Padding(8, 0, 0, 0);
            btnNavUserReg.Size = new Size(140, 36);
            btnNavUserReg.TabIndex = 4;
            btnNavUserReg.Text = "📝 使用者註冊";
            btnNavUserReg.TextAlign = ContentAlignment.MiddleLeft;
            btnNavUserReg.UseVisualStyleBackColor = false;
            // 
            // btnNavRentalMgmt
            // 
            btnNavRentalMgmt.BackColor = Color.FromArgb(248, 249, 250);
            btnNavRentalMgmt.Dock = DockStyle.Top;
            btnNavRentalMgmt.FlatAppearance.BorderSize = 0;
            btnNavRentalMgmt.FlatStyle = FlatStyle.Flat;
            btnNavRentalMgmt.Font = new Font("Segoe UI Semibold", 9.75F);
            btnNavRentalMgmt.ForeColor = Color.FromArgb(33, 37, 41);
            btnNavRentalMgmt.Location = new Point(0, 108);
            btnNavRentalMgmt.Name = "btnNavRentalMgmt";
            btnNavRentalMgmt.Padding = new Padding(8, 0, 0, 0);
            btnNavRentalMgmt.Size = new Size(140, 36);
            btnNavRentalMgmt.TabIndex = 3;
            btnNavRentalMgmt.Text = "📖 租借管理";
            btnNavRentalMgmt.TextAlign = ContentAlignment.MiddleLeft;
            btnNavRentalMgmt.UseVisualStyleBackColor = false;
            // 
            // btnNavMemberMgmt
            // 
            btnNavMemberMgmt.BackColor = Color.FromArgb(248, 249, 250);
            btnNavMemberMgmt.Dock = DockStyle.Top;
            btnNavMemberMgmt.FlatAppearance.BorderSize = 0;
            btnNavMemberMgmt.FlatStyle = FlatStyle.Flat;
            btnNavMemberMgmt.Font = new Font("Segoe UI Semibold", 9.75F);
            btnNavMemberMgmt.ForeColor = Color.FromArgb(33, 37, 41);
            btnNavMemberMgmt.Location = new Point(0, 72);
            btnNavMemberMgmt.Name = "btnNavMemberMgmt";
            btnNavMemberMgmt.Padding = new Padding(8, 0, 0, 0);
            btnNavMemberMgmt.Size = new Size(140, 36);
            btnNavMemberMgmt.TabIndex = 2;
            btnNavMemberMgmt.Text = "👥 會員管理";
            btnNavMemberMgmt.TextAlign = ContentAlignment.MiddleLeft;
            btnNavMemberMgmt.UseVisualStyleBackColor = false;
            // 
            // btnNavComicMgmt
            // 
            btnNavComicMgmt.BackColor = Color.FromArgb(248, 249, 250);
            btnNavComicMgmt.Dock = DockStyle.Top;
            btnNavComicMgmt.FlatAppearance.BorderSize = 0;
            btnNavComicMgmt.FlatStyle = FlatStyle.Flat;
            btnNavComicMgmt.Font = new Font("Segoe UI Semibold", 9.75F);
            btnNavComicMgmt.ForeColor = Color.FromArgb(33, 37, 41);
            btnNavComicMgmt.Location = new Point(0, 36);
            btnNavComicMgmt.Name = "btnNavComicMgmt";
            btnNavComicMgmt.Padding = new Padding(8, 0, 0, 0);
            btnNavComicMgmt.Size = new Size(140, 36);
            btnNavComicMgmt.TabIndex = 1;
            btnNavComicMgmt.Text = "📚 漫畫狀態";
            btnNavComicMgmt.TextAlign = ContentAlignment.MiddleLeft;
            btnNavComicMgmt.UseVisualStyleBackColor = false;
            // 
            // btnNavDashboard
            // 
            btnNavDashboard.BackColor = Color.FromArgb(248, 249, 250);
            btnNavDashboard.Dock = DockStyle.Top;
            btnNavDashboard.FlatAppearance.BorderSize = 0;
            btnNavDashboard.FlatStyle = FlatStyle.Flat;
            btnNavDashboard.Font = new Font("Segoe UI Semibold", 9.75F);
            btnNavDashboard.ForeColor = Color.FromArgb(33, 37, 41);
            btnNavDashboard.Location = new Point(0, 0);
            btnNavDashboard.Name = "btnNavDashboard";
            btnNavDashboard.Padding = new Padding(8, 0, 0, 0);
            btnNavDashboard.Size = new Size(140, 36);
            btnNavDashboard.TabIndex = 0;
            btnNavDashboard.Text = "🏠 概要";
            btnNavDashboard.TextAlign = ContentAlignment.MiddleLeft;
            btnNavDashboard.UseVisualStyleBackColor = false;
            // 
            // pnlAdminComicMgmt
            // 
            pnlAdminComicMgmt.Controls.Add(cmbAdminComicFilterStatus);
            pnlAdminComicMgmt.Dock = DockStyle.Fill;
            pnlAdminComicMgmt.Location = new Point(0, 0);
            pnlAdminComicMgmt.Name = "pnlAdminComicMgmt";
            pnlAdminComicMgmt.Size = new Size(969, 589);
            pnlAdminComicMgmt.TabIndex = 20;
            pnlAdminComicMgmt.Visible = false;
            // 
            // cmbAdminComicFilterStatus
            // 
            cmbAdminComicFilterStatus.FormattingEnabled = true;
            cmbAdminComicFilterStatus.Location = new Point(200, 12);
            cmbAdminComicFilterStatus.Name = "cmbAdminComicFilterStatus";
            cmbAdminComicFilterStatus.Size = new Size(150, 23);
            cmbAdminComicFilterStatus.TabIndex = 1;
            cmbAdminComicFilterStatus.Visible = false;
            // 
            // lblAvailableComics
            // 
            lblAvailableComics.AutoSize = true;
            lblAvailableComics.Dock = DockStyle.Top;
            lblAvailableComics.Font = new Font("Segoe UI Semibold", 12F);
            lblAvailableComics.Location = new Point(8, 48);
            lblAvailableComics.Name = "lblAvailableComics";
            lblAvailableComics.Padding = new Padding(5);
            lblAvailableComics.Size = new Size(88, 31);
            lblAvailableComics.TabIndex = 0;
            lblAvailableComics.Text = "漫畫狀態";
            lblAvailableComics.Visible = false;
            // 
            // dgvAvailableComics
            // 
            dgvAvailableComics.AllowUserToAddRows = false;
            dgvAvailableComics.AllowUserToDeleteRows = false;
            dgvAvailableComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAvailableComics.Dock = DockStyle.Fill;
            dgvAvailableComics.Location = new Point(8, 79);
            dgvAvailableComics.Name = "dgvAvailableComics";
            dgvAvailableComics.ReadOnly = true;
            dgvAvailableComics.RowHeadersWidth = 51;
            dgvAvailableComics.Size = new Size(945, 449);
            dgvAvailableComics.TabIndex = 2;
            // 
            // mainContentPanel
            // 
            mainContentPanel.BackColor = Color.White;
            mainContentPanel.Controls.Add(memberViewTabControl);
            mainContentPanel.Controls.Add(pnlAdminComicMgmt);
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.Location = new Point(148, 56);
            mainContentPanel.Name = "mainContentPanel";
            mainContentPanel.Size = new Size(969, 589);
            mainContentPanel.TabIndex = 2;
            // 
            // memberViewTabControl
            // 
            memberViewTabControl.Controls.Add(availableComicsTabPage);
            memberViewTabControl.Controls.Add(myRentalsTabPage);
            memberViewTabControl.Dock = DockStyle.Fill;
            memberViewTabControl.Font = new Font("Segoe UI", 9F);
            memberViewTabControl.Location = new Point(0, 0);
            memberViewTabControl.Name = "memberViewTabControl";
            memberViewTabControl.SelectedIndex = 0;
            memberViewTabControl.Size = new Size(969, 589);
            memberViewTabControl.TabIndex = 12;
            // 
            // availableComicsTabPage
            // 
            availableComicsTabPage.BackColor = Color.White;
            availableComicsTabPage.Controls.Add(dgvAvailableComics);
            availableComicsTabPage.Controls.Add(lblAvailableComics);
            availableComicsTabPage.Controls.Add(availableComicsFilterPanel);
            availableComicsTabPage.Controls.Add(lblAvailableLegend);
            availableComicsTabPage.Location = new Point(4, 24);
            availableComicsTabPage.Name = "availableComicsTabPage";
            availableComicsTabPage.Padding = new Padding(8);
            availableComicsTabPage.Size = new Size(961, 561);
            availableComicsTabPage.TabIndex = 0;
            availableComicsTabPage.Text = "可租借漫畫";
            // 
            // availableComicsFilterPanel
            // 
            availableComicsFilterPanel.AutoSize = true;
            availableComicsFilterPanel.Controls.Add(txtSearchAvailableComics);
            availableComicsFilterPanel.Controls.Add(cmbGenreFilter);
            availableComicsFilterPanel.Dock = DockStyle.Top;
            availableComicsFilterPanel.Location = new Point(8, 8);
            availableComicsFilterPanel.Name = "availableComicsFilterPanel";
            availableComicsFilterPanel.Padding = new Padding(0, 4, 0, 4);
            availableComicsFilterPanel.Size = new Size(945, 40);
            availableComicsFilterPanel.TabIndex = 0;
            // 
            // txtSearchAvailableComics
            // 
            txtSearchAvailableComics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearchAvailableComics.Font = new Font("Segoe UI", 9F);
            txtSearchAvailableComics.ForeColor = Color.Gray;
            txtSearchAvailableComics.Location = new Point(3, 10);
            txtSearchAvailableComics.Name = "txtSearchAvailableComics";
            txtSearchAvailableComics.Size = new Size(939, 23);
            txtSearchAvailableComics.TabIndex = 0;
            // 
            // cmbGenreFilter
            // 
            cmbGenreFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbGenreFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGenreFilter.Font = new Font("Segoe UI", 9F);
            cmbGenreFilter.FormattingEnabled = true;
            cmbGenreFilter.Location = new Point(1108, 10);
            cmbGenreFilter.Name = "cmbGenreFilter";
            cmbGenreFilter.Size = new Size(141, 23);
            cmbGenreFilter.TabIndex = 1;
            // 
            // lblAvailableLegend
            // 
            lblAvailableLegend.AutoSize = true;
            lblAvailableLegend.Dock = DockStyle.Bottom;
            lblAvailableLegend.Font = new Font("Segoe UI", 9F);
            lblAvailableLegend.ForeColor = Color.FromArgb(33, 37, 41);
            lblAvailableLegend.Location = new Point(8, 528);
            lblAvailableLegend.Name = "lblAvailableLegend";
            lblAvailableLegend.Padding = new Padding(5);
            lblAvailableLegend.Size = new Size(212, 25);
            lblAvailableLegend.TabIndex = 3;
            lblAvailableLegend.Text = "紅色表示逾期，黃色表示即將到期";
            // 
            // myRentalsTabPage
            // 
            myRentalsTabPage.BackColor = Color.White;
            myRentalsTabPage.Controls.Add(dgvMyRentedComics);
            myRentalsTabPage.Controls.Add(lblMyRentedComicsHeader);
            myRentalsTabPage.Controls.Add(lblMyRentalsLegend);
            myRentalsTabPage.Location = new Point(4, 24);
            myRentalsTabPage.Name = "myRentalsTabPage";
            myRentalsTabPage.Padding = new Padding(8);
            myRentalsTabPage.Size = new Size(961, 554);
            myRentalsTabPage.TabIndex = 1;
            myRentalsTabPage.Text = "我租借的漫畫";
            // 
            // dgvMyRentedComics
            // 
            dgvMyRentedComics.AllowUserToAddRows = false;
            dgvMyRentedComics.AllowUserToDeleteRows = false;
            dgvMyRentedComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMyRentedComics.Dock = DockStyle.Fill;
            dgvMyRentedComics.Location = new Point(8, 27);
            dgvMyRentedComics.Name = "dgvMyRentedComics";
            dgvMyRentedComics.ReadOnly = true;
            dgvMyRentedComics.RowHeadersWidth = 51;
            dgvMyRentedComics.RowTemplate.Height = 27;
            dgvMyRentedComics.Size = new Size(945, 494);
            dgvMyRentedComics.TabIndex = 7;
            // 
            // lblMyRentedComicsHeader
            // 
            lblMyRentedComicsHeader.AutoSize = true;
            lblMyRentedComicsHeader.Dock = DockStyle.Top;
            lblMyRentedComicsHeader.Font = new Font("Segoe UI Semibold", 10F);
            lblMyRentedComicsHeader.Location = new Point(8, 8);
            lblMyRentedComicsHeader.Name = "lblMyRentedComicsHeader";
            lblMyRentedComicsHeader.Size = new Size(93, 19);
            lblMyRentedComicsHeader.TabIndex = 6;
            lblMyRentedComicsHeader.Text = "我租借的漫畫";
            lblMyRentedComicsHeader.Visible = false;
            // 
            // lblMyRentalsLegend
            // 
            lblMyRentalsLegend.AutoSize = true;
            lblMyRentalsLegend.Dock = DockStyle.Bottom;
            lblMyRentalsLegend.Font = new Font("Segoe UI", 9F);
            lblMyRentalsLegend.ForeColor = Color.FromArgb(33, 37, 41);
            lblMyRentalsLegend.Location = new Point(8, 521);
            lblMyRentalsLegend.Name = "lblMyRentalsLegend";
            lblMyRentalsLegend.Padding = new Padding(5);
            lblMyRentalsLegend.Size = new Size(212, 25);
            lblMyRentalsLegend.TabIndex = 8;
            lblMyRentalsLegend.Text = "紅色表示逾期，黃色表示即將到期";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Location = new Point(8, 32);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1109, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            menuStrip2.ImageScalingSize = new Size(20, 20);
            menuStrip2.Items.AddRange(new ToolStripItem[] { 檔案ToolStripMenuItem, 管理ToolStripMenuItem, 工具ToolStripMenuItem, 使用者註冊ToolStripMenuItem, logoutToolStripMenuItem });
            menuStrip2.Location = new Point(8, 8);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Padding = new Padding(5, 2, 0, 2);
            menuStrip2.Size = new Size(1109, 24);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            // 
            // 檔案ToolStripMenuItem
            // 
            檔案ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 離開ToolStripMenuItem });
            檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            檔案ToolStripMenuItem.Size = new Size(43, 20);
            檔案ToolStripMenuItem.Text = "檔案";
            // 
            // 離開ToolStripMenuItem
            // 
            離開ToolStripMenuItem.Name = "離開ToolStripMenuItem";
            離開ToolStripMenuItem.Size = new Size(98, 22);
            離開ToolStripMenuItem.Text = "離開";
            離開ToolStripMenuItem.Click += 離開ToolStripMenuItem_Click;
            // 
            // 管理ToolStripMenuItem
            // 
            管理ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 漫畫管理ToolStripMenuItem, 會員管理ToolStripMenuItem, rentalManagementToolStripMenuItem });
            管理ToolStripMenuItem.Name = "管理ToolStripMenuItem";
            管理ToolStripMenuItem.Size = new Size(43, 20);
            管理ToolStripMenuItem.Text = "管理";
            // 
            // 漫畫管理ToolStripMenuItem
            // 
            漫畫管理ToolStripMenuItem.Name = "漫畫管理ToolStripMenuItem";
            漫畫管理ToolStripMenuItem.Size = new Size(122, 22);
            漫畫管理ToolStripMenuItem.Text = "漫畫管理";
            漫畫管理ToolStripMenuItem.Click += 漫畫管理ToolStripMenuItem_Click;
            // 
            // 會員管理ToolStripMenuItem
            // 
            會員管理ToolStripMenuItem.Name = "會員管理ToolStripMenuItem";
            會員管理ToolStripMenuItem.Size = new Size(122, 22);
            會員管理ToolStripMenuItem.Text = "會員管理";
            會員管理ToolStripMenuItem.Click += 會員管理ToolStripMenuItem_Click;
            // 
            // rentalManagementToolStripMenuItem
            // 
            rentalManagementToolStripMenuItem.Name = "rentalManagementToolStripMenuItem";
            rentalManagementToolStripMenuItem.Size = new Size(122, 22);
            rentalManagementToolStripMenuItem.Text = "租借管理";
            rentalManagementToolStripMenuItem.Click += rentalManagementToolStripMenuItem_Click;
            // 
            // 工具ToolStripMenuItem
            // 
            工具ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 檢視日誌ToolStripMenuItem, 日誌管理ToolStripMenuItem });
            工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            工具ToolStripMenuItem.Size = new Size(43, 20);
            工具ToolStripMenuItem.Text = "工具";
            // 
            // 檢視日誌ToolStripMenuItem
            // 
            檢視日誌ToolStripMenuItem.Name = "檢視日誌ToolStripMenuItem";
            檢視日誌ToolStripMenuItem.Size = new Size(122, 22);
            檢視日誌ToolStripMenuItem.Text = "檢視日誌";
            檢視日誌ToolStripMenuItem.Click += 檢視日誌ToolStripMenuItem_Click;
            // 
            // 日誌管理ToolStripMenuItem
            // 
            日誌管理ToolStripMenuItem.Name = "日誌管理ToolStripMenuItem";
            日誌管理ToolStripMenuItem.Size = new Size(122, 22);
            日誌管理ToolStripMenuItem.Text = "日誌管理";
            日誌管理ToolStripMenuItem.Click += 日誌管理ToolStripMenuItem_Click;
            // 
            // 使用者註冊ToolStripMenuItem
            // 
            使用者註冊ToolStripMenuItem.Name = "使用者註冊ToolStripMenuItem";
            使用者註冊ToolStripMenuItem.Size = new Size(98, 20);
            使用者註冊ToolStripMenuItem.Text = "使用者註冊 (&R)";
            使用者註冊ToolStripMenuItem.Click += 使用者註冊ToolStripMenuItem_Click;
            // 
            // logoutToolStripMenuItem
            // 
            logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            logoutToolStripMenuItem.Size = new Size(60, 20);
            logoutToolStripMenuItem.Text = "登出 (&L)";
            logoutToolStripMenuItem.Click += logoutToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelUser });
            statusStrip1.Location = new Point(8, 645);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 11, 0);
            statusStrip1.Size = new Size(1109, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelUser
            // 
            toolStripStatusLabelUser.Name = "toolStripStatusLabelUser";
            toolStripStatusLabelUser.Size = new Size(112, 17);
            toolStripStatusLabelUser.Text = "使用者: 無 | 角色: 無";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1125, 675);
            Controls.Add(mainContentPanel);
            Controls.Add(leftNavPanel);
            Controls.Add(menuStrip1);
            Controls.Add(menuStrip2);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStrip2;
            Name = "MainForm";
            Padding = new Padding(8);
            Text = "漫畫租借系統";
            Load += MainForm_Load;
            leftNavPanel.ResumeLayout(false);
            pnlAdminComicMgmt.ResumeLayout(false);
            ((ISupportInitialize)dgvAvailableComics).EndInit();
            mainContentPanel.ResumeLayout(false);
            memberViewTabControl.ResumeLayout(false);
            availableComicsTabPage.ResumeLayout(false);
            availableComicsTabPage.PerformLayout();
            availableComicsFilterPanel.ResumeLayout(false);
            availableComicsFilterPanel.PerformLayout();
            myRentalsTabPage.ResumeLayout(false);
            myRentalsTabPage.PerformLayout();
            ((ISupportInitialize)dgvMyRentedComics).EndInit();
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
