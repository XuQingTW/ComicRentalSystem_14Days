namespace ComicRentalSystem_14Days.Forms
{
    partial class RentalPeriodForm
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
            monthCalendarRental = new MonthCalendar();
            btnConfirmRental = new Button();
            btnCancelRental = new Button();
            lblInfo = new Label();
            SuspendLayout();
            // 
            // monthCalendarRental
            // 
            monthCalendarRental.Location = new Point(18, 50);
            monthCalendarRental.MaxSelectionCount = 1;
            monthCalendarRental.Name = "monthCalendarRental";
            monthCalendarRental.TabIndex = 0;
            // 
            // btnConfirmRental
            // 
            btnConfirmRental.Location = new Point(18, 225);
            btnConfirmRental.Name = "btnConfirmRental";
            btnConfirmRental.Size = new Size(100, 30);
            btnConfirmRental.TabIndex = 1;
            btnConfirmRental.Text = "Confirm";
            btnConfirmRental.UseVisualStyleBackColor = true;
            btnConfirmRental.Click += btnConfirmRental_Click;
            // 
            // btnCancelRental
            // 
            btnCancelRental.Location = new Point(145, 225);
            btnCancelRental.Name = "btnCancelRental";
            btnCancelRental.Size = new Size(100, 30);
            btnCancelRental.TabIndex = 2;
            btnCancelRental.Text = "Cancel";
            btnCancelRental.UseVisualStyleBackColor = true;
            btnCancelRental.Click += btnCancelRental_Click;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(12, 26);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(272, 15);
            lblInfo.TabIndex = 3;
            lblInfo.Text = "Select a return date (Min 3 days, Max 1 month).";
            // 
            // RentalPeriodForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(309, 283);
            Controls.Add(lblInfo);
            Controls.Add(btnCancelRental);
            Controls.Add(btnConfirmRental);
            Controls.Add(monthCalendarRental);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RentalPeriodForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Rental Period";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendarRental;
        private System.Windows.Forms.Button btnConfirmRental;
        private System.Windows.Forms.Button btnCancelRental;
        private System.Windows.Forms.Label lblInfo;
    }
}
