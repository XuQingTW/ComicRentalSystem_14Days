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
            _logger.Log("AuthenticationService initialized.");
        }

        private List<User>? LoadUsers()
        {
            try
            {
                _logger.Log($"Attempting to load users from {_usersFilePath}");
                string json = _fileHelper.ReadFile(_usersFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.Log("Users file is empty or not found, returning new list.");
                    return new List<User>();
                }
                var users = JsonSerializer.Deserialize<List<User>>(json);
                _logger.Log($"Successfully loaded {users?.Count ?? 0} users.");
                return users;
            }
            catch (FileNotFoundException)
            {
                _logger.Log($"Users file '{_usersFilePath}' not found. A new one will be created on registration.");
                return new List<User>();
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Critical error: User data file '{_usersFilePath}' is corrupted. Details: {ex.Message}", ex);
                MessageBox.Show($"使用者資料檔案已損壞，無法載入使用者。應用程式將關閉。\n錯誤詳情: {ex.Message}\n檔案路徑: {_usersFilePath}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new ApplicationException("Failed to load critical user data due to corrupted file.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while loading users from {_usersFilePath}. Details: {ex.Message}", ex);
                MessageBox.Show($"載入使用者時發生未預期的錯誤。應用程式將關閉。\n錯誤詳情: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new ApplicationException("Unexpected error during user data loading.", ex);
            }
        }

        private void SaveUsers()
        {
            try
            {
                _logger.Log($"Attempting to save {_users.Count} users to {_usersFilePath}");
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                _fileHelper.WriteFile(_usersFilePath, json);
                _logger.Log("Users saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving users to {_usersFilePath}.", ex);
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
            _logger.Log($"Attempting to register user: {username}, Role: {role}");
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Log($"Registration failed: Username '{username}' already exists.");
                return false; // Username already exists
            }

            string passwordHash = HashPassword(password);
            User newUser = new User(username, passwordHash, role);
            _users.Add(newUser);
            SaveUsers();
            _logger.Log($"User '{username}' registered successfully with ID {newUser.Id}.");
            return true;
        }

        public User? Login(string username, string password)
        {
            _logger.Log($"Attempting to login user: {username}");
            string passwordHash = HashPassword(password);
            User? user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.PasswordHash == passwordHash);

            if (user != null)
            {
                _logger.Log($"User '{username}' logged in successfully. Role: {user.Role}");
                return user;
            }
            else
            {
                _logger.Log($"Login failed for user: {username}. Invalid username or password.");
                return null;
            }
        }

        // Method to allow creating a default admin user if no users exist
        public void EnsureAdminUserExists(string adminUsername, string adminPassword)
        {
            if (!_users.Any(u => u.Role == UserRole.Admin))
            {
                _logger.Log($"No admin user found. Creating default admin: {adminUsername}");
                Register(adminUsername, adminPassword, UserRole.Admin);
            }
            else
            {
                _logger.Log("Admin user already exists.");
            }
        }

        public bool DeleteUser(string username)
        {
            _logger.Log($"Attempting to delete user: {username}");
            User? userToDelete = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (userToDelete != null)
            {
                _users.Remove(userToDelete);
                SaveUsers();
                _logger.Log($"User '{username}' deleted successfully.");
                return true;
            }
            else
            {
                _logger.Log($"Delete failed: User '{username}' not found.");
                return false;
            }
        }
    }
}
