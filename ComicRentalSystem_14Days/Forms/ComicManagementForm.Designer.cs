namespace ComicRentalSystem_14Days.Forms
{
    partial class ComicManagementForm
    {
        private System.ComponentModel.IContainer components = null;

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
            dgvComics = new DataGridView();
            panel1 = new Panel();
            btnRefresh = new Button();
            btnDeleteComic = new Button();
            btnEditComic = new Button();
            btnAddComic = new Button();
            txtSearchComics = new TextBox();
            btnSearchComics = new Button();
            btnClearSearchComics = new Button();
            panelSearch = new Panel();
            ((System.ComponentModel.ISupportInitialize)dgvComics).BeginInit();
            panel1.SuspendLayout();
            panelSearch.SuspendLayout();
            SuspendLayout();
            dgvComics.AllowUserToAddRows = false;
            dgvComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvComics.Dock = DockStyle.Fill;
            dgvComics.Location = new Point(10, 40);
            dgvComics.MultiSelect = false;
            dgvComics.Name = "dgvComics";
            dgvComics.ReadOnly = true;
            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.Size = new Size(680, 305);
            dgvComics.TabIndex = 0;
            panel1.AutoSize = true;
            panel1.Controls.Add(btnRefresh);
            panel1.Controls.Add(btnDeleteComic);
            panel1.Controls.Add(btnEditComic);
            panel1.Controls.Add(btnAddComic);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(10, 345);
            panel1.Name = "panel1";
            panel1.Size = new Size(680, 33);
            panel1.TabIndex = 1;
            btnRefresh.AutoSize = true;
            btnRefresh.Location = new Point(319, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(69, 25);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "重新整理";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            btnDeleteComic.AutoSize = true;
            btnDeleteComic.Location = new Point(218, 4);
            btnDeleteComic.Name = "btnDeleteComic";
            btnDeleteComic.Size = new Size(95, 25);
            btnDeleteComic.TabIndex = 2;
            btnDeleteComic.Text = "刪除選定漫畫";
            btnDeleteComic.UseVisualStyleBackColor = true;
            btnDeleteComic.Click += btnDeleteComic_Click;
            btnEditComic.AutoSize = true;
            btnEditComic.Location = new Point(111, 4);
            btnEditComic.Name = "btnEditComic";
            btnEditComic.Size = new Size(101, 26);
            btnEditComic.TabIndex = 1;
            btnEditComic.Text = "編輯選定漫畫";
            btnEditComic.UseVisualStyleBackColor = true;
            btnEditComic.Click += btnEditComic_Click;
            btnAddComic.AutoSize = true;
            btnAddComic.Location = new Point(3, 4);
            btnAddComic.Name = "btnAddComic";
            btnAddComic.Size = new Size(105, 26);
            btnAddComic.TabIndex = 0;
            btnAddComic.Text = "新增漫畫";
            btnAddComic.UseVisualStyleBackColor = true;
            btnAddComic.Click += btnAddComic_Click;
            txtSearchComics.Location = new Point(12, 3);
            txtSearchComics.Name = "txtSearchComics";
            txtSearchComics.Size = new Size(200, 23);
            txtSearchComics.TabIndex = 0;
            btnSearchComics.Location = new Point(218, 2);
            btnSearchComics.Name = "btnSearchComics";
            btnSearchComics.Size = new Size(75, 25);
            btnSearchComics.TabIndex = 1;
            btnSearchComics.Text = "搜尋";
            btnSearchComics.UseVisualStyleBackColor = true;
            btnSearchComics.Click += btnSearchComics_Click;
            btnClearSearchComics.Location = new Point(299, 2);
            btnClearSearchComics.Name = "btnClearSearchComics";
            btnClearSearchComics.Size = new Size(75, 25);
            btnClearSearchComics.TabIndex = 2;
            btnClearSearchComics.Text = "清除";
            btnClearSearchComics.UseVisualStyleBackColor = true;
            btnClearSearchComics.Click += btnClearSearchComics_Click;
            panelSearch.Controls.Add(txtSearchComics);
            panelSearch.Controls.Add(btnSearchComics);
            panelSearch.Controls.Add(btnClearSearchComics);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(10, 10);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(680, 30);
            panelSearch.TabIndex = 2;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 388);
            Controls.Add(dgvComics);
            Controls.Add(panel1);
            Controls.Add(panelSearch);
            Name = "ComicManagementForm";
            Text = "漫畫管理";
            Load += ComicManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvComics).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
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
        private Panel panelSearch;
        private TextBox txtSearchComics;
        private Button btnSearchComics;
        private Button btnClearSearchComics;
    }
}