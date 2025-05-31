// In ComicRentalSystem_14Days/MainForm.Designer.cs

namespace ComicRentalSystem_14Days
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Added for new UI structure
        private System.Windows.Forms.Panel leftNavPanel;
        private System.Windows.Forms.Panel mainContentPanel;
        private System.Windows.Forms.Button btnNavDashboard;
        private System.Windows.Forms.Button btnNavComicMgmt;
        private System.Windows.Forms.Button btnNavMemberMgmt;
        private System.Windows.Forms.Button btnNavRentalMgmt;
        private System.Windows.Forms.Button btnNavUserReg;
        private System.Windows.Forms.Button btnNavLogs;
        private System.Windows.Forms.ComboBox cmbAdminComicFilterStatus; // Added as it was not a field

        // Fields for Member View TabControl
        private System.Windows.Forms.TabControl memberViewTabControl;
        private System.Windows.Forms.TabPage availableComicsTabPage;
        private System.Windows.Forms.TabPage myRentalsTabPage;

        // Fields for Available Comics Filter
        private System.Windows.Forms.Panel availableComicsFilterPanel;
        private System.Windows.Forms.TextBox txtSearchAvailableComics;
        private System.Windows.Forms.ComboBox cmbGenreFilter;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Instantiate new controls first
            this.leftNavPanel = new System.Windows.Forms.Panel();
            this.mainContentPanel = new System.Windows.Forms.Panel();
            this.btnNavDashboard = new System.Windows.Forms.Button();
            this.btnNavComicMgmt = new System.Windows.Forms.Button();
            this.btnNavMemberMgmt = new System.Windows.Forms.Button();
            this.btnNavRentalMgmt = new System.Windows.Forms.Button();
            this.btnNavUserReg = new System.Windows.Forms.Button();
            this.btnNavLogs = new System.Windows.Forms.Button();
            this.cmbAdminComicFilterStatus = new System.Windows.Forms.ComboBox();

            // Instantiate TabControl and TabPages
            this.memberViewTabControl = new System.Windows.Forms.TabControl();
            this.availableComicsTabPage = new System.Windows.Forms.TabPage();
            this.myRentalsTabPage = new System.Windows.Forms.TabPage();

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
            dgvAvailableComics = new DataGridView();
            lblAvailableComics = new Label();
            this.使用者註冊ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRentComic = new System.Windows.Forms.Button();
            this.lblMyRentedComicsHeader = new System.Windows.Forms.Label();
            this.dgvMyRentedComics = new System.Windows.Forms.DataGridView();

            // Suspend layout for main form and panels
            this.SuspendLayout();
            this.mainContentPanel.SuspendLayout();
            this.leftNavPanel.SuspendLayout();
            this.memberViewTabControl.SuspendLayout(); // Suspend TabControl layout
            this.availableComicsTabPage.SuspendLayout(); // Suspend TabPage layout
            this.myRentalsTabPage.SuspendLayout(); // Suspend TabPage layout

            menuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAvailableComics).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRentedComics)).BeginInit(); // Ensure this exists if dgvMyRentedComics is used
            this.statusStrip1.SuspendLayout();

            //
            // mainContentPanel
            //
            this.mainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContentPanel.Location = new System.Drawing.Point(180, 51); // Initial placeholder, adjust after adding menu/status
            this.mainContentPanel.Name = "mainContentPanel";
            this.mainContentPanel.Padding = new System.Windows.Forms.Padding(10);
            this.mainContentPanel.Size = new System.Drawing.Size(720, 639); // Initial placeholder
            this.mainContentPanel.TabIndex = 11;
            this.mainContentPanel.BackColor = System.Drawing.Color.White;
            //
            // leftNavPanel
            //
            this.leftNavPanel.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.leftNavPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftNavPanel.Location = new System.Drawing.Point(0, 51); // Initial placeholder
            this.leftNavPanel.Name = "leftNavPanel";
            this.leftNavPanel.Size = new System.Drawing.Size(180, 639); // Initial placeholder
            this.leftNavPanel.TabIndex = 10;
            this.leftNavPanel.Visible = false;
            //
            // btnNavDashboard
            //
            this.btnNavDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavDashboard.FlatAppearance.BorderSize = 0;
            this.btnNavDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavDashboard.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnNavDashboard.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41); // TextColor
            this.btnNavDashboard.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnNavDashboard.Size = new System.Drawing.Size(180, 45);
            this.btnNavDashboard.TabIndex = 0;
            this.btnNavDashboard.Text = "概要";
            this.btnNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavDashboard.UseVisualStyleBackColor = false;
            //
            // btnNavComicMgmt
            //
            this.btnNavComicMgmt.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavComicMgmt.FlatAppearance.BorderSize = 0;
            this.btnNavComicMgmt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavComicMgmt.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnNavComicMgmt.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41); // TextColor
            this.btnNavComicMgmt.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.btnNavComicMgmt.Name = "btnNavComicMgmt";
            this.btnNavComicMgmt.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnNavComicMgmt.Size = new System.Drawing.Size(180, 45);
            this.btnNavComicMgmt.TabIndex = 1;
            this.btnNavComicMgmt.Text = "漫畫管理";
            this.btnNavComicMgmt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavComicMgmt.UseVisualStyleBackColor = false;
            //
            // btnNavMemberMgmt
            //
            this.btnNavMemberMgmt.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavMemberMgmt.FlatAppearance.BorderSize = 0;
            this.btnNavMemberMgmt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavMemberMgmt.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnNavMemberMgmt.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41); // TextColor
            this.btnNavMemberMgmt.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.btnNavMemberMgmt.Name = "btnNavMemberMgmt";
            this.btnNavMemberMgmt.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnNavMemberMgmt.Size = new System.Drawing.Size(180, 45);
            this.btnNavMemberMgmt.TabIndex = 2;
            this.btnNavMemberMgmt.Text = "會員管理";
            this.btnNavMemberMgmt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavMemberMgmt.UseVisualStyleBackColor = false;
            //
            // btnNavRentalMgmt
            //
            this.btnNavRentalMgmt.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavRentalMgmt.FlatAppearance.BorderSize = 0;
            this.btnNavRentalMgmt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavRentalMgmt.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnNavRentalMgmt.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41); // TextColor
            this.btnNavRentalMgmt.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.btnNavRentalMgmt.Name = "btnNavRentalMgmt";
            this.btnNavRentalMgmt.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnNavRentalMgmt.Size = new System.Drawing.Size(180, 45);
            this.btnNavRentalMgmt.TabIndex = 3;
            this.btnNavRentalMgmt.Text = "租借管理";
            this.btnNavRentalMgmt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavRentalMgmt.UseVisualStyleBackColor = false;
            //
            // btnNavUserReg
            //
            this.btnNavUserReg.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavUserReg.FlatAppearance.BorderSize = 0;
            this.btnNavUserReg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavUserReg.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnNavUserReg.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41); // TextColor
            this.btnNavUserReg.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.btnNavUserReg.Name = "btnNavUserReg";
            this.btnNavUserReg.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnNavUserReg.Size = new System.Drawing.Size(180, 45);
            this.btnNavUserReg.TabIndex = 4;
            this.btnNavUserReg.Text = "使用者註冊";
            this.btnNavUserReg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavUserReg.UseVisualStyleBackColor = false;
            //
            // btnNavLogs
            //
            this.btnNavLogs.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNavLogs.FlatAppearance.BorderSize = 0;
            this.btnNavLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavLogs.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnNavLogs.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41); // TextColor
            this.btnNavLogs.BackColor = System.Drawing.Color.FromArgb(248, 249, 250); // SecondaryColor
            this.btnNavLogs.Name = "btnNavLogs";
            this.btnNavLogs.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnNavLogs.Size = new System.Drawing.Size(180, 45);
            this.btnNavLogs.TabIndex = 5;
            this.btnNavLogs.Text = "檢視日誌";
            this.btnNavLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavLogs.UseVisualStyleBackColor = false;
            //
            // leftNavPanel Controls
            //
            this.leftNavPanel.Controls.Add(this.btnNavLogs);
            this.leftNavPanel.Controls.Add(this.btnNavUserReg);
            this.leftNavPanel.Controls.Add(this.btnNavRentalMgmt);
            this.leftNavPanel.Controls.Add(this.btnNavMemberMgmt);
            this.leftNavPanel.Controls.Add(this.btnNavComicMgmt);
            this.leftNavPanel.Controls.Add(this.btnNavDashboard);
            //
            // menuStrip1
            //
            // menuStrip1 must be configured before it's added to Controls
            // menuStrip1 is also defined later, ensure declarations are proper
            // For now, I'll assume menuStrip1 and menuStrip2 are correctly defined before this point
            // And that their configurations (Items.AddRange, etc.) are also handled.
            // The critical part is adding them to the main form's Controls collection.
            //
            // Original SuspendLayout() and other initializations:
            // menuStrip2.SuspendLayout(); // Already called above
            // ((System.ComponentModel.ISupportInitialize)dgvAvailableComics).BeginInit(); // Called above
            // this.statusStrip1.SuspendLayout(); // Called above
            // SuspendLayout(); // Called for the form above

            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Location = new Point(0, 27); // This menuStrip appears unused for items
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(900, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            menuStrip2.ImageScalingSize = new Size(20, 20);
            // Added 使用者註冊ToolStripMenuItem and logoutToolStripMenuItem here
            menuStrip2.Items.AddRange(new ToolStripItem[] { 檔案ToolStripMenuItem, 管理ToolStripMenuItem, 工具ToolStripMenuItem, this.使用者註冊ToolStripMenuItem, this.logoutToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Padding = new Padding(7, 2, 0, 2);
            menuStrip2.Size = new Size(900, 27);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            // 
            // btnRentComic, lblMyRentedComicsHeader, dgvMyRentedComics are already declared as fields.
            // Their instantiation happens above with other controls.
            // Their specific properties will be set when they are added to mainContentPanel.
            //
            // 檔案ToolStripMenuItem
            // 
            檔案ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 離開ToolStripMenuItem });
            檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            檔案ToolStripMenuItem.Size = new Size(53, 23);
            檔案ToolStripMenuItem.Text = "檔案";
            // 
            // 離開ToolStripMenuItem
            // 
            離開ToolStripMenuItem.Name = "離開ToolStripMenuItem";
            離開ToolStripMenuItem.Size = new Size(122, 26);
            離開ToolStripMenuItem.Text = "離開";
            離開ToolStripMenuItem.Click += 離開ToolStripMenuItem_Click;
            // 
            // 管理ToolStripMenuItem
            // 
            管理ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 漫畫管理ToolStripMenuItem, 會員管理ToolStripMenuItem, rentalManagementToolStripMenuItem });
            管理ToolStripMenuItem.Name = "管理ToolStripMenuItem";
            管理ToolStripMenuItem.Size = new Size(53, 23);
            管理ToolStripMenuItem.Text = "管理";
            // 
            // 漫畫管理ToolStripMenuItem
            // 
            漫畫管理ToolStripMenuItem.Name = "漫畫管理ToolStripMenuItem";
            漫畫管理ToolStripMenuItem.Size = new Size(152, 26);
            漫畫管理ToolStripMenuItem.Text = "漫畫管理";
            漫畫管理ToolStripMenuItem.Click += 漫畫管理ToolStripMenuItem_Click;
            // 
            // 會員管理ToolStripMenuItem
            // 
            會員管理ToolStripMenuItem.Name = "會員管理ToolStripMenuItem";
            會員管理ToolStripMenuItem.Size = new Size(152, 26);
            會員管理ToolStripMenuItem.Text = "會員管理";
            會員管理ToolStripMenuItem.Click += 會員管理ToolStripMenuItem_Click;
            // 
            // rentalManagementToolStripMenuItem
            // 
            rentalManagementToolStripMenuItem.Name = "rentalManagementToolStripMenuItem";
            rentalManagementToolStripMenuItem.Size = new Size(152, 26);
            rentalManagementToolStripMenuItem.Text = "租借管理";
            rentalManagementToolStripMenuItem.Click += rentalManagementToolStripMenuItem_Click;
            // 
            // 工具ToolStripMenuItem
            // 
            工具ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 檢視日誌ToolStripMenuItem });
            工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            工具ToolStripMenuItem.Size = new Size(53, 23);
            工具ToolStripMenuItem.Text = "工具";
            // 
            // 檢視日誌ToolStripMenuItem
            // 
            檢視日誌ToolStripMenuItem.Name = "檢視日誌ToolStripMenuItem";
            檢視日誌ToolStripMenuItem.Size = new Size(152, 26);
            檢視日誌ToolStripMenuItem.Text = "檢視日誌";
            檢視日誌ToolStripMenuItem.Click += 檢視日誌ToolStripMenuItem_Click;
            // 
            // 使用者註冊ToolStripMenuItem
            //
            this.使用者註冊ToolStripMenuItem.Name = "使用者註冊ToolStripMenuItem";
            this.使用者註冊ToolStripMenuItem.Size = new System.Drawing.Size(103, 23);
            this.使用者註冊ToolStripMenuItem.Text = "使用者註冊 (&R)";
            this.使用者註冊ToolStripMenuItem.Click += new System.EventHandler(this.使用者註冊ToolStripMenuItem_Click);
            //
            // logoutToolStripMenuItem
            //
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(74, 23);
            this.logoutToolStripMenuItem.Text = "登出 (&L)";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            //
            // dgvAvailableComics
            // 
            this.dgvAvailableComics.AllowUserToAddRows = false;
            this.dgvAvailableComics.AllowUserToDeleteRows = false;
            this.dgvAvailableComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAvailableComics.Location = new System.Drawing.Point(10, 40); // Relative to mainContentPanel
            this.dgvAvailableComics.Margin = new System.Windows.Forms.Padding(4);
            this.dgvAvailableComics.Name = "dgvAvailableComics";
            this.dgvAvailableComics.ReadOnly = true;
            this.dgvAvailableComics.RowHeadersWidth = 51;
            this.dgvAvailableComics.Size = new System.Drawing.Size(700, 250); // Adjusted size
            this.dgvAvailableComics.TabIndex = 2;
            this.dgvAvailableComics.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right; // Anchor to fill
            // 
            // lblAvailableComics
            // 
            this.lblAvailableComics.AutoSize = true;
            // this.lblAvailableComics.Dock = DockStyle.Top; // Remove Dock, use Location
            this.lblAvailableComics.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            this.lblAvailableComics.Location = new System.Drawing.Point(10, 10); // Relative to mainContentPanel
            this.lblAvailableComics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAvailableComics.Name = "lblAvailableComics";
            this.lblAvailableComics.Padding = new System.Windows.Forms.Padding(5);
            this.lblAvailableComics.Size = new System.Drawing.Size(182, 35);
            this.lblAvailableComics.TabIndex = 3;
            this.lblAvailableComics.Text = "目前可借閱的漫畫";
            //
            // cmbAdminComicFilterStatus
            //
            this.cmbAdminComicFilterStatus.FormattingEnabled = true;
            this.cmbAdminComicFilterStatus.Location = new System.Drawing.Point(200, 10); // Example location
            this.cmbAdminComicFilterStatus.Name = "cmbAdminComicFilterStatus";
            this.cmbAdminComicFilterStatus.Size = new System.Drawing.Size(150, 27); // Example size
            this.cmbAdminComicFilterStatus.TabIndex = 4; // Example TabIndex
            this.cmbAdminComicFilterStatus.Visible = false; // Initially hidden for Admin
            // 
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelUser});
            this.statusStrip1.Location = new System.Drawing.Point(0, 481); // Adjusted Y based on new ClientSize
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(900, 22);
            this.statusStrip1.TabIndex = 4; // New TabIndex
            this.statusStrip1.Text = "statusStrip1";
            //
            // toolStripStatusLabelUser
            //
            this.toolStripStatusLabelUser.Name = "toolStripStatusLabelUser";
            this.toolStripStatusLabelUser.Size = new System.Drawing.Size(136, 17); // Example size, text might make it wider
            this.toolStripStatusLabelUser.Text = "User: None | Role: None";
            //
            // btnRentComic
            //
            this.btnRentComic.Location = new System.Drawing.Point(10, 295); // Relative to mainContentPanel
            this.btnRentComic.Name = "btnRentComic";
            this.btnRentComic.Size = new System.Drawing.Size(120, 30);
            this.btnRentComic.TabIndex = 5;
            this.btnRentComic.Text = "租借漫畫";
            this.btnRentComic.UseVisualStyleBackColor = true;
            this.btnRentComic.Visible = false;
            this.btnRentComic.Enabled = false;
            this.btnRentComic.Click += new System.EventHandler(this.btnRentComic_Click);
            //
            // lblMyRentedComicsHeader
            //
            this.lblMyRentedComicsHeader.AutoSize = true;
            this.lblMyRentedComicsHeader.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMyRentedComicsHeader.Location = new System.Drawing.Point(10, 330); // Relative to mainContentPanel
            this.lblMyRentedComicsHeader.Name = "lblMyRentedComicsHeader";
            this.lblMyRentedComicsHeader.Size = new System.Drawing.Size(123, 22);
            this.lblMyRentedComicsHeader.TabIndex = 6;
            this.lblMyRentedComicsHeader.Text = "我租借的漫畫";
            this.lblMyRentedComicsHeader.Visible = false;
            //
            // dgvMyRentedComics
            //
            this.dgvMyRentedComics.AllowUserToAddRows = false;
            this.dgvMyRentedComics.AllowUserToDeleteRows = false;
            this.dgvMyRentedComics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyRentedComics.Location = new System.Drawing.Point(10, 355); // Relative to mainContentPanel
            this.dgvMyRentedComics.Name = "dgvMyRentedComics";
            this.dgvMyRentedComics.ReadOnly = true;
            this.dgvMyRentedComics.RowHeadersWidth = 51;
            this.dgvMyRentedComics.RowTemplate.Height = 27;
            this.dgvMyRentedComics.Size = new System.Drawing.Size(700, 150); // Adjusted size
            this.dgvMyRentedComics.TabIndex = 7;
            this.dgvMyRentedComics.Visible = false;
            this.dgvMyRentedComics.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right); // Anchor to bottom, left, right

            //
            // memberViewTabControl
            //
            this.memberViewTabControl.Controls.Add(this.availableComicsTabPage);
            this.memberViewTabControl.Controls.Add(this.myRentalsTabPage);
            this.memberViewTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memberViewTabControl.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
            this.memberViewTabControl.Location = new System.Drawing.Point(10, 10); // Relative to mainContentPanel's padding
            this.memberViewTabControl.Name = "memberViewTabControl";
            this.memberViewTabControl.SelectedIndex = 0;
            this.memberViewTabControl.Size = new System.Drawing.Size(700, 619); // Adjusted to fill mainContentPanel padding
            this.memberViewTabControl.TabIndex = 12;
            this.memberViewTabControl.Visible = false; // Initially hidden
            //
            // availableComicsTabPage
            //
            // Instantiate new filter controls for availableComicsTabPage
            this.availableComicsFilterPanel = new System.Windows.Forms.Panel();
            this.txtSearchAvailableComics = new System.Windows.Forms.TextBox();
            this.cmbGenreFilter = new System.Windows.Forms.ComboBox();

            // Configure availableComicsFilterPanel
            this.availableComicsFilterPanel.SuspendLayout();
            this.availableComicsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.availableComicsFilterPanel.Location = new System.Drawing.Point(10, 45); // Placeholder, will be after lblAvailableComics
            this.availableComicsFilterPanel.Name = "availableComicsFilterPanel";
            this.availableComicsFilterPanel.Size = new System.Drawing.Size(672, 40); // Width of TabPage.ClientRectangle.Width - Padding*2
            this.availableComicsFilterPanel.TabIndex = 0; // First control in this specific logical group
            this.availableComicsFilterPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);

            // Configure txtSearchAvailableComics
            this.txtSearchAvailableComics.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtSearchAvailableComics.Location = new System.Drawing.Point(0, 7); // Relative to filter panel padding
            this.txtSearchAvailableComics.Name = "txtSearchAvailableComics";
            this.txtSearchAvailableComics.Size = new System.Drawing.Size(482, 23); // panel.width - cmb.width - spacing
            this.txtSearchAvailableComics.TabIndex = 0;
            this.txtSearchAvailableComics.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearchAvailableComics.Text = "Search by Title/Author...";
            this.txtSearchAvailableComics.ForeColor = System.Drawing.Color.Gray;

            // Configure cmbGenreFilter
            this.cmbGenreFilter.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            this.cmbGenreFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGenreFilter.FormattingEnabled = true;
            this.cmbGenreFilter.Location = new System.Drawing.Point(492, 7); // Relative to filter panel padding
            this.cmbGenreFilter.Name = "cmbGenreFilter";
            this.cmbGenreFilter.Size = new System.Drawing.Size(180, 23);
            this.cmbGenreFilter.TabIndex = 1;
            this.cmbGenreFilter.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);

            // Add filter controls to availableComicsFilterPanel
            this.availableComicsFilterPanel.Controls.Add(this.txtSearchAvailableComics);
            this.availableComicsFilterPanel.Controls.Add(this.cmbGenreFilter);
            this.availableComicsFilterPanel.ResumeLayout(false);
            this.availableComicsFilterPanel.PerformLayout(); // For TextBox and ComboBox within Panel

            // Configure availableComicsTabPage
            this.availableComicsTabPage.Controls.Clear(); // Clear before re-adding in specific order
            this.availableComicsTabPage.Controls.Add(this.lblAvailableComics);          // Top
            this.availableComicsTabPage.Controls.Add(this.availableComicsFilterPanel); // Below label, above DGV
            this.availableComicsTabPage.Controls.Add(this.dgvAvailableComics);         // Fill remaining space
            this.availableComicsTabPage.Controls.Add(this.btnRentComic);               // Bottom
            this.availableComicsTabPage.Location = new System.Drawing.Point(4, 28); // Font dependent
            this.availableComicsTabPage.Name = "availableComicsTabPage";
            this.availableComicsTabPage.Padding = new System.Windows.Forms.Padding(10);
            this.availableComicsTabPage.Size = new System.Drawing.Size(692, 587); // Adjusted
            this.availableComicsTabPage.TabIndex = 0;
            this.availableComicsTabPage.Text = "Available Comics";
            this.availableComicsTabPage.UseVisualStyleBackColor = true;
            this.availableComicsTabPage.BackColor = ModernBaseForm.SecondaryColor;
            // Adjusting moved controls for TabPage context
            this.lblAvailableComics.Dock = System.Windows.Forms.DockStyle.Top;
            // availableComicsFilterPanel is already docked Top
            this.dgvAvailableComics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRentComic.Dock = System.Windows.Forms.DockStyle.Bottom;
            //
            // myRentalsTabPage
            //
            this.myRentalsTabPage.Controls.Add(this.lblMyRentedComicsHeader); // Moved
            this.myRentalsTabPage.Controls.Add(this.dgvMyRentedComics);       // Moved
            this.myRentalsTabPage.Location = new System.Drawing.Point(4, 28);
            this.myRentalsTabPage.Name = "myRentalsTabPage";
            this.myRentalsTabPage.Padding = new System.Windows.Forms.Padding(10);
            this.myRentalsTabPage.Size = new System.Drawing.Size(692, 587); // Adjusted
            this.myRentalsTabPage.TabIndex = 1;
            this.myRentalsTabPage.Text = "My Rentals";
            this.myRentalsTabPage.UseVisualStyleBackColor = true;
            this.myRentalsTabPage.BackColor = ModernBaseForm.SecondaryColor;
            // Adjusting moved controls for TabPage context
            this.lblMyRentedComicsHeader.Dock = System.Windows.Forms.DockStyle.Top; // Changed from Location based
            this.dgvMyRentedComics.Dock = System.Windows.Forms.DockStyle.Fill;    // Changed from Location/Size/Anchor based
            //
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 690);

            // Add controls to mainContentPanel: cmbAdminComicFilterStatus (for admin) and memberViewTabControl (for members)
            // lblAvailableComics, dgvAvailableComics, btnRentComic, lblMyRentedComicsHeader, dgvMyRentedComics are now in TabPages.
            this.mainContentPanel.Controls.Add(this.cmbAdminComicFilterStatus); // Remains for Admin view (Comic Management)
            this.mainContentPanel.Controls.Add(this.memberViewTabControl);     // For Member view

            // Add new panels and existing menu/status strips to the Form's controls
            // Order can be important for docking. mainContentPanel fills remaining space.
            this.Controls.Add(this.mainContentPanel); // Should fill space left by others
            this.Controls.Add(this.leftNavPanel);   // Docks left
            this.Controls.Add(menuStrip1);         // Optional, appears unused
            this.Controls.Add(menuStrip2);         // Main menu, docks top
            this.Controls.Add(this.statusStrip1);   // Status, docks bottom

            this.MainMenuStrip = menuStrip2;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "漫畫租借系統";
            this.Load += new System.EventHandler(this.MainForm_Load);

            // Resume Layout for panels and main form
            this.myRentalsTabPage.ResumeLayout(false);
            //this.myRentalsTabPage.PerformLayout(); // Not needed for docked controls
            this.availableComicsTabPage.ResumeLayout(false);
            //this.availableComicsTabPage.PerformLayout(); // Not needed for docked controls
            this.memberViewTabControl.ResumeLayout(false);

            this.mainContentPanel.ResumeLayout(false);
            this.mainContentPanel.PerformLayout(); // PerformLayout for mainContentPanel if it has non-docked controls like cmbAdminComicFilterStatus
            this.leftNavPanel.ResumeLayout(false);
            // this.leftNavPanel.PerformLayout();

            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.dgvAvailableComics).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRentedComics)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem 檔案ToolStripMenuItem;
        private ToolStripMenuItem 離開ToolStripMenuItem;
        private ToolStripMenuItem 管理ToolStripMenuItem;
        private ToolStripMenuItem 漫畫管理ToolStripMenuItem;
        private ToolStripMenuItem 會員管理ToolStripMenuItem;
        private ToolStripMenuItem 工具ToolStripMenuItem;
        private ToolStripMenuItem 檢視日誌ToolStripMenuItem;
        private ToolStripMenuItem rentalManagementToolStripMenuItem;
        private DataGridView dgvAvailableComics;
        private Label lblAvailableComics;
        private System.Windows.Forms.ToolStripMenuItem 使用者註冊ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelUser;
        private System.Windows.Forms.Button btnRentComic;
        private System.Windows.Forms.Label lblMyRentedComicsHeader; // Added
        private System.Windows.Forms.DataGridView dgvMyRentedComics; // Added
    }
}