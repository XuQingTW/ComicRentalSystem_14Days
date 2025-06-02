using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using System.IO;
using System.Linq;
using System; // Required for DateTime

namespace ComicRentalSystem_14Days.IntegrationTests
{
    [TestClass]
    public class RentalProcessIntegrationTests
    {
        private const string TestUsersFile = "rental_test_users.csv";
        private const string TestMembersFile = "rental_test_members.csv";
        private const string TestComicsFile = "rental_test_comics.csv";
        private const string TestLogFile = "rental_test_log.txt";

        private IFileHelper _fileHelper = null!;
        private ILogger _logger = null!;
        private AuthenticationService _authService = null!;
        private MemberService _memberService = null!;
        private ComicService _comicService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Clean up any existing test files before each test
            DeleteTestFiles();

            _fileHelper = new FileHelper(TestUsersFile, TestMembersFile, TestComicsFile);
            _logger = new FileLogger(TestLogFile);

            _comicService = new ComicService(_fileHelper, _logger);
            _memberService = new MemberService(_fileHelper, _logger, _comicService); // Pass comicService to MemberService
            _authService = new AuthenticationService(_fileHelper, _logger, TestUsersFile);

            // Ensure services start clean or with necessary defaults
            _authService.EnsureAdminUserExists("admin", "adminpass"); // Default admin
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up test files after each test
            DeleteTestFiles();
        }

        private void DeleteTestFiles()
        {
            if (File.Exists(TestUsersFile)) File.Delete(TestUsersFile);
            if (File.Exists(TestMembersFile)) File.Delete(TestMembersFile);
            if (File.Exists(TestComicsFile)) File.Delete(TestComicsFile);
            if (File.Exists(TestLogFile)) File.Delete(TestLogFile);
        }

        private User RegisterAndLoginTestUser(string username, string password, string fullName, string phone)
        {
            _authService.RegisterUser(username, password, UserRole.Member, fullName, phone, _memberService);
            User? user = _authService.Login(username, password);
            Assert.IsNotNull(user, $"Failed to log in test user {username}");
            return user;
        }

        private Member GetTestMember(string username)
        {
            Member? member = _memberService.GetMemberByUsername(username);
            Assert.IsNotNull(member, $"Failed to retrieve member {username}. Ensure RegisterUser also creates a member.");
            return member;
        }

        private Comic AddTestComic(string title, string author, string isbn, string genre, decimal price = 5.0m, int stock = 1, DateTime? publicationDate = null, bool isRented = false, int rentedToMemberId = 0, DateTime? rentalDate = null, DateTime? returnDate = null)
        {
            var comic = new Comic
            {
                Title = title, Author = author, Isbn = isbn, Genre = genre,
                Price = price, Stock = stock, PublicationDate = publicationDate ?? DateTime.Now.Date,
                IsRented = isRented, RentedToMemberId = rentedToMemberId,
                RentalDate = rentalDate, ReturnDate = returnDate
            };
            _comicService.AddComic(comic);
            Assert.IsTrue(comic.Id > 0, "Comic ID should be assigned after adding.");
            return comic;
        }

        [TestMethod]
        public void RentComic_AvailableComicAndValidMember_ShouldSucceed()
        {
            // Arrange
            User testUser = RegisterAndLoginTestUser("member1", "pass1", "Member One", "111");
            Member testMember = GetTestMember(testUser.Username);
            Comic availableComic = AddTestComic("Comic Alpha", "Auth A", "ISBN001", "GenreX");
            DateTime expectedReturnDate = DateTime.Today.AddDays(7);

            // Act
            bool rentResult = _comicService.RentComic(availableComic.Id, testMember.Id, expectedReturnDate);
            Comic rentedComic = _comicService.GetComicById(availableComic.Id);

            // Assert
            Assert.IsTrue(rentResult, "Renting the comic should succeed.");
            Assert.IsNotNull(rentedComic, "Rented comic should still be retrievable.");
            Assert.IsTrue(rentedComic.IsRented, "Comic status should be IsRented = true.");
            Assert.AreEqual(testMember.Id, rentedComic.RentedToMemberId, "Comic should be rented to the correct member ID.");
            Assert.IsNotNull(rentedComic.RentalDate, "RentalDate should be set.");
            Assert.AreEqual(expectedReturnDate.Date, rentedComic.ReturnDate?.Date, "ReturnDate should be set correctly.");

            // Verify persistence
            var persistedComics = _fileHelper.ReadFile<Comic>(TestComicsFile, Comic.FromCsvString);
            var persistedComic = persistedComics.FirstOrDefault(c => c.Id == availableComic.Id);
            Assert.IsNotNull(persistedComic);
            Assert.IsTrue(persistedComic.IsRented);
            Assert.AreEqual(testMember.Id, persistedComic.RentedToMemberId);
        }

        [TestMethod]
        public void RentComic_AlreadyRentedComic_ShouldFail()
        {
            // Arrange
            User user1 = RegisterAndLoginTestUser("userA", "passA", "User A", "100");
            Member member1 = GetTestMember(user1.Username);
            User user2 = RegisterAndLoginTestUser("userB", "passB", "User B", "200");
            Member member2 = GetTestMember(user2.Username);

            Comic comic = AddTestComic("Shared Comic", "Auth S", "ISBN00S", "Shared", isRented: true, rentedToMemberId: member1.Id, rentalDate: DateTime.Now.AddDays(-1));
            DateTime newReturnDate = DateTime.Today.AddDays(5);

            // Act
            bool rentResult = _comicService.RentComic(comic.Id, member2.Id, newReturnDate);

            // Assert
            Assert.IsFalse(rentResult, "Renting an already rented comic to another member should fail.");
            Comic comicAfterAttempt = _comicService.GetComicById(comic.Id);
            Assert.AreEqual(member1.Id, comicAfterAttempt.RentedToMemberId, "Comic should still be rented to the original member.");
        }

