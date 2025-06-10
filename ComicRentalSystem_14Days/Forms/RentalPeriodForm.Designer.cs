namespace ComicRentalSystem_14Days.Forms
{
    partial class RentalPeriodForm
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
            monthCalendarRental = new MonthCalendar();
            btnConfirmRental = new Button();
            btnCancelRental = new Button();
            lblInfo = new Label();
            SuspendLayout();
            this.monthCalendarRental.Location = new System.Drawing.Point((280 - 227) / 2 + 15 , 65); 
            this.monthCalendarRental.MaxSelectionCount = 1;
            this.monthCalendarRental.Name = "monthCalendarRental";
            this.monthCalendarRental.TabIndex = 0;
            this.monthCalendarRental.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnConfirmRental.Location = new System.Drawing.Point( (280 / 2) - 100 - 5 + 15 , 240); 
            this.btnConfirmRental.Name = "btnConfirmRental";
            this.btnConfirmRental.Size = new System.Drawing.Size(100, 35);
            this.btnConfirmRental.TabIndex = 1;
            this.btnConfirmRental.Text = "確認"; 
            this.btnConfirmRental.UseVisualStyleBackColor = true;
            this.btnConfirmRental.Click += new System.EventHandler(this.btnConfirmRental_Click);
            this.btnConfirmRental.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelRental.Location = new System.Drawing.Point( (280 / 2) + 5 + 15, 240); 
            this.btnCancelRental.Name = "btnCancelRental";
            this.btnCancelRental.Size = new System.Drawing.Size(100, 35);
            this.btnCancelRental.TabIndex = 2;
            this.btnCancelRental.Text = "取消"; 
            this.btnCancelRental.UseVisualStyleBackColor = true;
            this.btnCancelRental.Click += new System.EventHandler(this.btnCancelRental_Click);
            this.btnCancelRental.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblInfo.Location = new System.Drawing.Point(15, 15);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(250, 45);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "請選擇歸還日期。"; 
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.AcceptButton = this.btnConfirmRental;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelRental;
            this.ClientSize = new System.Drawing.Size(280, 290);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnCancelRental);
            this.Controls.Add(this.btnConfirmRental);
            this.Controls.Add(this.monthCalendarRental);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.Name = "RentalPeriodForm";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "選擇歸還日期"; 
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendarRental;
        private System.Windows.Forms.Button btnConfirmRental;
        private System.Windows.Forms.Button btnCancelRental;
        private System.Windows.Forms.Label lblInfo;
    }
}
