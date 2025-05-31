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

        // UI Controls are now defined in ChangeUserRoleForm.Designer.cs
        // Manual declarations are no longer needed here.

        public ChangeUserRoleForm(User userToEdit, AuthenticationService authService, ILogger logger) : base(logger)
        {
            InitializeComponent(); // This will now call the method from Designer.cs

            _editingUser = userToEdit ?? throw new ArgumentNullException(nameof(userToEdit));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            this.Text = "更改使用者角色"; // Change User Role
            lblUsernameValue.Text = _editingUser.Username;

            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = _editingUser.Role;
            LogActivity($"ChangeUserRoleForm initialized for user: {_editingUser.Username}");
        }

        // Manual InitializeComponent() removed. It's now in ChangeUserRoleForm.Designer.cs

        private void btnSave_Click(object? sender, EventArgs e) // sender changed to object?
        {
            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("請選擇一個角色。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning); // "Please select a role." | "Validation Error"
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
                    MessageBox.Show("無法更改最後一位管理員的角色。", "操作禁止", MessageBoxButtons.OK, MessageBoxIcon.Error); // "Cannot change the role of the last administrator." | "Operation Forbidden"
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
                MessageBox.Show("使用者角色已成功更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); // "User role updated successfully." | "Success"
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error saving user role update for '{_editingUser.Username}': {ex.Message}", ex);
                MessageBox.Show($"儲存角色更新失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); // $"Failed to save role update: {ex.Message}" | "Error"
                // Optionally revert role change on _editingUser if save fails, though tricky if it's by reference.
                // For simplicity, we're not reverting here. The object in memory is changed, but next load would show old role if save failed.
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e) // sender changed to object?
        {
            LogActivity("ChangeUserRoleForm cancelled by user.");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
