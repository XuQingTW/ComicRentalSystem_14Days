using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json; // 請確保使用 System.Text.Json
using System.Windows.Forms; // 為 MessageBox 加入

namespace ComicRentalSystem_14Days.Services
{
    public class AuthenticationService
    {
        private readonly IFileHelper _fileHelper; // 已變更為 IFileHelper
        private readonly ILogger _logger;
        private readonly string _usersFilePath = "users.json"; // 相對於 FileHelper 基礎目錄的路徑
        private List<User> _users;

        private const int MaxFailedLoginAttempts = 5;
        private const int LockoutDurationMinutes = 15;

        public AuthenticationService(IFileHelper fileHelper, ILogger logger) // 已變更為 IFileHelper
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
                    // 嘗試從備份還原
                    return LoadFromBackupOrInitialize();
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"已成功從 {_usersFilePath} 載入 {users?.Count ?? 0} 位使用者。");
                return users;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"反序列化 '{_usersFilePath}' 失敗: {jsonEx.Message}。正在嘗試從備份載入。", jsonEx);
                // 嘗試從備份還原
                return LoadFromBackupOrInitialize(jsonEx); // 傳遞原始例外狀況以取得內容
            }
            catch (Exception ex) // 攔截主要檔案其他可能的 IO 例外狀況
            {
                _logger.LogError($"載入 '{_usersFilePath}' 時發生未預期錯誤: {ex.Message}。正在嘗試從備份載入。", ex);
                return LoadFromBackupOrInitialize(ex); // 傳遞原始例外狀況以取得內容
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
                // 在實際應用程式中，可能會擲回由 Program.cs 攔截的特定例外狀況，以通知管理員或觸發初始設定。
                // 目前，傳回新清單並依賴 EnsureAdminUserExists。
                return new List<User>();
            }

            try
            {
                string json = _fileHelper.ReadFile(backupFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning($"備份使用者檔案 '{backupFilePath}' 為空。正在初始化空的使用者清單。");
                    return new List<User>(); // 或擲回例外狀況，因為這表示備份也已損毀。
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"已成功從備份檔案 '{backupFilePath}' 載入 {users?.Count ?? 0} 位使用者。");

                // 嘗試從備份還原
                try
                {
                    _fileHelper.CopyFile(backupFilePath, _usersFilePath, overwrite: true);
                    _logger.Log($"已成功從 '{backupFilePath}' 還原 '{_usersFilePath}'。");
                }
                catch(Exception restoreEx)
                {
                    _logger.LogError($"從 '{backupFilePath}' 還原 '{_usersFilePath}' 失敗: {restoreEx.Message}", restoreEx);
                    // 無論如何都繼續使用從備份載入的使用者。
                }
                return users;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"嚴重錯誤: 備份使用者檔案 '{backupFilePath}' 也已損毀: {jsonEx.Message}。正在初始化空的使用者清單。", jsonEx);
                return new List<User>(); // 主要檔案和備份檔案都已損毀。
            }
            catch (Exception ex)
            {
                _logger.LogError($"嚴重錯誤: 載入備份使用者檔案 '{backupFilePath}' 時發生未預期錯誤: {ex.Message}。正在初始化空的使用者清單。", ex);
                return new List<User>(); // 主要檔案和備份檔案都已損毀。
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

        public void SaveUsers() // 從 private 改為 public
        {
            string backupFilePath = _usersFilePath + ".bak"; // 例如："users.json.bak"
            string tempFilePath = _usersFilePath + ".tmp";   // 例如："users.json.tmp"

            try
            {
                _logger.Log($"正在嘗試儲存 {_users.Count} 位使用者。");
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });

                // 步驟 1：寫入暫存檔案
                _fileHelper.WriteFile(tempFilePath, json);
                _logger.Log($"使用者資料已寫入暫存檔案: {tempFilePath}");

                // 步驟 2：如果備份檔案存在，則將其刪除
                if (_fileHelper.FileExists(backupFilePath))
                {
                    _fileHelper.DeleteFile(backupFilePath);
                    _logger.Log($"已刪除現有的備份檔案: {backupFilePath}");
                }

                // 步驟 3：如果主要使用者檔案存在，則將其重新命名為備份檔案
                if (_fileHelper.FileExists(_usersFilePath))
                {
                    _fileHelper.MoveFile(_usersFilePath, backupFilePath); // 將目前檔案重新命名為 .bak
                    _logger.Log($"已將目前的使用者檔案 {_usersFilePath} 重新命名為 {backupFilePath}");
                }

                // 步驟 4：將暫存檔案重新命名為主要使用者檔案
                _fileHelper.MoveFile(tempFilePath, _usersFilePath); // 將 .tmp 重新命名為目前檔案
                _logger.Log($"已成功將使用者儲存至 {_usersFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"儲存使用者時發生錯誤: {ex.Message}", ex);
                // 如果已建立暫存檔案，請嘗試清除它
                if (_fileHelper.FileExists(tempFilePath))
                {
                    try { _fileHelper.DeleteFile(tempFilePath); }
                    catch (Exception cleanupEx) { _logger.LogError($"清理暫存檔案 {tempFilePath} 失敗: {cleanupEx.Message}", cleanupEx); }
                }
                // 根據例外狀況，原始的 _usersFilePath 或 backupFilePath 可能仍保持完整。
                throw; // 重新擲回例外狀況，讓應用程式知道儲存失敗
            }
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20); // 20 位元組用於 160 位元雜湊
                return Convert.ToBase64String(hash);
            }
        }

        public bool Register(string username, string password, UserRole role)
        {
            _logger.Log($"使用者名稱註冊嘗試: {username}，角色: {role}");
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"使用者名稱註冊失敗: {username}。使用者名稱已存在。");
                return false; // 使用者名稱已存在
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

            // 檢查帳戶是否鎖定
            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                _logger.Log($"使用者 '{username}' 登入失敗: 帳戶已鎖定直到 {user.LockoutEndDate.Value}。");
                return null;
            }

            // 新增/修改的 PasswordSalt 檢查：
            if (string.IsNullOrEmpty(user.PasswordSalt))
            {
                _logger.LogError($"使用者 '{username}' 登入嘗試失敗：遺失或空白的密碼鹽。帳戶可能來自舊版或已損毀。");
                // 出於鎖定目的，將其視為失敗的登入嘗試
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    _logger.Log($"因遺失密碼鹽導致嘗試失敗，使用者 '{username}' 帳戶已鎖定至 {user.LockoutEndDate.Value}。");
                }
                SaveUsers();
                return null;
            }

            byte[] saltBytes = Convert.FromBase64String(user.PasswordSalt); // 現在確認 PasswordSalt 不是 null 或空字串
            string passwordHashToCompare = HashPassword(password, saltBytes);

            if (user.PasswordHash == passwordHashToCompare)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEndDate = null;
                SaveUsers(); // 儲存變更 (嘗試次數或鎖定)
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
                SaveUsers(); // 儲存變更 (嘗試次數或鎖定)
                return null; // 密碼無效
            }
        }

        // 若不存在任何使用者，則允許建立預設管理員使用者的方法
        public void EnsureAdminUserExists(string adminUsername, string adminPassword)
        {
            if (!_users.Any(u => u.Role == UserRole.Admin))
            {
                _logger.Log($"找不到管理員使用者。正在建立預設管理員: {adminUsername}"); // This log is fine as is, matches existing style
                Register(adminUsername, adminPassword, UserRole.Admin);
            }
            else
            {
                _logger.Log("管理員使用者已存在。"); // This log is fine as is
            }
        }

        public bool DeleteUser(string username)
        {
            _logger.Log($"正在嘗試刪除使用者: {username}"); // This log is fine as is
            User? userToDelete = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (userToDelete != null)
            {
                 // 防止刪除最後一位管理員使用者
                if (userToDelete.Role == UserRole.Admin && _users.Count(u => u.Role == UserRole.Admin) <= 1)
                {
                    _logger.Log($"使用者 '{username}' 是最後一位管理員。無法刪除。"); // This log is fine as is
                    return false; // 無法刪除最後一位管理員
                }
                _users.Remove(userToDelete);
                SaveUsers();
                _logger.Log($"使用者 '{username}' 已成功刪除。"); // This log is fine as is
                return true;
            }
            else
            {
                _logger.Log($"找不到使用者 '{username}'。無法刪除。"); // This log is fine as is
                return false;
            }
        }
    }
}
