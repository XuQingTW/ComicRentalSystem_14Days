using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging; // Assuming a simple logger might be needed
using System.IO; // For file operations during test cleanup
using System.Linq; // For Enumerable.Count

namespace ComicRentalSystem_14Days.IntegrationTests
{
    [TestClass]
    public class AuthenticationServiceIntegrationTests
    {
        private const string TestUsersFilePath = "test_users.csv";
        private const string TestLogFilePath = "test_auth_integration_log.txt";
        private IFileHelper _fileHelper = null!;
        private ILogger _logger = null!;
        private AuthenticationService _authService = null!;
        private MemberService _memberService = null!; // Needed for some auth operations like checking if member exists
        private ComicService _comicService = null!; // Needed for MemberService constructor

        [TestInitialize]
        public void TestInitialize()
        {
            // Clean up any existing test files before each test
            if (File.Exists(TestUsersFilePath))
            {
                File.Delete(TestUsersFilePath);
            }
            if (File.Exists(TestLogFilePath))
            {
                File.Delete(TestLogFilePath);
            }
            if (File.Exists("comics.csv")) // Default comic file, might be touched by MemberService init
            {
                 File.Delete("comics.csv");
            }
            if (File.Exists("members.csv")) // Default member file
            {
                 File.Delete("members.csv");
            }


            _fileHelper = new FileHelper(TestUsersFilePath, "members.csv", "comics.csv"); // Specify users file path
            _logger = new FileLogger(TestLogFilePath);

            // ComicService and MemberService are needed for AuthenticationService context,
            // especially if user creation also implies member creation or checks.
            // For integration tests, we use real instances but with test-specific file paths.
            _comicService = new ComicService(_fileHelper, _logger); // Uses "comics.csv" by default internally
            _memberService = new MemberService(_fileHelper, _logger, _comicService); // Uses "members.csv" by default
            _authService = new AuthenticationService(_fileHelper, _logger, TestUsersFilePath); // Override default users file

            // Ensure a clean state for services that might load data on instantiation
            // For example, ensure the test users file is empty or non-existent before _authService potentially creates/loads it.
             _authService.EnsureAdminUserExists("admin", "admin123"); // Ensure default admin for tests
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up test files after each test
            if (File.Exists(TestUsersFilePath))
            {
                File.Delete(TestUsersFilePath);
            }
            if (File.Exists(TestLogFilePath))
            {
                File.Delete(TestLogFilePath);
            }
            if (File.Exists("members.csv"))
            {
                File.Delete("members.csv");
            }
            if (File.Exists("comics.csv"))
            {
                 File.Delete("comics.csv");
            }
        }

        [TestMethod]
        public void RegisterUser_NewUser_ShouldSucceed()
        {
            // Arrange
            string username = "testuser";
            string password = "password123";
            string fullName = "Test User";
            string phoneNumber = "1234567890";
            UserRole role = UserRole.Member;

            // Act
            bool registrationResult = _authService.RegisterUser(username, password, role, fullName, phoneNumber, _memberService);

            // Assert
            Assert.IsTrue(registrationResult, "User registration should be successful.");
            User? registeredUser = _authService.Login(username, password);
            Assert.IsNotNull(registeredUser, "User should be able to login after registration.");
            Assert.AreEqual(username, registeredUser.Username);
            Assert.AreEqual(role, registeredUser.Role);

            // Verify member creation as well
            Member? member = _memberService.GetMemberByUsername(username);
            Assert.IsNotNull(member, "Member record should be created for the registered user.");
            Assert.AreEqual(fullName, member.Name);
            Assert.AreEqual(phoneNumber, member.PhoneNumber);
        }

        [TestMethod]
        public void RegisterUser_ExistingUsername_ShouldFail()
        {
            // Arrange
            string username = "existinguser";
            string password = "password123";
            _authService.RegisterUser(username, "initialpassword", UserRole.Member, "Existing User", "0987654321", _memberService);

            // Act
            bool registrationResult = _authService.RegisterUser(username, password, UserRole.Member, "Another User", "1122334455", _memberService);

            // Assert
            Assert.IsFalse(registrationResult, "User registration should fail for an existing username.");
        }

        [TestMethod]
        public void Login_ValidCredentials_ShouldReturnUser()
        {
            // Arrange
            string username = "testlogin";
            string password = "password123";
            _authService.RegisterUser(username, password, UserRole.Member, "Login Test User", "5555555555", _memberService);

            // Act
            User? loggedInUser = _authService.Login(username, password);

            // Assert
            Assert.IsNotNull(loggedInUser, "Login should be successful with valid credentials.");
            Assert.AreEqual(username, loggedInUser.Username);
        }

        [TestMethod]
        public void Login_InvalidPassword_ShouldReturnNull()
        {
            // Arrange
            string username = "testloginfail";
            string password = "correctpassword";
            _authService.RegisterUser(username, password, UserRole.Member, "Login Fail User", "6666666666", _memberService);

            // Act
            User? loggedInUser = _authService.Login(username, "wrongpassword");

            // Assert
            Assert.IsNull(loggedInUser, "Login should fail with an invalid password.");
        }

        [TestMethod]
        public void Login_NonExistentUser_ShouldReturnNull()
        {
            // Arrange
            string username = "nonexistentuser";
            string password = "password123";

            // Act
            User? loggedInUser = _authService.Login(username, password);

            // Assert
            Assert.IsNull(loggedInUser, "Login should fail for a non-existent user.");
        }

        [TestMethod]
        public void EnsureAdminUserExists_AdminDoesNotExist_ShouldCreateAdmin()
        {
            // Arrange - TestInitialize already calls EnsureAdminUserExists,
            // so we need to check if it was created.
            // To isolate this test, we might need a fresh authService without the TestInitialize call,
            // or verify its effects directly.

            // For this test, we'll assume TestInitialize created it.
            // We can verify by trying to log in as admin.

            // Act
            User? adminUser = _authService.Login("admin", "admin123");

            // Assert
            Assert.IsNotNull(adminUser, "Admin user should exist and be able to login.");
            Assert.AreEqual("admin", adminUser.Username);
            Assert.AreEqual(UserRole.Admin, adminUser.Role);
        }

        [TestMethod]
        public void EnsureAdminUserExists_AdminAlreadyExists_ShouldNotCreateDuplicate()
        {
            // Arrange
            // The first call is in TestInitialize. We call it again.
            _authService.EnsureAdminUserExists("admin", "newpassword"); // Attempt to create again or update

            // Act
            User? adminUser = _authService.Login("admin", "admin123"); // Try logging in with the original password

            // Assert
            Assert.IsNotNull(adminUser, "Admin should still exist and be loginable with the original password if not designed to update.");

            // Depending on the design of EnsureAdminUserExists (e.g., if it updates password), this assertion might change.
            // Current AuthenticationService.EnsureAdminUserExists does not update if admin exists.
            var users = _fileHelper.ReadFile<User>("test_users.csv", User.FromCsvString);
            int adminCount = users.Count(u => u.Username == "admin");
            Assert.AreEqual(1, adminCount, "There should be only one admin user.");
        }
    }
}
