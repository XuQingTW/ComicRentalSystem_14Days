namespace ComicRentalSystem_14Days.Forms
{
    partial class ComicEditForm
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
            lblTitle = new Label();
            txtTitle = new TextBox();
            lblAuthor = new Label();
            txtAuthor = new TextBox();
            lblIsbn = new Label();
            txtIsbn = new TextBox();
            lblGenre = new Label();
            txtGenre = new TextBox();
            chkIsRented = new CheckBox();
            btnSave = new Button();
            btnCancel = new Button();
            gbComicDetails = new GroupBox();
            lblCoverImage = new Label();
            pbCoverPreview = new PictureBox();
            btnBrowseCover = new Button();
            gbStatus = new GroupBox();
            gbComicDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCoverPreview).BeginInit();
            gbStatus.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Location = new Point(15, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(95, 23);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "書名:";
            lblTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtTitle
            // 
            txtTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTitle.Location = new Point(120, 30);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(315, 23);
            txtTitle.TabIndex = 1;
            txtTitle.Validating += txtTitle_Validating;
            // 
            // lblAuthor
            // 
            lblAuthor.Location = new Point(15, 60);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(95, 23);
            lblAuthor.TabIndex = 2;
            lblAuthor.Text = "作者:";
            lblAuthor.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAuthor
            // 
            txtAuthor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAuthor.Location = new Point(120, 60);
            txtAuthor.Name = "txtAuthor";
            txtAuthor.Size = new Size(315, 23);
            txtAuthor.TabIndex = 3;
            txtAuthor.Validating += txtAuthor_Validating;
            // 
            // lblIsbn
            // 
            lblIsbn.Location = new Point(15, 90);
            lblIsbn.Name = "lblIsbn";
            lblIsbn.Size = new Size(95, 23);
            lblIsbn.TabIndex = 4;
            lblIsbn.Text = "ISBN:";
            lblIsbn.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtIsbn
            // 
            txtIsbn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtIsbn.Location = new Point(120, 90);
            txtIsbn.Name = "txtIsbn";
            txtIsbn.Size = new Size(315, 23);
            txtIsbn.TabIndex = 5;
            txtIsbn.Validating += txtIsbn_Validating;
            // 
            // lblGenre
            // 
            lblGenre.Location = new Point(15, 120);
            lblGenre.Name = "lblGenre";
            lblGenre.Size = new Size(95, 23);
            lblGenre.TabIndex = 6;
            lblGenre.Text = "類型:";
            lblGenre.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtGenre
            // 
            txtGenre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtGenre.Location = new Point(120, 120);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(315, 23);
            txtGenre.TabIndex = 7;
            txtGenre.Validating += txtGenre_Validating;
            // 
            // chkIsRented
            // 
            chkIsRented.AutoSize = true;
            chkIsRented.Enabled = false;
            chkIsRented.Location = new Point(15, 25);
            chkIsRented.Name = "chkIsRented";
            chkIsRented.Size = new Size(65, 19);
            chkIsRented.TabIndex = 0;
            chkIsRented.Text = "已租借";
            chkIsRented.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(279, 377);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(90, 30);
            btnSave.TabIndex = 2;
            btnSave.Text = "儲存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(375, 377);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 30);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // gbComicDetails
            // 
            gbComicDetails.Controls.Add(lblTitle);
            gbComicDetails.Controls.Add(txtTitle);
            gbComicDetails.Controls.Add(lblAuthor);
            gbComicDetails.Controls.Add(txtAuthor);
            gbComicDetails.Controls.Add(lblIsbn);
            gbComicDetails.Controls.Add(txtIsbn);
            gbComicDetails.Controls.Add(lblGenre);
            gbComicDetails.Controls.Add(txtGenre);
            gbComicDetails.Controls.Add(lblCoverImage);
            gbComicDetails.Controls.Add(pbCoverPreview);
            gbComicDetails.Controls.Add(btnBrowseCover);
            gbComicDetails.Location = new Point(15, 15);
            gbComicDetails.Name = "gbComicDetails";
            gbComicDetails.Padding = new Padding(10);
            gbComicDetails.Size = new Size(450, 270);
            gbComicDetails.TabIndex = 0;
            gbComicDetails.TabStop = false;
            gbComicDetails.Text = "漫畫資訊";
            // 
            // lblCoverImage
            // 
            lblCoverImage.Location = new Point(15, 155);
            lblCoverImage.Name = "lblCoverImage";
            lblCoverImage.Size = new Size(95, 23);
            lblCoverImage.TabIndex = 8;
            lblCoverImage.Text = "封面圖片:";
            lblCoverImage.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pbCoverPreview
            // 
            pbCoverPreview.Location = new Point(120, 155);
            pbCoverPreview.Name = "pbCoverPreview";
            pbCoverPreview.Size = new Size(80, 100);
            pbCoverPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pbCoverPreview.TabIndex = 9;
            pbCoverPreview.TabStop = false;
            // 
            // btnBrowseCover
            // 
            btnBrowseCover.Location = new Point(210, 190);
            btnBrowseCover.Name = "btnBrowseCover";
            btnBrowseCover.Size = new Size(90, 25);
            btnBrowseCover.TabIndex = 10;
            btnBrowseCover.Text = "選擇圖片";
            btnBrowseCover.UseVisualStyleBackColor = true;
            btnBrowseCover.Click += btnBrowseCover_Click;
            // 
            // gbStatus
            // 
            gbStatus.Controls.Add(chkIsRented);
            gbStatus.Location = new Point(15, 295);
            gbStatus.Name = "gbStatus";
            gbStatus.Padding = new Padding(10);
            gbStatus.Size = new Size(450, 60);
            gbStatus.TabIndex = 1;
            gbStatus.TabStop = false;
            gbStatus.Text = "租借狀態";
            // 
            // ComicEditForm
            // 
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            CancelButton = btnCancel;
            ClientSize = new Size(480, 450);
            Controls.Add(gbComicDetails);
            Controls.Add(gbStatus);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Name = "ComicEditForm";
            Padding = new Padding(15);
            gbComicDetails.ResumeLayout(false);
            gbComicDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbCoverPreview).EndInit();
            gbStatus.ResumeLayout(false);
            gbStatus.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Label lblIsbn;
        private System.Windows.Forms.TextBox txtIsbn;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.TextBox txtGenre;
        private System.Windows.Forms.CheckBox chkIsRented;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbComicDetails;
        private System.Windows.Forms.GroupBox gbStatus;
        private System.Windows.Forms.Label lblCoverImage;
        private System.Windows.Forms.PictureBox pbCoverPreview;
        private System.Windows.Forms.Button btnBrowseCover;
    }
}