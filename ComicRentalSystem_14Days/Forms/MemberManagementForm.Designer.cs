namespace ComicRentalSystem_14Days.Forms
{
    partial class MemberManagementForm
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
            dgvMembers = new DataGridView();
            panel1 = new Panel();
            btnChangeUserRole = new Button();
            btnRefreshMembers = new Button();
            btnDeleteMember = new Button();
            btnEditMember = new Button();
            btnAddMember = new Button();
            txtSearchMembers = new TextBox();
            btnSearchMembers = new Button();
            btnClearSearchMembers = new Button();
            panelSearch = new Panel();
            ((System.ComponentModel.ISupportInitialize)dgvMembers).BeginInit();
            panel1.SuspendLayout();
            panelSearch.SuspendLayout();
            SuspendLayout();
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMembers.Dock = DockStyle.Fill;
            dgvMembers.Location = new Point(10, 40);
            dgvMembers.Name = "dgvMembers";
            dgvMembers.Size = new Size(680, 292);
            dgvMembers.TabIndex = 0;
            dgvMembers.SelectionChanged += dgvMembers_SelectionChanged;
            panel1.Controls.Add(btnChangeUserRole);
            panel1.Controls.Add(btnRefreshMembers);
            panel1.Controls.Add(btnDeleteMember);
            panel1.Controls.Add(btnEditMember);
            panel1.Controls.Add(btnAddMember);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(10, 332);
            panel1.Name = "panel1";
            panel1.Size = new Size(680, 55);
            panel1.TabIndex = 1;
            btnChangeUserRole.Location = new Point(3, 17);
            btnChangeUserRole.Name = "btnChangeUserRole";
            btnChangeUserRole.Size = new Size(108, 23);
            btnChangeUserRole.Enabled = false;
            btnChangeUserRole.TabIndex = 4;
            btnChangeUserRole.Text = "更改使用者角色";
            btnChangeUserRole.UseVisualStyleBackColor = true;
            btnChangeUserRole.Click += btnChangeUserRole_Click;
            btnRefreshMembers.Location = new Point(380, 17);
            btnRefreshMembers.Name = "btnRefreshMembers";
            btnRefreshMembers.Size = new Size(75, 23);
            btnRefreshMembers.TabIndex = 3;
            btnRefreshMembers.Text = "重新整理";
            btnRefreshMembers.UseVisualStyleBackColor = true;
            btnDeleteMember.Location = new Point(299, 17);
            btnDeleteMember.Name = "btnDeleteMember";
            btnDeleteMember.Size = new Size(75, 23);
            btnDeleteMember.Enabled = false;
            btnDeleteMember.TabIndex = 2;
            btnDeleteMember.Text = "刪除選定會員 🗑";
            btnDeleteMember.UseVisualStyleBackColor = true;
            btnDeleteMember.Click += btnDeleteMember_Click;
            btnEditMember.Location = new Point(198, 17);
            btnEditMember.Name = "btnEditMember";
            btnEditMember.Size = new Size(95, 23);
            btnEditMember.Enabled = false;
            btnEditMember.TabIndex = 1;
            btnEditMember.Text = "編輯選定會員 ✎";
            btnEditMember.UseVisualStyleBackColor = true;
            btnEditMember.Click += btnEditMember_Click;
            btnAddMember.Location = new Point(117, 17);
            btnAddMember.Name = "btnAddMember";
            btnAddMember.Size = new Size(75, 23);
            btnAddMember.TabIndex = 0;
            btnAddMember.Text = "新增會員 +";
            btnAddMember.UseVisualStyleBackColor = true;
            btnAddMember.Click += btnAddMember_Click;
            txtSearchMembers.Location = new Point(12, 3);
            txtSearchMembers.Name = "txtSearchMembers";
            txtSearchMembers.Size = new Size(200, 23);
            txtSearchMembers.TabIndex = 0;
            txtSearchMembers.KeyDown += txtSearchMembers_KeyDown;
            btnSearchMembers.Location = new Point(218, 2);
            btnSearchMembers.Name = "btnSearchMembers";
            btnSearchMembers.Size = new Size(75, 25);
            btnSearchMembers.TabIndex = 1;
            btnSearchMembers.Text = "搜尋";
            btnSearchMembers.UseVisualStyleBackColor = true;
            btnSearchMembers.Click += btnSearchMembers_Click;
            btnClearSearchMembers.Location = new Point(299, 2);
            btnClearSearchMembers.Name = "btnClearSearchMembers";
            btnClearSearchMembers.Size = new Size(75, 25);
            btnClearSearchMembers.TabIndex = 2;
            btnClearSearchMembers.Text = "清除";
            btnClearSearchMembers.UseVisualStyleBackColor = true;
            btnClearSearchMembers.Click += btnClearSearchMembers_Click;
            panelSearch.Controls.Add(txtSearchMembers);
            panelSearch.Controls.Add(btnSearchMembers);
            panelSearch.Controls.Add(btnClearSearchMembers);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(10, 10);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(680, 30);
            panelSearch.TabIndex = 2;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 397);
            Controls.Add(dgvMembers);
            Controls.Add(panel1);
            Controls.Add(panelSearch);
            Name = "MemberManagementForm";
            Text = "會員管理";
            Load += MemberManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMembers).EndInit();
            panel1.ResumeLayout(false);
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvMembers;
        private Panel panel1;
        private Button btnRefreshMembers;
        private Button btnDeleteMember;
        private Button btnEditMember;
        private Button btnAddMember;
        private Button btnChangeUserRole;
        private Panel panelSearch;
        private TextBox txtSearchMembers;
        private Button btnSearchMembers;
        private Button btnClearSearchMembers;
    }
}