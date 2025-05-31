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
            txtSearchComics = new TextBox();
            btnSearchComics = new Button();
            btnClearSearchComics = new Button();
            panelSearch = new Panel(); // Panel for search controls
            ((System.ComponentModel.ISupportInitialize)dgvComics).BeginInit();
            panel1.SuspendLayout();
            panelSearch.SuspendLayout();
            SuspendLayout();
            // 
            // panelSearch
            //
            panelSearch.Controls.Add(txtSearchComics);
            panelSearch.Controls.Add(btnSearchComics);
            panelSearch.Controls.Add(btnClearSearchComics);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(0, 0);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(700, 30);
            panelSearch.TabIndex = 2;
            //
            // txtSearchComics
            //
            txtSearchComics.Location = new Point(12, 3);
            txtSearchComics.Name = "txtSearchComics";
            txtSearchComics.Size = new Size(200, 23);
            txtSearchComics.TabIndex = 0;
            //
            // btnSearchComics
            //
            btnSearchComics.Location = new Point(218, 2);
            btnSearchComics.Name = "btnSearchComics";
            btnSearchComics.Size = new Size(75, 25);
            btnSearchComics.TabIndex = 1;
            btnSearchComics.Text = "搜尋";
            btnSearchComics.UseVisualStyleBackColor = true;
            // IMPORTANT: btnSearchComics.Click event will be wired up in the .cs file if not already, or manually ensure it's done if needed.
            // Usually, the designer handles this. For this tool, we assume event handlers are connected in the main .cs file or via InitializeComponent extensions.
            // btnSearchComics.Click += new System.EventHandler(this.btnSearchComics_Click);
            //
            // btnClearSearchComics
            //
            btnClearSearchComics.Location = new Point(299, 2);
            btnClearSearchComics.Name = "btnClearSearchComics";
            btnClearSearchComics.Size = new Size(75, 25);
            btnClearSearchComics.TabIndex = 2;
            btnClearSearchComics.Text = "清除";
            btnClearSearchComics.UseVisualStyleBackColor = true;
            // btnClearSearchComics.Click += new System.EventHandler(this.btnClearSearchComics_Click);
            //
            // dgvComics
            // 
            dgvComics.AllowUserToAddRows = false;
            dgvComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvComics.Dock = DockStyle.Fill;
            dgvComics.Location = new Point(0, 30); // Below panelSearch
            dgvComics.MultiSelect = false;
            dgvComics.Name = "dgvComics";
            dgvComics.ReadOnly = true;
            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // Adjusted height: ClientHeight - panelSearch.Height - panel1.Height (approx)
            // The form's ClientSize might need adjustment if panel1's height is fixed.
            // For now, let dgvComics fill the space between panelSearch and panel1.
            dgvComics.Size = new Size(700, 322);
            dgvComics.TabIndex = 0;
            // dgvComics.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComics_CellDoubleClick); // This is already present
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(btnRefresh);
            panel1.Controls.Add(btnDeleteComic);
            panel1.Controls.Add(btnEditComic);
            panel1.Controls.Add(btnAddComic);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 352);
            panel1.Name = "panel1";
            panel1.Size = new Size(700, 36);
            panel1.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.AutoSize = true;
            btnRefresh.Location = new Point(328, 7);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(73, 26);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "重新整理";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnDeleteComic
            // 
            btnDeleteComic.AutoSize = true;
            btnDeleteComic.Location = new Point(221, 7);
            btnDeleteComic.Name = "btnDeleteComic";
            btnDeleteComic.Size = new Size(89, 25);
            btnDeleteComic.TabIndex = 2;
            btnDeleteComic.Text = "刪除選定漫畫";
            btnDeleteComic.UseVisualStyleBackColor = true;
            btnDeleteComic.Click += btnDeleteComic_Click;
            // 
            // btnEditComic
            // 
            btnEditComic.AutoSize = true;
            btnEditComic.Location = new Point(114, 4);
            btnEditComic.Name = "btnEditComic";
            btnEditComic.Size = new Size(101, 26);
            btnEditComic.TabIndex = 1;
            btnEditComic.Text = "編輯選定漫畫";
            btnEditComic.UseVisualStyleBackColor = true;
            btnEditComic.Click += btnEditComic_Click;
            // 
            // btnAddComic
            // 
            btnAddComic.AutoSize = true;
            btnAddComic.Location = new Point(3, 4);
            btnAddComic.Name = "btnAddComic";
            btnAddComic.Size = new Size(105, 26);
            btnAddComic.TabIndex = 0;
            btnAddComic.Text = "新增漫畫";
            btnAddComic.UseVisualStyleBackColor = true;
            btnAddComic.Click += btnAddComic_Click;
            // 
            // ComicManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            // ClientSize might need adjustment based on fixed heights of panels.
            // Original: 700, 397. panelSearch H=30. panel1 H=36 (AutoSize).
            // New dgvComics Y = 30. panel1 Y = 352. So, 352+36 = 388. ClientSize should be ~700, 388.
            ClientSize = new Size(700, 388);
            Controls.Add(dgvComics);
            Controls.Add(panel1);
            Controls.Add(panelSearch);
            Name = "ComicManagementForm";
            Text = "漫畫管理"; // Changed form title
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