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
            工具ToolStripMenuItem = new ToolStripMenuItem();
            檢視日誌ToolStripMenuItem = new ToolStripMenuItem();
            rentalManagementToolStripMenuItem = new ToolStripMenuItem();
            menuStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Location = new Point(0, 28);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(8, 2, 0, 2);
            menuStrip1.Size = new Size(1000, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            menuStrip2.Items.AddRange(new ToolStripItem[] { 檔案ToolStripMenuItem, 管理ToolStripMenuItem, 工具ToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Padding = new Padding(8, 2, 0, 2);
            menuStrip2.Size = new Size(1000, 28);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            // 
            // 檔案ToolStripMenuItem
            // 
            檔案ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 離開ToolStripMenuItem });
            檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            檔案ToolStripMenuItem.Size = new Size(53, 24);
            檔案ToolStripMenuItem.Text = "檔案";
            // 
            // 離開ToolStripMenuItem
            // 
            離開ToolStripMenuItem.Name = "離開ToolStripMenuItem";
            離開ToolStripMenuItem.Size = new Size(110, 24);
            離開ToolStripMenuItem.Text = "離開";
            離開ToolStripMenuItem.Click += 離開ToolStripMenuItem_Click;
            // 
            // 管理ToolStripMenuItem
            // 
            管理ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 漫畫管理ToolStripMenuItem, 會員管理ToolStripMenuItem, rentalManagementToolStripMenuItem });
            管理ToolStripMenuItem.Name = "管理ToolStripMenuItem";
            管理ToolStripMenuItem.Size = new Size(53, 24);
            管理ToolStripMenuItem.Text = "管理";
            // 
            // 漫畫管理ToolStripMenuItem
            // 
            漫畫管理ToolStripMenuItem.Name = "漫畫管理ToolStripMenuItem";
            漫畫管理ToolStripMenuItem.Size = new Size(180, 24);
            漫畫管理ToolStripMenuItem.Text = "漫畫管理";
            漫畫管理ToolStripMenuItem.Click += 漫畫管理ToolStripMenuItem_Click;
            // 
            // 會員管理ToolStripMenuItem
            // 
            會員管理ToolStripMenuItem.Name = "會員管理ToolStripMenuItem";
            會員管理ToolStripMenuItem.Size = new Size(180, 24);
            會員管理ToolStripMenuItem.Text = "會員管理";
            會員管理ToolStripMenuItem.Click += 會員管理ToolStripMenuItem_Click;
            // 
            // 工具ToolStripMenuItem
            // 
            工具ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 檢視日誌ToolStripMenuItem });
            工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            工具ToolStripMenuItem.Size = new Size(53, 24);
            工具ToolStripMenuItem.Text = "工具";
            // 
            // 檢視日誌ToolStripMenuItem
            // 
            檢視日誌ToolStripMenuItem.Name = "檢視日誌ToolStripMenuItem";
            檢視日誌ToolStripMenuItem.Size = new Size(142, 24);
            檢視日誌ToolStripMenuItem.Text = "檢視日誌";
            // 
            // rentalManagementToolStripMenuItem
            // 
            rentalManagementToolStripMenuItem.Name = "rentalManagementToolStripMenuItem";
            rentalManagementToolStripMenuItem.Size = new Size(180, 24);
            rentalManagementToolStripMenuItem.Text = "租借管理";
            rentalManagementToolStripMenuItem.Click += rentalManagementToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 529);
            Controls.Add(menuStrip1);
            Controls.Add(menuStrip2);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 4, 4, 4);
            Name = "MainForm";
            Text = "MainForm";
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
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
    }
}