namespace ComicRentalSystem_14Days.Forms
{
    partial class RentalForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMember;
        private System.Windows.Forms.Label lblComic;
        private System.Windows.Forms.Label lblActualReturnTime;
        private System.Windows.Forms.Label lblRentedComics;

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
            cmbMembers = new ComboBox();
            cmbComics = new ComboBox();
            btnReturn = new Button();
            btnRent = new Button();
            dgvRentedComics = new DataGridView();
            dtpActualReturnTime = new DateTimePicker();
            grpRent = new GroupBox();
            lblMember = new Label();
            lblComic = new Label();
            grpReturn = new GroupBox();
            lblActualReturnTime = new Label();
            lblRentedComics = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).BeginInit();
            grpRent.SuspendLayout();
            grpReturn.SuspendLayout();
            SuspendLayout();
            // 
            // cmbMembers
            // 
            cmbMembers.FormattingEnabled = true;
            cmbMembers.Location = new Point(69, 24);
            cmbMembers.Margin = new Padding(2, 4, 2, 4);
            cmbMembers.Name = "cmbMembers";
            cmbMembers.Size = new Size(137, 28);
            cmbMembers.TabIndex = 0;
            cmbMembers.SelectedIndexChanged += cmbMembers_SelectedIndexChanged;
            // 
            // cmbComics
            // 
            cmbComics.FormattingEnabled = true;
            cmbComics.Location = new Point(273, 24);
            cmbComics.Margin = new Padding(2, 4, 2, 4);
            cmbComics.Name = "cmbComics";
            cmbComics.Size = new Size(137, 28);
            cmbComics.TabIndex = 1;
            // 
            // btnReturn
            // 
            btnReturn.Location = new Point(306, 23);
            btnReturn.Margin = new Padding(2, 4, 2, 4);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(70, 31);
            btnReturn.TabIndex = 2;
            btnReturn.Text = "↩ 歸還";
            btnReturn.UseVisualStyleBackColor = true;
            btnReturn.Click += btnReturn_Click;
            // 
            // btnRent
            // 
            btnRent.Location = new Point(423, 23);
            btnRent.Margin = new Padding(2, 4, 2, 4);
            btnRent.Name = "btnRent";
            btnRent.Size = new Size(59, 32);
            btnRent.TabIndex = 4;
            btnRent.Text = "租借 ⬇";
            btnRent.UseVisualStyleBackColor = true;
            btnRent.Click += btnRent_Click;
            // 
            // dgvRentedComics
            // 
            dgvRentedComics.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRentedComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentedComics.Location = new Point(16, 96);
            dgvRentedComics.Margin = new Padding(2, 4, 2, 4);
            dgvRentedComics.Name = "dgvRentedComics";
            dgvRentedComics.RowHeadersWidth = 51;
            dgvRentedComics.Size = new Size(805, 540);
            dgvRentedComics.TabIndex = 5;
            dgvRentedComics.SelectionChanged += dgvRentedComics_SelectionChanged;
            // 
            // dtpActualReturnTime
            // 
            dtpActualReturnTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dtpActualReturnTime.Format = DateTimePickerFormat.Custom;
            dtpActualReturnTime.Location = new Point(128, 24);
            dtpActualReturnTime.Margin = new Padding(2, 4, 2, 4);
            dtpActualReturnTime.Name = "dtpActualReturnTime";
            dtpActualReturnTime.Size = new Size(171, 27);
            dtpActualReturnTime.TabIndex = 3;
            // 
            // grpRent
            // 
            grpRent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpRent.Controls.Add(lblMember);
            grpRent.Controls.Add(cmbMembers);
            grpRent.Controls.Add(lblComic);
            grpRent.Controls.Add(cmbComics);
            grpRent.Controls.Add(btnRent);
            grpRent.Location = new Point(11, 13);
            grpRent.Margin = new Padding(3, 4, 3, 4);
            grpRent.Name = "grpRent";
            grpRent.Padding = new Padding(3, 4, 3, 4);
            grpRent.Size = new Size(823, 72);
            grpRent.TabIndex = 10;
            grpRent.TabStop = false;
            grpRent.Text = "租借新漫畫";
            // 
            // lblMember
            // 
            lblMember.AutoSize = true;
            lblMember.Location = new Point(13, 29);
            lblMember.Name = "lblMember";
            lblMember.Size = new Size(42, 20);
            lblMember.TabIndex = 6;
            lblMember.Text = "會員:";
            // 
            // lblComic
            // 
            lblComic.AutoSize = true;
            lblComic.Location = new Point(217, 29);
            lblComic.Name = "lblComic";
            lblComic.Size = new Size(42, 20);
            lblComic.TabIndex = 7;
            lblComic.Text = "漫畫:";
            // 
            // grpReturn
            // 
            grpReturn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpReturn.Controls.Add(lblActualReturnTime);
            grpReturn.Controls.Add(dtpActualReturnTime);
            grpReturn.Controls.Add(btnReturn);
            grpReturn.Controls.Add(lblRentedComics);
            grpReturn.Controls.Add(dgvRentedComics);
            grpReturn.Location = new Point(11, 93);
            grpReturn.Margin = new Padding(3, 4, 3, 4);
            grpReturn.Name = "grpReturn";
            grpReturn.Padding = new Padding(3, 4, 3, 4);
            grpReturn.Size = new Size(823, 536);
            grpReturn.TabIndex = 11;
            grpReturn.TabStop = false;
            grpReturn.Text = "歸還已租漫畫";
            // 
            // lblActualReturnTime
            // 
            lblActualReturnTime.AutoSize = true;
            lblActualReturnTime.Location = new Point(16, 29);
            lblActualReturnTime.Name = "lblActualReturnTime";
            lblActualReturnTime.Size = new Size(102, 20);
            lblActualReturnTime.TabIndex = 8;
            lblActualReturnTime.Text = "實際歸還時間:";
            // 
            // lblRentedComics
            // 
            lblRentedComics.AutoSize = true;
            lblRentedComics.Location = new Point(16, 69);
            lblRentedComics.Name = "lblRentedComics";
            lblRentedComics.Size = new Size(102, 20);
            lblRentedComics.TabIndex = 9;
            lblRentedComics.Text = "目前租借記錄:";
            // 
            // RentalForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(851, 643);
            Controls.Add(grpReturn);
            Controls.Add(grpRent);
            Margin = new Padding(2, 4, 2, 4);
            Name = "RentalForm";
            Padding = new Padding(11, 13, 11, 13);
            Text = "租借管理";
            Load += RentalForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).EndInit();
            grpRent.ResumeLayout(false);
            grpRent.PerformLayout();
            grpReturn.ResumeLayout(false);
            grpReturn.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox cmbMembers;
        private System.Windows.Forms.ComboBox cmbComics;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.Button btnRent;
        private System.Windows.Forms.DataGridView dgvRentedComics;
        private System.Windows.Forms.DateTimePicker dtpActualReturnTime;
        private System.Windows.Forms.GroupBox grpRent;
        private System.Windows.Forms.GroupBox grpReturn;
    }
}
