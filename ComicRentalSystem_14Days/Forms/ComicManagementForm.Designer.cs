namespace ComicRentalSystem_14Days.Forms
{
    partial class ComicManagementForm
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
            dgvComics = new DataGridView();
            panel1 = new Panel();
            btnRefresh = new Button();
            btnDeleteComic = new Button();
            btnEditComic = new Button();
            btnAddComic = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvComics).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvComics
            // 
            dgvComics.AllowUserToAddRows = false;
            dgvComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvComics.Dock = DockStyle.Fill;
            dgvComics.Location = new Point(0, 0);
            dgvComics.MultiSelect = false;
            dgvComics.Name = "dgvComics";
            dgvComics.ReadOnly = true;
            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.Size = new Size(800, 450);
            dgvComics.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(btnRefresh);
            panel1.Controls.Add(btnDeleteComic);
            panel1.Controls.Add(btnEditComic);
            panel1.Controls.Add(btnAddComic);
            panel1.Location = new Point(179, 399);
            panel1.Name = "panel1";
            panel1.Size = new Size(449, 39);
            panel1.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.AutoSize = true;
            btnRefresh.Location = new Point(346, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(83, 30);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "重新整理";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnDeleteComic
            // 
            btnDeleteComic.AutoSize = true;
            btnDeleteComic.Location = new Point(238, 4);
            btnDeleteComic.Name = "btnDeleteComic";
            btnDeleteComic.Size = new Size(115, 30);
            btnDeleteComic.TabIndex = 2;
            btnDeleteComic.Text = "刪除選定漫畫";
            btnDeleteComic.UseVisualStyleBackColor = true;
            this.btnDeleteComic.Click += new System.EventHandler(this.btnDeleteComic_Click);
            // 
            // btnEditComic
            // 
            btnEditComic.AutoSize = true;
            btnEditComic.Location = new Point(129, 4);
            btnEditComic.Name = "btnEditComic";
            btnEditComic.Size = new Size(115, 30);
            btnEditComic.TabIndex = 1;
            btnEditComic.Text = "編輯選定漫畫";
            btnEditComic.UseVisualStyleBackColor = true;
            // 
            // btnAddComic
            // 
            btnAddComic.AutoSize = true;
            btnAddComic.Location = new Point(3, 4);
            btnAddComic.Name = "btnAddComic";
            btnAddComic.Size = new Size(120, 30);
            btnAddComic.TabIndex = 0;
            btnAddComic.Text = "新增漫畫";
            btnAddComic.UseVisualStyleBackColor = true;
            btnAddComic.Click += btnAddComic_Click;
            // 
            // ComicManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(dgvComics);
            Name = "ComicManagementForm";
            Text = "ComicManagementForm";
            ((System.ComponentModel.ISupportInitialize)dgvComics).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvComics;
        private Panel panel1;
        private Button btnRefresh;
        private Button btnDeleteComic;
        private Button btnEditComic;
        private Button btnAddComic;
    }
}