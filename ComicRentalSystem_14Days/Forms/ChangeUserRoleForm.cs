using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Linq; 
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ChangeUserRoleForm : BaseForm
    {
        private readonly User _editingUser;
        private readonly Member _editingMember;
        private readonly AuthenticationService _authService;
        private readonly IComicService _comicService;


        public ChangeUserRoleForm(
            User userToEdit,
            Member member,
            AuthenticationService authService,
            IComicService comicService,
            ILogger logger) : base(logger)
        {
            InitializeComponent();

            _editingUser = userToEdit ?? throw new ArgumentNullException(nameof(userToEdit));
            _editingMember = member ?? throw new ArgumentNullException(nameof(member));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));

            this.Text = "更改使用者角色"; 
            lblUsernameValue.Text = _editingUser.Username;

            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = _editingUser.Role;
            LogActivity($"使用者角色變更表單已為使用者初始化: {_editingUser.Username}");
        }


        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("請選擇一個角色。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return;
            }

            UserRole newRole = (UserRole)cmbRole.SelectedItem;
            if (_editingUser.Role == newRole)
            {
                LogActivity($"使用者 '{_editingUser.Username}' 的角色已經是 {newRole}。未做任何變更。");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            if (_editingUser.Role == UserRole.Admin && newRole != UserRole.Admin)
            {
                if (_authService.GetAllUsers().Count(u => u.Role == UserRole.Admin) <= 1)
                {
                    MessageBox.Show("無法更改最後一位管理員的角色。", "操作禁止", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogActivity($"嘗試變更最後一位管理員 '{_editingUser.Username}' 角色的操作已被阻止。");
                    return;
                }
            }

            if (newRole == UserRole.Admin)
            {
                bool hasActiveRentals = _comicService.GetAllComics().Any(c => c.IsRented && c.RentedToMemberId == _editingMember.Id);
                if (hasActiveRentals)
                {
                    MessageBox.Show($"會員 '{_editingMember.Name}' 尚有未歸還的漫畫，無法變更為管理員。", "操作禁止", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogActivity($"嘗試將有租借中的會員 '{_editingMember.Name}' 變更為管理員已被阻止。");
                    return;
                }
            }

            LogActivity($"嘗試將使用者 '{_editingUser.Username}' 的角色從 {_editingUser.Role} 變更為 {newRole}。");
            _editingUser.Role = newRole;

            try
            {
                _authService.SaveUser(_editingUser);
                LogActivity($"已成功將使用者 '{_editingUser.Username}' 的角色變更為 {newRole}。");
                MessageBox.Show("使用者角色已成功更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"儲存使用者 '{_editingUser.Username}' 的角色更新時發生錯誤: {ex.Message}", ex);
                MessageBox.Show($"儲存角色更新失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); 

            }
        }

        private void btnCancel_Click(object? sender, EventArgs e) 
        {
            LogActivity("使用者角色變更表單已由使用者取消。");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
