using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models; // For User model if needed, though not directly for these tests
using System.IO;
using System;
using System.Text.Json; // For JsonException, though tests expect ApplicationException
using System.Collections.Generic; // For List<User>

namespace ComicRentalSystem_14Days.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IFileHelper> _mockFileHelper;
        private Mock<ILogger> _mockLogger;
        private const string TestUsersFilePath = "users.json"; // Matching the path in AuthenticationService

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFileHelper = new Mock<IFileHelper>();
            _mockLogger = new Mock<ILogger>();

            // Default setup for GetFullFilePath if AuthenticationService uses it internally
            // (though it seems _usersFilePath is used directly with ReadFile/WriteFile)
            _mockFileHelper.Setup(fh => fh.GetFullFilePath(TestUsersFilePath)).Returns(TestUsersFilePath);
        }

        [TestMethod]
        public void Test_LoadUsers_FileNotFound()
        {
            // Arrange
            // FileHelper.ReadFile now returns string.Empty for non-existent files,
            // and AuthenticationService's LoadUsers catches FileNotFoundException if ReadFile were to throw it.
            // If ReadFile returns string.Empty, LoadUsers interprets it as no users.
            _mockFileHelper.Setup(fh => fh.ReadFile(TestUsersFilePath)).Returns(string.Empty);
            // Alternative: If we want to ensure the explicit FileNotFoundException path in LoadUsers is tested
            // _mockFileHelper.Setup(fh => fh.ReadFile(TestUsersFilePath)).Throws<FileNotFoundException>();

            // Act
            var authService = new AuthenticationService(_mockFileHelper.Object, _mockLogger.Object);

            // Assert
            // Constructor should complete, and _users list should be empty.
            // We can indirectly test this by trying to log in.
            Assert.IsNull(authService.Login("anyuser", "anypass"));
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains("Users file is empty or not found") || s.Contains("not found. A new one will be created"))), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Test_LoadUsers_EmptyFile()
        {
            // Arrange
            _mockFileHelper.Setup(fh => fh.ReadFile(TestUsersFilePath)).Returns("   "); // Whitespace only

            // Act
            var authService = new AuthenticationService(_mockFileHelper.Object, _mockLogger.Object);

            // Assert
            Assert.IsNull(authService.Login("anyuser", "anypass")); // No users loaded
            _mockLogger.Verify(log => log.Log("Users file is empty or not found, returning new list."), Times.Once);
        }

        [TestMethod]
        public void Test_LoadUsers_CorruptedJsonFile()
        {
            // Arrange
            _mockFileHelper.Setup(fh => fh.ReadFile(TestUsersFilePath)).Returns("{ \"Username\": \"test\""); // Malformed JSON

            // Act & Assert
            var ex = Assert.ThrowsException<ApplicationException>(() =>
                new AuthenticationService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.AreEqual("Failed to load critical user data due to corrupted file.", ex.Message);
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains("Critical error: User data file 'users.json' is corrupted.")), It.IsAny<JsonException>()), Times.Once);
            // Verification of MessageBox.Show is difficult in unit tests and not typically done.
        }

        [TestMethod]
        public void Test_LoadUsers_OtherIOException()
        {
            // Arrange
            var ioException = new IOException("Disk error or file lock");
            _mockFileHelper.Setup(fh => fh.ReadFile(TestUsersFilePath)).Throws(ioException);

            // Act & Assert
            var ex = Assert.ThrowsException<ApplicationException>(() =>
                new AuthenticationService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.AreEqual("Unexpected error during user data loading.", ex.Message);
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains("An unexpected error occurred while loading users from users.json")), ioException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadUsers_SuccessfulLoad()
        {
            // Arrange
            var users = new List<User> { new User("testuser", "hashedpassword", UserRole.User) { Id = 1 } };
            string jsonContent = JsonSerializer.Serialize(users);
            _mockFileHelper.Setup(fh => fh.ReadFile(TestUsersFilePath)).Returns(jsonContent);

            // Act
            var authService = new AuthenticationService(_mockFileHelper.Object, _mockLogger.Object);

            // Assert
            // Successfully loaded, try login (assuming HashPassword works correctly, which is not part of this test's scope)
            // To make login testable here without depending on HashPassword, we'd need to know the hashed pass or mock HashPassword.
            // For now, just verify the log.
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains($"Successfully loaded {users.Count} users."))), Times.Once);

            // A more direct test would be to make _users internal or provide a method to get user count for testing.
            // Example of a login test if password hashing was predictable or mockable:
            // User loggedInUser = authService.Login("testuser", "password"); // This would fail due to hashing
            // Assert.IsNotNull(loggedInUser);
        }
    }
}
