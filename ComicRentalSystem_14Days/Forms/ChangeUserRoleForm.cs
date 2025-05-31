using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Linq; // Required for Count()
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ChangeUserRoleForm : BaseForm // Inherit from BaseForm for logging
    {
        private readonly User _editingUser;
        private readonly AuthenticationService _authService;

        // Controls that would be in Designer.cs - declare them here for the code to compile
        private System.Windows.Forms.Label lblUsernameLabel;
        private System.Windows.Forms.Label lblUsernameValue;
        private System.Windows.Forms.Label lblRoleLabel;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        public ChangeUserRoleForm(User userToEdit, AuthenticationService authService, ILogger logger) : base(logger)
        {
            InitializeComponent(); // Make sure this is called

            _editingUser = userToEdit ?? throw new ArgumentNullException(nameof(userToEdit));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            this.Text = "Change User Role";
            lblUsernameValue.Text = _editingUser.Username;

            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = _editingUser.Role;
            LogActivity($"ChangeUserRoleForm initialized for user: {_editingUser.Username}");
        }

        // Add a basic InitializeComponent for non-designer context
        // In a real scenario, this is auto-generated in Designer.cs
        private void InitializeComponent()
        {
            this.lblUsernameLabel = new System.Windows.Forms.Label();
            this.lblUsernameValue = new System.Windows.Forms.Label();
            this.lblRoleLabel = new System.Windows.Forms.Label();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblUsernameLabel
            //
            this.lblUsernameLabel.AutoSize = true;
            this.lblUsernameLabel.Location = new System.Drawing.Point(12, 15);
            this.lblUsernameLabel.Name = "lblUsernameLabel";
            this.lblUsernameLabel.Size = new System.Drawing.Size(63, 13);
            this.lblUsernameLabel.TabIndex = 0;
            this.lblUsernameLabel.Text = "Username:";
            //
            // lblUsernameValue
            //
            this.lblUsernameValue.AutoSize = true;
            this.lblUsernameValue.Location = new System.Drawing.Point(81, 15);
            this.lblUsernameValue.Name = "lblUsernameValue";
            this.lblUsernameValue.Size = new System.Drawing.Size(0, 13); // Will be set in constructor
            this.lblUsernameValue.TabIndex = 1;
            //
            // lblRoleLabel
            //
            this.lblRoleLabel.AutoSize = true;
            this.lblRoleLabel.Location = new System.Drawing.Point(12, 42);
            this.lblRoleLabel.Name = "lblRoleLabel";
            this.lblRoleLabel.Size = new System.Drawing.Size(32, 13);
            this.lblRoleLabel.TabIndex = 2;
            this.lblRoleLabel.Text = "Role:";
            //
            // cmbRole
            //
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.FormattingEnabled = true;
            this.cmbRole.Location = new System.Drawing.Point(84, 39);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(188, 21);
            this.cmbRole.TabIndex = 3;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(116, 76);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 76);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // ChangeUserRoleForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 111);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbRole);
            this.Controls.Add(this.lblRoleLabel);
            this.Controls.Add(this.lblUsernameValue);
            this.Controls.Add(this.lblUsernameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeUserRoleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change User Role";
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserRole newRole = (UserRole)cmbRole.SelectedItem;
            if (_editingUser.Role == newRole)
            {
                LogActivity($"User role for '{_editingUser.Username}' is already {newRole}. No changes made.");
                this.DialogResult = DialogResult.Cancel; // Or OK, but nothing changed
                this.Close();
                return;
            }

            // Prevent changing the role of the last admin to non-admin
            if (_editingUser.Role == UserRole.Admin && newRole != UserRole.Admin)
            {
                // Need GetAllUsers() in AuthService for this
                if (_authService.GetAllUsers().Count(u => u.Role == UserRole.Admin) <= 1)
                {
                    MessageBox.Show("Cannot change the role of the last administrator.", "Operation Forbidden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogActivity($"Attempt to change role of last admin '{_editingUser.Username}' was blocked.");
                    return;
                }
            }

            LogActivity($"Attempting to change role for user '{_editingUser.Username}' from {_editingUser.Role} to {newRole}.");
            _editingUser.Role = newRole;

            try
            {
                // This assumes _authService._users list contains the same instance as _editingUser
                // and SaveUsers() will persist this change.
                _authService.SaveUsers();
                LogActivity($"Successfully changed role for user '{_editingUser.Username}' to {newRole}.");
                MessageBox.Show("User role updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error saving user role update for '{_editingUser.Username}': {ex.Message}", ex);
                MessageBox.Show($"Failed to save role update: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Optionally revert role change on _editingUser if save fails, though tricky if it's by reference.
                // For simplicity, we're not reverting here. The object in memory is changed, but next load would show old role if save failed.
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LogActivity("ChangeUserRoleForm cancelled by user.");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
