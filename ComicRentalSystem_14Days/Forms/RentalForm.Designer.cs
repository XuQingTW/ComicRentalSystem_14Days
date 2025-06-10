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
            grpReturn = new GroupBox();

            lblMember = new Label();
            lblComic = new Label();
            lblActualReturnTime = new Label();
            lblRentedComics = new Label();

            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).BeginInit();
            SuspendLayout();
            grpRent.SuspendLayout();
            grpReturn.SuspendLayout();
            lblMember.AutoSize = true;
            lblMember.Location = new Point(11, 22);
            lblMember.Name = "lblMember";
            lblMember.Size = new Size(44, 15);
            lblMember.TabIndex = 6;
            lblMember.Text = "會員:";
            cmbMembers.FormattingEnabled = true;
            cmbMembers.Location = new Point(60, 18);
            cmbMembers.Margin = new Padding(2, 3, 2, 3);
            cmbMembers.Name = "cmbMembers";
            cmbMembers.Size = new Size(120, 23);
            cmbMembers.TabIndex = 0;
            cmbMembers.SelectedIndexChanged += cmbMembers_SelectedIndexChanged;
            lblComic.AutoSize = true;
            lblComic.Location = new Point(190, 22);
            lblComic.Name = "lblComic";
            lblComic.Size = new Size(44, 15);
            lblComic.TabIndex = 7;
            lblComic.Text = "漫畫:";
            cmbComics.FormattingEnabled = true;
            cmbComics.Location = new Point(239, 18);
            cmbComics.Margin = new Padding(2, 3, 2, 3);
            cmbComics.Name = "cmbComics";
            cmbComics.Size = new Size(120, 23); 
            cmbComics.TabIndex = 1;
            btnRent.Location = new Point(370, 17);
            btnRent.Margin = new Padding(2, 3, 2, 3);
            btnRent.Name = "btnRent";
            btnRent.Size = new Size(52, 24);
            btnRent.TabIndex = 4;
            btnRent.Text = "租借";
            btnRent.UseVisualStyleBackColor = true;
            btnRent.Click += btnRent_Click;
            lblActualReturnTime.AutoSize = true;
            lblActualReturnTime.Location = new Point(14, 22);
            lblActualReturnTime.Name = "lblActualReturnTime";
            lblActualReturnTime.Size = new Size(92, 15);
            lblActualReturnTime.TabIndex = 8;
            lblActualReturnTime.Text = "實際歸還時間:";
            dtpActualReturnTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dtpActualReturnTime.Format = DateTimePickerFormat.Custom;
            dtpActualReturnTime.Location = new Point(112, 18);
            dtpActualReturnTime.Margin = new Padding(2, 3, 2, 3);
            dtpActualReturnTime.Name = "dtpActualReturnTime";
            dtpActualReturnTime.Size = new Size(150, 23); 
            dtpActualReturnTime.TabIndex = 3;
            btnReturn.Location = new Point(268, 17);
            btnReturn.Margin = new Padding(2, 3, 2, 3);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(52, 23);
            btnReturn.TabIndex = 2;
            btnReturn.Text = "歸還";
            btnReturn.UseVisualStyleBackColor = true;
            btnReturn.Click += btnReturn_Click;
            lblRentedComics.AutoSize = true;
            lblRentedComics.Location = new Point(14, 52);
            lblRentedComics.Name = "lblRentedComics";
            lblRentedComics.Size = new Size(92, 15);
            lblRentedComics.TabIndex = 9;
            lblRentedComics.Text = "目前租借記錄:";
            dgvRentedComics.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            dgvRentedComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentedComics.Location = new Point(14, 72);
            dgvRentedComics.Margin = new Padding(2, 3, 2, 3);
            dgvRentedComics.Name = "dgvRentedComics";
            dgvRentedComics.RowHeadersWidth = 51;
            dgvRentedComics.Size = new Size(704, 405);
            dgvRentedComics.TabIndex = 5;
            dgvRentedComics.SelectionChanged += dgvRentedComics_SelectionChanged;

            grpRent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpRent.Location = new Point(10, 10);
            grpRent.Name = "grpRent";
            grpRent.Size = new Size(720, 54);
            grpRent.TabIndex = 10;
            grpRent.TabStop = false;
            grpRent.Text = "租借新漫畫";
            grpRent.Controls.Add(lblMember);
            grpRent.Controls.Add(cmbMembers);
            grpRent.Controls.Add(lblComic);
            grpRent.Controls.Add(cmbComics);
            grpRent.Controls.Add(btnRent);

            grpReturn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpReturn.Location = new Point(10, 70);
            grpReturn.Name = "grpReturn";
            grpReturn.Size = new Size(720, 402);
            grpReturn.TabIndex = 11;
            grpReturn.TabStop = false;
            grpReturn.Text = "歸還已租漫畫";
            grpReturn.Controls.Add(lblActualReturnTime);
            grpReturn.Controls.Add(dtpActualReturnTime);
            grpReturn.Controls.Add(btnReturn);
            grpReturn.Controls.Add(lblRentedComics);
            grpReturn.Controls.Add(dgvRentedComics);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(745, 482);
            Controls.Add(grpReturn);
            Controls.Add(grpRent);
            Margin = new Padding(2, 3, 2, 3);
            Name = "RentalForm";
            Text = "租借管理";
            Load += RentalForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRentedComics).EndInit();
            grpReturn.ResumeLayout(false);
            grpReturn.PerformLayout();
            grpRent.ResumeLayout(false);
            grpRent.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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