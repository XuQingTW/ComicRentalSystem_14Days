using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Linq; // Count() 所需
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ChangeUserRoleForm : BaseForm // 繼承自 BaseForm 以進行日誌記錄
    {
        private readonly User _editingUser;
        private readonly AuthenticationService _authService;

        // UI 控制項現在定義在 ChangeUserRoleForm.Designer.cs 中
        // 此處不再需要手動宣告。

        public ChangeUserRoleForm(User userToEdit, AuthenticationService authService, ILogger logger) : base(logger)
        {
            InitializeComponent(); // 這現在將呼叫 Designer.cs 中的方法

            _editingUser = userToEdit ?? throw new ArgumentNullException(nameof(userToEdit));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            this.Text = "更改使用者角色"; // 更改使用者角色
            lblUsernameValue.Text = _editingUser.Username;

            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = _editingUser.Role;
            LogActivity($"使用者角色變更表單已為使用者初始化: {_editingUser.Username}");
        }

        // 手動的 InitializeComponent() 已移除。它現在位於 ChangeUserRoleForm.Designer.cs 中

        private void btnSave_Click(object? sender, EventArgs e) // sender 已變更為 object?
        {
            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("請選擇一個角色。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning); // "請選擇一個角色。" | "驗證錯誤"
                return;
            }

            UserRole newRole = (UserRole)cmbRole.SelectedItem;
            if (_editingUser.Role == newRole)
            {
                LogActivity($"使用者 '{_editingUser.Username}' 的角色已經是 {newRole}。未做任何變更。");
                this.DialogResult = DialogResult.Cancel; // 或 OK，但無任何變更
                this.Close();
                return;
            }

            // 防止將最後一位管理員的角色變更為非管理員
            if (_editingUser.Role == UserRole.Admin && newRole != UserRole.Admin)
            {
                // 此功能需要在 AuthService 中使用 GetAllUsers()
                if (_authService.GetAllUsers().Count(u => u.Role == UserRole.Admin) <= 1)
                {
                    MessageBox.Show("無法更改最後一位管理員的角色。", "操作禁止", MessageBoxButtons.OK, MessageBoxIcon.Error); // "無法更改最後一位管理員的角色。" | "操作禁止"
                    LogActivity($"嘗試變更最後一位管理員 '{_editingUser.Username}' 角色的操作已被阻止。");
                    return;
                }
            }

            LogActivity($"嘗試將使用者 '{_editingUser.Username}' 的角色從 {_editingUser.Role} 變更為 {newRole}。");
            _editingUser.Role = newRole;

            try
            {
                // 這假設 _authService._users 列表包含與 _editingUser 相同的實例
                // 且 SaveUsers() 將會永久保存此變更。
                _authService.SaveUsers();
                LogActivity($"已成功將使用者 '{_editingUser.Username}' 的角色變更為 {newRole}。");
                MessageBox.Show("使用者角色已成功更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); // "使用者角色已成功更新。" | "成功"
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"儲存使用者 '{_editingUser.Username}' 的角色更新時發生錯誤: {ex.Message}", ex);
                MessageBox.Show($"儲存角色更新失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); // $"儲存角色更新失敗: {ex.Message}" | "錯誤"
                // 如果儲存失敗，可選擇性地復原對 _editingUser 的角色變更，但如果通過引用操作則較為棘手。
                // 為簡單起見，此處不進行復原。記憶體中的物件已變更，但如果儲存失敗，下次載入時將顯示舊角色。
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e) // sender 已變更為 object?
        {
            LogActivity("使用者角色變更表單已由使用者取消。");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
