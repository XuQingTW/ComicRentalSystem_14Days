using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json; // Make sure to use System.Text.Json
using System.Windows.Forms; // Added for MessageBox

namespace ComicRentalSystem_14Days.Services
{
    public class AuthenticationService
    {
        private readonly IFileHelper _fileHelper; // Changed to IFileHelper
        private readonly ILogger _logger;
        private readonly string _usersFilePath = "users.json"; // Path relative to FileHelper's base directory
        private List<User> _users;

        public AuthenticationService(IFileHelper fileHelper, ILogger logger) // Changed parameter to IFileHelper
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _users = LoadUsers() ?? new List<User>();
            _logger.Log("驗證服務已初始化。");
        }

        private List<User>? LoadUsers()
        {
            try
            {
                _logger.Log($"正在嘗試從 {_usersFilePath} 載入使用者");
                string json = _fileHelper.ReadFile(_usersFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.Log("使用者檔案為空或找不到，返回新列表。");
                    return new List<User>();
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"成功載入 {users?.Count ?? 0} 位使用者。");
                return users;
            }
            catch (FileNotFoundException)
            {
                _logger.Log($"找不到使用者檔案 '{_usersFilePath}'。將在註冊時建立新檔案。");
                return new List<User>();
            }
            catch (JsonException ex)
            {
                _logger.LogError($"嚴重錯誤: 使用者資料檔案 '{_usersFilePath}' 已損壞。詳細資訊: {ex.Message}", ex);
                // MessageBox.Show($"使用者資料檔案已損壞，無法載入使用者。應用程式將關閉。\n錯誤詳情: {ex.Message}\n檔案路徑: {_usersFilePath}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new ApplicationException("由於檔案損壞，無法載入關鍵使用者資料。", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 {_usersFilePath} 載入使用者時發生未預期的錯誤。詳細資訊: {ex.Message}", ex);
                // MessageBox.Show($"載入使用者時發生未預期的錯誤。應用程式將關閉。\n錯誤詳情: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new ApplicationException("載入使用者資料期間發生未預期錯誤。", ex);
            }
        }

        private void SaveUsers()
        {
            try
            {
                _logger.Log($"正在嘗試將 {_users.Count} 位使用者儲存到 {_usersFilePath}");
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                _fileHelper.WriteFile(_usersFilePath, json);
                _logger.Log("使用者儲存成功。");
            }
            catch (Exception ex)
            {
                _logger.LogError($"將使用者儲存到 {_usersFilePath} 時發生錯誤。", ex);
                // Handle exception (e.g., notify admin, retry logic)
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool Register(string username, string password, UserRole role)
        {
            _logger.Log($"正在嘗試註冊使用者: {username}, 角色: {role}");
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Log($"註冊失敗: 使用者名稱 '{username}' 已存在。");
                return false; // Username already exists
            }

            string passwordHash = HashPassword(password);
            User newUser = new User(username, passwordHash, role);
            _users.Add(newUser);
            SaveUsers();
            _logger.Log($"使用者 '{username}' (ID: {newUser.Id}) 註冊成功。");
            return true;
        }

        public User? Login(string username, string password)
        {
            _logger.Log($"正在嘗試登入使用者: {username}");
            string passwordHash = HashPassword(password);
            User? user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.PasswordHash == passwordHash);

            if (user != null)
            {
                _logger.Log($"使用者 '{username}' 成功登入。角色: {user.Role}");
                return user;
            }
            else
            {
                _logger.Log($"使用者 '{username}' 登入失敗。使用者名稱或密碼無效。");
                return null;
            }
        }

        // Method to allow creating a default admin user if no users exist
        public void EnsureAdminUserExists(string adminUsername, string adminPassword)
        {
            if (!_users.Any(u => u.Role == UserRole.Admin))
            {
                _logger.Log($"找不到管理員使用者。正在建立預設管理員: {adminUsername}");
                Register(adminUsername, adminPassword, UserRole.Admin);
            }
            else
            {
                _logger.Log("管理員使用者已存在。");
            }
        }

        public bool DeleteUser(string username)
        {
            _logger.Log($"正在嘗試刪除使用者: {username}");
            User? userToDelete = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (userToDelete != null)
            {
                _users.Remove(userToDelete);
                SaveUsers();
                _logger.Log($"使用者 '{username}' 刪除成功。");
                return true;
            }
            else
            {
                _logger.Log($"刪除失敗: 找不到使用者 '{username}'。");
                return false;
            }
        }
    }
}
