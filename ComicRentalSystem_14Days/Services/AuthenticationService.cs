using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ComicRentalSystem_14Days.Services
{
    public class AuthenticationService
    {
        private readonly ILogger _logger;

        private const int MaxFailedLoginAttempts = 5;
        private const int LockoutDurationMinutes = 15;

        public AuthenticationService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.Log("AuthenticationService initialized (DbContext will be created per-operation).");
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

        public List<User> GetAllUsers()
        {
            _logger.Log("GetAllUsers called.");
            using (var context = new ComicRentalDbContext())
            {
                return context.Users.ToList();
            }
        }

        public User? GetUserByUsername(string username)
        {
            _logger.Log($"Attempting to retrieve user by username: {username}");
            using (var context = new ComicRentalDbContext())
            {
                User? user = context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
                if (user == null)
                {
                _logger.LogWarning($"User with username '{username}' not found.");
            }
            else
            {
                _logger.Log($"User found: {user.Username}, Role: {user.Role}");
            }
            return user;
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
            _logger.Log($"Attempting to register user: {username}, Role: {role}");
            using (var context = new ComicRentalDbContext())
            {
                if (context.Users.Any(u => u.Username.ToLower() == username.ToLower()))
                {
                    _logger.LogWarning($"Registration failed for {username}: Username already exists.");
                    return false;
                }

                byte[] saltBytes = GenerateSalt();
                string hashedPassword = HashPassword(password, saltBytes);

                User newUser = new User(username, hashedPassword, role)
                {
                    PasswordSalt = Convert.ToBase64String(saltBytes)
                };

                context.Users.Add(newUser);
                try
                {
                    context.SaveChanges();
                    _logger.Log($"User {username} registered successfully as {role}. User count now (from DB): {context.Users.Count()}.");
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Error registering user {username} to database.", ex);
                    return false;
                }
            }
        }

        public User? Login(string username, string password)
        {
            _logger.Log($"Login attempt for username: {username}");
            using (var context = new ComicRentalDbContext())
            {
                User? user = context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

                if (user == null)
                {
                    _logger.LogWarning($"Login failed for {username}: User not found.");
                    return null;
                }

                if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
                {
                    _logger.Log($"Login failed for '{username}': Account locked until {user.LockoutEndDate.Value}.");
                    return null;
                }

                if (string.IsNullOrEmpty(user.PasswordSalt))
                {
                    _logger.LogError($"Login attempt failed for '{username}': Missing or empty password salt. Account might be legacy or corrupted.");
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                    {
                        user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                        _logger.Log($"User '{username}' account locked until {user.LockoutEndDate.Value} due to missing salt and failed attempts.");
                    }
                    try { context.SaveChanges(); } catch (DbUpdateException dbEx) { _logger.LogError($"Failed to save login attempt changes for user {username} (missing salt).", dbEx); }
                    return null;
                }

                byte[] saltBytes = Convert.FromBase64String(user.PasswordSalt);
                string passwordHashToCompare = HashPassword(password, saltBytes);

                if (user.PasswordHash == passwordHashToCompare)
                {
                    user.FailedLoginAttempts = 0;
                    user.LockoutEndDate = null;
                    try { context.SaveChanges(); } catch (DbUpdateException dbEx) { _logger.LogError($"Failed to save successful login changes for user {username}.", dbEx); }
                    _logger.Log($"Login successful for {username}, Role: {user.Role}");
                    return user;
                }
                else
                {
                    user.FailedLoginAttempts++;
                    _logger.LogWarning($"Login failed for {username}: Incorrect password.");
                    if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                    {
                        user.LockoutEndDate = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                        _logger.Log($"User '{username}' account locked until {user.LockoutEndDate.Value} due to too many failed login attempts.");
                    }
                    try { context.SaveChanges(); } catch (DbUpdateException dbEx) { _logger.LogError($"Failed to save failed login attempt changes for user {username}.", dbEx); }
                    return null;
                }
            }
        }

        public void EnsureAdminUserExists(string adminUsername, string adminPassword)
        {
            using (var context = new ComicRentalDbContext()) // Context for the initial check
            {
                if (!context.Users.Any(u => u.Role == UserRole.Admin))
                {
                    _logger.Log($"No admin user found. Creating default admin: {adminUsername}");
                    // Register method itself creates a new context, so this is fine.
                    Register(adminUsername, adminPassword, UserRole.Admin);
                }
                else
                {
                    _logger.Log("Admin user already exists.");
                }
            }
        }

        public bool DeleteUser(string username)
        {
            _logger.Log($"Attempting to delete user: {username}");
            using (var context = new ComicRentalDbContext())
            {
                User? userToDelete = context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

                if (userToDelete != null)
                {
                    if (userToDelete.Role == UserRole.Admin && context.Users.Count(u => u.Role == UserRole.Admin) <= 1)
                    {
                        _logger.LogWarning($"User '{username}' is the last admin. Deletion aborted.");
                        return false;
                    }
                    context.Users.Remove(userToDelete);
                    try
                    {
                        context.SaveChanges();
                        _logger.Log($"User '{username}' deleted successfully from database.");
                        return true;
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError($"Error deleting user {username} from database.", ex);
                        return false;
                    }
                }
                else
                {
                    _logger.LogWarning($"User '{username}' not found. Deletion failed.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Persists pending changes for the user table. This helper was kept
        /// for backwards compatibility with forms that previously relied on a
        /// separate save step when user data was stored in files.
        /// </summary>
        public void SaveUsers()
        {
            _logger.Log("Saving user changes to database via AuthenticationService.SaveUsers().");
            using (var context = new ComicRentalDbContext())
            {
                context.SaveChanges();
            }
        }
    }
}
