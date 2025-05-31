namespace ComicRentalSystem_14Days.Forms
{
    partial class MemberEditForm
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
            this.lblName = new System.Windows.Forms.Label();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.btnSaveMember = new System.Windows.Forms.Button();
            this.btnCancelMember = new System.Windows.Forms.Button();
            this.gbMemberDetails = new System.Windows.Forms.GroupBox();

            this.gbMemberDetails.SuspendLayout();
            this.SuspendLayout();
            //
            // gbMemberDetails
            //
            this.gbMemberDetails.Controls.Add(this.lblName);
            this.gbMemberDetails.Controls.Add(this.txtName);
            this.gbMemberDetails.Controls.Add(this.lblPhoneNumber);
            this.gbMemberDetails.Controls.Add(this.txtPhoneNumber);
            this.gbMemberDetails.Location = new System.Drawing.Point(15, 15);
            this.gbMemberDetails.Name = "gbMemberDetails";
            this.gbMemberDetails.Padding = new System.Windows.Forms.Padding(10);
            this.gbMemberDetails.Size = new System.Drawing.Size(390, 120);
            this.gbMemberDetails.TabIndex = 0;
            this.gbMemberDetails.TabStop = false;
            this.gbMemberDetails.Text = "Member Details";
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(15, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(95, 23);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.Location = new System.Drawing.Point(15, 60);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(95, 23);
            this.lblPhoneNumber.TabIndex = 2;
            this.lblPhoneNumber.Text = "Phone:";
            this.lblPhoneNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtName.Location = new System.Drawing.Point(120, 30); // Adjusted X
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(250, 23);
            this.txtName.TabIndex = 1;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.txtPhoneNumber.Location = new System.Drawing.Point(120, 60); // Adjusted X
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(250, 23);
            this.txtPhoneNumber.TabIndex = 3;
            this.txtPhoneNumber.Validating += new System.ComponentModel.CancelEventHandler(this.txtPhoneNumber_Validating);
            // 
            // btnSaveMember
            // 
            this.btnSaveMember.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.btnSaveMember.Location = new System.Drawing.Point(225, 155);
            this.btnSaveMember.Name = "btnSaveMember";
            this.btnSaveMember.Size = new System.Drawing.Size(90, 30);
            this.btnSaveMember.TabIndex = 1; // Form-level tab index
            this.btnSaveMember.Text = "Save";
            this.btnSaveMember.UseVisualStyleBackColor = true;
            this.btnSaveMember.Click += new System.EventHandler(this.btnSaveMember_Click);
            // 
            // btnCancelMember
            // 
            this.btnCancelMember.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.btnCancelMember.Location = new System.Drawing.Point(321, 155); // Adjusted X to be next to Save
            this.btnCancelMember.Name = "btnCancelMember";
            this.btnCancelMember.Size = new System.Drawing.Size(90, 30);
            this.btnCancelMember.TabIndex = 2; // Form-level tab index
            this.btnCancelMember.Text = "Cancel";
            this.btnCancelMember.UseVisualStyleBackColor = true;
            this.btnCancelMember.Click += new System.EventHandler(this.btnCancelMember_Click);
            // 
            // MemberEditForm
            // 
            this.AcceptButton = this.btnSaveMember;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F); // Or 7F, 15F depending on original
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.btnCancelMember;
            this.ClientSize = new System.Drawing.Size(426, 200); // Adjusted client size
            this.Controls.Add(this.gbMemberDetails);
            this.Controls.Add(this.btnSaveMember);
            this.Controls.Add(this.btnCancelMember);
            this.Name = "MemberEditForm";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Text = "Edit Member"; // Default text
            this.gbMemberDetails.ResumeLayout(false);
            this.gbMemberDetails.PerformLayout();
            this.ResumeLayout(false));
        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Button btnSaveMember;
        private System.Windows.Forms.Button btnCancelMember;
        private System.Windows.Forms.GroupBox gbMemberDetails;
    });
}