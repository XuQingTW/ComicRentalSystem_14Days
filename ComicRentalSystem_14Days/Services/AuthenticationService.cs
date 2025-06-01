using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Services
{
    public class AuthenticationService
    {
        private readonly IFileHelper _fileHelper;
        private readonly ILogger _logger;
        private readonly string _usersFilePath = "users.json";
        private List<User> _users;

        private const int MaxFailedLoginAttempts = 5;
        private const int LockoutDurationMinutes = 15;

        public AuthenticationService(IFileHelper fileHelper, ILogger logger)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _users = LoadUsers() ?? new List<User>();
            _logger.Log("驗證服務已初始化。");
        }

        private byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private List<User>? LoadUsers()
        {
            try
            {
                _logger.Log($"正在嘗試從 {_usersFilePath} 載入使用者資料");
                if (!_fileHelper.FileExists(_usersFilePath))
                {
                    _logger.LogWarning($"在 {_usersFilePath} 找不到使用者檔案。");
                    // Attempt to restore from backup
                    return LoadFromBackupOrInitialize();
                }

                string json = _fileHelper.ReadFile(_usersFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning($"Users file '{_usersFilePath}' is empty.");
                    return LoadFromBackupOrInitialize();
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"已成功從 {_usersFilePath} 載入 {users?.Count ?? 0} 位使用者。");
                return users;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"反序列化 '{_usersFilePath}' 失敗: {jsonEx.Message}。正在嘗試從備份載入。", jsonEx);
                return LoadFromBackupOrInitialize(jsonEx); 
            }
            catch (Exception ex) 
            {
                _logger.LogError($"載入 '{_usersFilePath}' 時發生未預期錯誤: {ex.Message}。正在嘗試從備份載入。", ex);
                return LoadFromBackupOrInitialize(ex);
            }
        }

        private List<User>? LoadFromBackupOrInitialize(Exception? primaryLoadException = null)
        {
            string backupFilePath = _usersFilePath + ".bak";
            _logger.Log($"正在嘗試從備份檔案 '{backupFilePath}' 載入使用者資料。");

            if (!_fileHelper.FileExists(backupFilePath))
            {
                _logger.LogWarning($"找不到備份使用者檔案 '{backupFilePath}'。");
                if (primaryLoadException != null)
                {
                     _logger.LogError($"嚴重錯誤: 主要使用者檔案 '{_usersFilePath}' 載入失敗且找不到備份檔案 '{backupFilePath}'。正在初始化空的使用者清單。", primaryLoadException);
                }
                else
                {
                    _logger.LogWarning($"嚴重警告: 主要使用者檔案 '{_usersFilePath}' 不存在且找不到備份檔案 '{backupFilePath}'。正在初始化空的使用者清單。");
                }
                return new List<User>();
            }

            try
            {
                string json = _fileHelper.ReadFile(backupFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning($"備份使用者檔案 '{backupFilePath}' 為空。正在初始化空的使用者清單。");
                    return new List<User>();
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"已成功從備份檔案 '{backupFilePath}' 載入 {users?.Count ?? 0} 位使用者。");

                try
                {
                    _fileHelper.CopyFile(backupFilePath, _usersFilePath, overwrite: true);
                    _logger.Log($"已成功從 '{backupFilePath}' 還原 '{_usersFilePath}'。");
                }
                catch(Exception restoreEx)
                {
                    _logger.LogError($"從 '{backupFilePath}' 還原 '{_usersFilePath}' 失敗: {restoreEx.Message}", restoreEx);
                }
                return users;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"嚴重錯誤: 備份使用者檔案 '{backupFilePath}' 也已損毀: {jsonEx.Message}。正在初始化空的使用者清單。", jsonEx);
                return new List<User>(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"嚴重錯誤: 載入備份使用者檔案 '{backupFilePath}' 時發生未預期錯誤: {ex.Message}。正在初始化空的使用者清單。", ex);
                return new List<User>(); 
            }
        }

        public List<User> GetAllUsers()
        {
            _logger.Log($"GetAllUsers called. Returning {_users.Count} users.");
            return new List<User>(_users);
        }

        public User? GetUserByUsername(string username)
        {
            _logger.Log($"正在嘗試透過使用者名稱擷取使用者: {username}");
            User? user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                _logger.LogWarning($"找不到使用者名稱為 '{username}' 的使用者。");
            }
            else
            {
                _logger.Log($"User found: {user.Username}, Role: {user.Role}");
            }
            return user;
        }

        public void SaveUsers() 
        {
            string backupFilePath = _usersFilePath + ".bak"; 
            string tempFilePath = _usersFilePath + ".tmp";  

            try
            {
                _logger.Log($"正在嘗試儲存 {_users.Count} 位使用者。");
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });

                _fileHelper.WriteFile(tempFilePath, json);
                _logger.Log($"使用者資料已寫入暫存檔案: {tempFilePath}");

                if (_fileHelper.FileExists(backupFilePath))
                {
                    _fileHelper.DeleteFile(backupFilePath);
                    _logger.Log($"已刪除現有的備份檔案: {backupFilePath}");
                }

                if (_fileHelper.FileExists(_usersFilePath))
                {
                    _fileHelper.MoveFile(_usersFilePath, backupFilePath);
                    _logger.Log($"已將目前的使用者檔案 {_usersFilePath} 重新命名為 {backupFilePath}");
                }
                _fileHelper.MoveFile(tempFilePath, _usersFilePath);
                _logger.Log($"已成功將使用者儲存至 {_usersFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"儲存使用者時發生錯誤: {ex.Message}", ex);
                if (_fileHelper.FileExists(tempFilePath))
                {
                    try { _fileHelper.DeleteFile(tempFilePath); }
                    catch (Exception cleanupEx) { _logger.LogError($"清理暫存檔案 {tempFilePath} 失敗: {cleanupEx.Message}", cleanupEx); }
                }
                throw; 
            }
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20); 
                return Convert.ToBase64String(hash);
            }
        }

        public bool Register(string username, string password, UserRole role)
        {
            _logger.Log($"使用者名稱註冊嘗試: {username}，角色: {role}");
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"使用者名稱註冊失敗: {username}。使用者名稱已存在。");
                return false;
            }

            byte[] saltBytes = GenerateSalt();
            string hashedPassword = HashPassword(password, saltBytes);

            User newUser = new User(username, hashedPassword, role)
            {
                PasswordSalt = Convert.ToBase64String(saltBytes)
            };
            _users.Add(newUser);
            SaveUsers();
            _logger.Log($"使用者 {username} 已成功註冊為 {role} 角色。使用者總數: {_users.Count}");
            return true;
        }

        public User? Login(string username, string password)
        {
            _logger.Log($"使用者名稱登入嘗試: {username}");
            User? user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                _logger.LogWarning($"使用者名稱登入失敗: {username}。找不到使用者。");
                return null;
            }

            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                _logger.Log($"使用者 '{username}' 登入失敗: 帳戶已鎖定直到 {user.LockoutEndDate.Value}。");
                return null;
            }
            if (string.IsNullOrEmpty(user.PasswordSalt))
            {
                _logger.LogError($"使用者 '{username}' 登入嘗試失敗：遺失或空白的密碼鹽。帳戶可能來自舊版或已損毀。");
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    _logger.Log($"因遺失密碼鹽導致嘗試失敗，使用者 '{username}' 帳戶已鎖定至 {user.LockoutEndDate.Value}。");
                }
                SaveUsers();
                return null;
            }

            byte[] saltBytes = Convert.FromBase64String(user.PasswordSalt);
            string passwordHashToCompare = HashPassword(password, saltBytes);

            if (user.PasswordHash == passwordHashToCompare)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEndDate = null;
                SaveUsers(); 
                _logger.Log($"使用者名稱登入成功: {username}，角色: {user.Role}");
                return user;
            }
            else
            {
                user.FailedLoginAttempts++;
                _logger.LogWarning($"使用者名稱登入失敗: {username}。密碼不正確。");
                if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    _logger.Log($"使用者 '{username}' 帳戶已鎖定直到 {user.LockoutEndDate.Value}，因為登入失敗次數過多。");
                }
                SaveUsers(); 
                return null; 
            }
        }

        // 如果不存在任何使用者，則允許建立預設管理員使用者
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
                if (userToDelete.Role == UserRole.Admin && _users.Count(u => u.Role == UserRole.Admin) <= 1)
                {
                    _logger.Log($"使用者 '{username}' 是最後一位管理員。無法刪除。"); 
                    return false; 
                }
                _users.Remove(userToDelete);
                SaveUsers();
                _logger.Log($"使用者 '{username}' 已成功刪除。"); 
                return true;
            }
            else
            {
                _logger.Log($"找不到使用者 '{username}'。無法刪除。");
                return false;
            }
        }
    }
}
