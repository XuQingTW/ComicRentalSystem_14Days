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

        private const int MaxFailedLoginAttempts = 5;
        private const int LockoutDurationMinutes = 15;

        public AuthenticationService(IFileHelper fileHelper, ILogger logger) // Changed parameter to IFileHelper
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
                _logger.Log($"Attempting to load users from {_usersFilePath}");
                if (!_fileHelper.FileExists(_usersFilePath))
                {
                    _logger.LogWarning($"Users file '{_usersFilePath}' not found.");
                    // Attempt to restore from backup
                    return LoadFromBackupOrInitialize();
                }

                string json = _fileHelper.ReadFile(_usersFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning($"Users file '{_usersFilePath}' is empty.");
                    // Attempt to restore from backup
                    return LoadFromBackupOrInitialize();
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"Successfully loaded {users?.Count ?? 0} users from {_usersFilePath}.");
                return users;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"Error deserializing '{_usersFilePath}': {jsonEx.Message}. Attempting to load from backup.", jsonEx);
                // Attempt to restore from backup
                return LoadFromBackupOrInitialize(jsonEx); // Pass original exception for context
            }
            catch (Exception ex) // Catch other potential IO exceptions for main file
            {
                _logger.LogError($"Unexpected error loading '{_usersFilePath}': {ex.Message}. Attempting to load from backup.", ex);
                return LoadFromBackupOrInitialize(ex); // Pass original exception for context
            }
        }

        private List<User>? LoadFromBackupOrInitialize(Exception? primaryLoadException = null)
        {
            string backupFilePath = _usersFilePath + ".bak";
            _logger.Log($"Attempting to load users from backup file '{backupFilePath}'.");

            if (!_fileHelper.FileExists(backupFilePath))
            {
                _logger.LogWarning($"Backup users file '{backupFilePath}' not found.");
                if (primaryLoadException != null)
                {
                     _logger.LogError($"Critical: Main users file '{_usersFilePath}' failed to load and no backup '{backupFilePath}' exists. Initializing empty user list.", primaryLoadException);
                }
                else
                {
                    _logger.LogWarning($"Critical: Main users file '{_usersFilePath}' not found and no backup '{backupFilePath}' exists. Initializing empty user list.");
                }
                // In a real app, might throw a specific exception caught by Program.cs to inform admin or trigger initial setup.
                // For now, returning new list and relying on EnsureAdminUserExists.
                return new List<User>();
            }

            try
              
            {
                string json = _fileHelper.ReadFile(backupFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning($"Backup users file '{backupFilePath}' is empty. Initializing empty user list.");
                    return new List<User>(); // Or throw, as this means backup is also bad.
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"Successfully loaded {users?.Count ?? 0} users from backup file '{backupFilePath}'.");

                // Attempt to restore the main file from backup
                try
                {
                    _fileHelper.CopyFile(backupFilePath, _usersFilePath, overwrite: true);
                    _logger.Log($"Successfully restored '{_usersFilePath}' from '{backupFilePath}'.");
                }
                catch(Exception restoreEx)
                {
                    _logger.LogError($"Failed to restore '{_usersFilePath}' from '{backupFilePath}': {restoreEx.Message}", restoreEx);
                    // Continue with users loaded from backup anyway.
                }
                return users;
            }
            catch (JsonException jsonEx)
            {

                _logger.LogError($"Critical: Backup users file '{backupFilePath}' is also corrupted: {jsonEx.Message}. Initializing empty user list.", jsonEx);
                return new List<User>(); // Both main and backup are bad.
            }
            catch (Exception ex)
            {
                _logger.LogError($"Critical: Unexpected error loading backup users file '{backupFilePath}': {ex.Message}. Initializing empty user list.", ex);
                return new List<User>(); // Both main and backup are bad.
            }
        }

        public User? GetUserByUsername(string username)
        {
            _logger.Log($"Attempting to retrieve user by username: {username}");
            User? user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                _logger.LogWarning($"User with username '{username}' not found.");
            }
            return user;
        }

        public List<User> GetAllUsers()
        {
            return new List<User>(_users);
        }

        public void SaveUsers() // Changed from private to public
        {
            string backupFilePath = _usersFilePath + ".bak"; // e.g., "users.json.bak"
            string tempFilePath = _usersFilePath + ".tmp";   // e.g., "users.json.tmp"

            try
            {
                _logger.Log($"Attempting to save {_users.Count} users.");
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });

                // Step 1: Write to a temporary file
                _fileHelper.WriteFile(tempFilePath, json);
                _logger.Log($"Users data written to temporary file: {tempFilePath}");

                // Step 2: If a backup file exists, delete it
                if (_fileHelper.FileExists(backupFilePath))
                {
                    _fileHelper.DeleteFile(backupFilePath);
                    _logger.Log($"Deleted existing backup file: {backupFilePath}");
                }

                // Step 3: If the main user file exists, rename it to be the backup file
                if (_fileHelper.FileExists(_usersFilePath))
                {
                    _fileHelper.MoveFile(_usersFilePath, backupFilePath); // Rename current to .bak
                    _logger.Log($"Renamed current users file {_usersFilePath} to {backupFilePath}");
                }

                // Step 4: Rename the temporary file to be the main user file
                _fileHelper.MoveFile(tempFilePath, _usersFilePath); // Rename .tmp to current
                _logger.Log($"Successfully saved users to {_usersFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during saving users: {ex.Message}", ex);
                // If temp file was created, try to clean it up
                if (_fileHelper.FileExists(tempFilePath))
                {
                    try { _fileHelper.DeleteFile(tempFilePath); }
                    catch (Exception cleanupEx) { _logger.LogError($"Failed to cleanup temp file {tempFilePath}: {cleanupEx.Message}", cleanupEx); }
                }
                // Depending on the exception, the original _usersFilePath or backupFilePath might still be intact.
                throw; // Re-throw so the application knows saving failed
            }
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20); // 20 bytes for a 160-bit hash
                return Convert.ToBase64String(hash);
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

            byte[] saltBytes = GenerateSalt();
            string hashedPassword = HashPassword(password, saltBytes);

            User newUser = new User(username, hashedPassword, role)
            {
                PasswordSalt = Convert.ToBase64String(saltBytes)
            };
            _users.Add(newUser);
            SaveUsers();
            _logger.Log($"使用者 '{username}' (ID: {newUser.Id}) 註冊成功。");
            return true;
        }

        public User? Login(string username, string password)
        {
            _logger.Log($"正在嘗試登入使用者: {username}");
            User? user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                _logger.Log($"使用者 '{username}' 登入失敗: 使用者不存在。");
                return null;
            }

            // Check for lockout
            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                _logger.Log($"使用者 '{username}' 登入失敗: 帳戶已鎖定直到 {user.LockoutEndDate.Value}。");
                return null;
            }

            // Ensure PasswordSalt is not null before attempting to use it
            if (user.PasswordSalt == null)
            {
                _logger.LogError($"使用者 '{username}' 的 PasswordSalt 為空值。無法驗證密碼。");
                // This case might indicate a data corruption or an old user record prior to salt implementation.
                // Treat as a failed login attempt for security.
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    _logger.Log($"使用者 '{username}' 帳戶已鎖定直到 {user.LockoutEndDate.Value}，因為 PasswordSalt 為空且達到最大失敗嘗試次數。");
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
                SaveUsers(); // Save changes
                _logger.Log($"使用者 '{username}' 成功登入。角色: {user.Role}");
                return user;
            }
            else
            {
                user.FailedLoginAttempts++;
                _logger.Log($"使用者 '{username}' 登入失敗: 密碼無效。嘗試次數 {user.FailedLoginAttempts} / {MaxFailedLoginAttempts}。");
                if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                    _logger.Log($"使用者 '{username}' 帳戶已鎖定直到 {user.LockoutEndDate.Value}，因為登入失敗次數過多。");
                }
                SaveUsers(); // Save changes (attempts or lockout)
                return null; // Invalid password
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