        [TestMethod]
        public void RentComic_NonExistentComic_ShouldFail()
        {
            // Arrange
            User testUser = RegisterAndLoginTestUser("member2", "pass2", "Member Two", "222");
            Member testMember = GetTestMember(testUser.Username);
            int nonExistentComicId = 999;
            DateTime returnDate = DateTime.Today.AddDays(7);

            // Act
            bool rentResult = _comicService.RentComic(nonExistentComicId, testMember.Id, returnDate);

            // Assert
            Assert.IsFalse(rentResult, "Renting a non-existent comic should fail.");
        }

        [TestMethod]
        public void RentComic_NonExistentMember_ShouldFail()
        {
            // Arrange
            Comic availableComic = AddTestComic("Comic Beta", "Auth B", "ISBN002", "GenreY");
            int nonExistentMemberId = 999;
            DateTime returnDate = DateTime.Today.AddDays(7);

            // Act
            bool rentResult = _comicService.RentComic(availableComic.Id, nonExistentMemberId, returnDate);

            // Assert
            Assert.IsFalse(rentResult, "Renting a comic to a non-existent member should fail.");
             Comic comicAfterAttempt = _comicService.GetComicById(availableComic.Id);
            Assert.IsFalse(comicAfterAttempt.IsRented, "Comic should remain not rented.");
        }


        [TestMethod]
        public void ReturnComic_RentedComic_ShouldSucceed()
        {
            // Arrange
            User testUser = RegisterAndLoginTestUser("member3", "pass3", "Member Three", "333");
            Member testMember = GetTestMember(testUser.Username);
            Comic rentedComic = AddTestComic("Comic Charlie", "Auth C", "ISBN003", "GenreZ",
                                            isRented: true, rentedToMemberId: testMember.Id,
                                            rentalDate: DateTime.Today.AddDays(-5),
                                            returnDate: DateTime.Today.AddDays(2));

            // Act
            bool returnResult = _comicService.ReturnComic(rentedComic.Id, testMember.Id);
            Comic comicAfterReturn = _comicService.GetComicById(rentedComic.Id);

            // Assert
            Assert.IsTrue(returnResult, "Returning the comic should succeed.");
            Assert.IsNotNull(comicAfterReturn, "Comic should still be retrievable after return.");
            Assert.IsFalse(comicAfterReturn.IsRented, "Comic IsRented status should be false after return.");
            Assert.AreEqual(0, comicAfterReturn.RentedToMemberId, "RentedToMemberId should be reset (e.g., to 0 or default).");
            Assert.IsNotNull(comicAfterReturn.ActualReturnTime, "ActualReturnTime should be set.");

            // Verify persistence
            var persistedComics = _fileHelper.ReadFile<Comic>(TestComicsFile, Comic.FromCsvString);
            var persistedComic = persistedComics.FirstOrDefault(c => c.Id == rentedComic.Id);
            Assert.IsNotNull(persistedComic);
            Assert.IsFalse(persistedComic.IsRented);
            Assert.AreEqual(0, persistedComic.RentedToMemberId);
            Assert.IsNotNull(persistedComic.ActualReturnTime);
        }

        [TestMethod]
        public void ReturnComic_NotRentedComic_ShouldFail()
        {
            // Arrange
            User testUser = RegisterAndLoginTestUser("member4", "pass4", "Member Four", "444");
            Member testMember = GetTestMember(testUser.Username);
            Comic availableComic = AddTestComic("Comic Delta", "Auth D", "ISBN004", "GenreW");

            // Act
            bool returnResult = _comicService.ReturnComic(availableComic.Id, testMember.Id);

            // Assert
            Assert.IsFalse(returnResult, "Returning a comic that was not rented should fail.");
        }

        [TestMethod]
        public void ReturnComic_RentedByDifferentMember_ShouldFail()
        {
            // Arrange
            User userAlpha = RegisterAndLoginTestUser("userAlpha", "passAlpha", "Alpha Rent", "A01");
            Member memberAlpha = GetTestMember(userAlpha.Username);
            User userBeta = RegisterAndLoginTestUser("userBeta", "passBeta", "Beta Return", "B02");
            Member memberBeta = GetTestMember(userBeta.Username); // The one trying to return

            Comic comic = AddTestComic("Comic Epsilon", "Auth E", "ISBN005", "GenreV",
                                     isRented: true, rentedToMemberId: memberAlpha.Id,
                                     rentalDate: DateTime.Today.AddDays(-3));

            // Act
            bool returnResult = _comicService.ReturnComic(comic.Id, memberBeta.Id); // memberBeta tries to return memberAlpha's comic

            // Assert
            Assert.IsFalse(returnResult, "Returning a comic rented by a different member should fail.");
            Comic comicAfterAttempt = _comicService.GetComicById(comic.Id);
            Assert.IsTrue(comicAfterAttempt.IsRented, "Comic should remain rented.");
            Assert.AreEqual(memberAlpha.Id, comicAfterAttempt.RentedToMemberId, "Comic should still be rented to original member.");
        }

        [TestMethod]
        public void ReturnComic_NonExistentComic_ShouldFail()
        {
            // Arrange
            User testUser = RegisterAndLoginTestUser("member5", "pass5", "Member Five", "555");
            Member testMember = GetTestMember(testUser.Username);
            int nonExistentComicId = 777;

            // Act
            bool returnResult = _comicService.ReturnComic(nonExistentComicId, testMember.Id);

            // Assert
            Assert.IsFalse(returnResult, "Returning a non-existent comic should fail.");
        }
    }
}
