// In ComicRentalSystem_14Days/MainForm.Designer.cs

namespace ComicRentalSystem_14Days
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            menuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAvailableComics).BeginInit();
            this.statusStrip1.SuspendLayout(); // Added
            SuspendLayout();
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
            // this.btnRentComic = new System.Windows.Forms.Button();
            //
            this.lblMyRentedComicsHeader = new System.Windows.Forms.Label();
            this.dgvMyRentedComics = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRentedComics)).BeginInit();
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
            dgvAvailableComics.AllowUserToAddRows = false;
            dgvAvailableComics.AllowUserToDeleteRows = false;
            dgvAvailableComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            // dgvAvailableComics.Dock = DockStyle.Fill; // Changed to allow button placement
            // Adjusted Location and Size to accommodate statusStrip1 and btnRentComic
            dgvAvailableComics.Location = new Point(0, 86);
            dgvAvailableComics.Margin = new Padding(4);
            dgvAvailableComics.Name = "dgvAvailableComics";
            dgvAvailableComics.ReadOnly = true;
            dgvAvailableComics.RowHeadersWidth = 51;
            // Size will be adjusted below, or use Anchor properties for responsiveness
            dgvAvailableComics.Size = new Size(900, 355); // Reduced height to make space for button
            dgvAvailableComics.TabIndex = 2;
            // 
            // lblAvailableComics
            // 
            lblAvailableComics.AutoSize = true;
            lblAvailableComics.Dock = DockStyle.Top;
            lblAvailableComics.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            lblAvailableComics.Location = new Point(0, 51);
            lblAvailableComics.Margin = new Padding(4, 0, 4, 0);
            lblAvailableComics.Name = "lblAvailableComics";
            lblAvailableComics.Padding = new Padding(5);
            lblAvailableComics.Size = new Size(182, 35);
            lblAvailableComics.TabIndex = 3; // Changed from 3 to 4, as statusStrip will be 3
            lblAvailableComics.Text = "目前可借閱的漫畫";
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
            // this.btnRentComic.Location = new System.Drawing.Point(12, 445);
            // this.btnRentComic.Name = "btnRentComic";
            // this.btnRentComic.Size = new System.Drawing.Size(120, 30);
            // this.btnRentComic.TabIndex = 5;
            // this.btnRentComic.Text = "租借";
            // this.btnRentComic.UseVisualStyleBackColor = true;
            // this.btnRentComic.Visible = false;
            // this.btnRentComic.Enabled = false;
            // this.btnRentComic.Click += new System.EventHandler(this.btnRentComic_Click);
            //
            // lblMyRentedComicsHeader
            //
            this.lblMyRentedComicsHeader.AutoSize = true;
            this.lblMyRentedComicsHeader.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMyRentedComicsHeader.Location = new System.Drawing.Point(12, 485); // Positioned below btnRentComic
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
            this.dgvMyRentedComics.Location = new System.Drawing.Point(12, 510); // Positioned below lblMyRentedComicsHeader
            this.dgvMyRentedComics.Name = "dgvMyRentedComics";
            this.dgvMyRentedComics.ReadOnly = true;
            this.dgvMyRentedComics.RowHeadersWidth = 51;
            this.dgvMyRentedComics.RowTemplate.Height = 27; // Standard height
            this.dgvMyRentedComics.Size = new System.Drawing.Size(876, 150); // Takes most of the width, 150px height
            this.dgvMyRentedComics.TabIndex = 7;
            this.dgvMyRentedComics.Visible = false;
            //
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 690); // Increased client height to accommodate new controls + status bar
            Controls.Add(this.lblMyRentedComicsHeader); // Added
            Controls.Add(this.dgvMyRentedComics); // Added
            // Controls.Add(this.btnRentComic);
            Controls.Add(dgvAvailableComics);
            Controls.Add(lblAvailableComics);
            Controls.Add(menuStrip1); // This menuStrip1 seems to be secondary or unused for items
            Controls.Add(menuStrip2); // This is the main menuStrip
            Controls.Add(this.statusStrip1); // Added statusStrip1
            MainMenuStrip = menuStrip2; // Changed to menuStrip2 as it contains the items
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "漫畫租借系統";
            Load += MainForm_Load;
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAvailableComics).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyRentedComics)).EndInit(); // Added
            ResumeLayout(false);
            PerformLayout();
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
        // private System.Windows.Forms.Button btnRentComic;
        private System.Windows.Forms.Label lblMyRentedComicsHeader; // Added
        private System.Windows.Forms.DataGridView dgvMyRentedComics; // Added
    }
}