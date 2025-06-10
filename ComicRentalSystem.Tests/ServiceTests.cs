using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Models;
using System;
using System.IO;
using System.Linq;

namespace ComicRentalSystem.Tests
{
    [TestClass]
    public class ServiceTests
    {
        private string? _originalConfigHome;
        private string _tempDir = string.Empty;
        private TestLogger _logger = null!;
        private AuthenticationService _authService = null!;
        private ComicService _comicService = null!;
        private MemberService _memberService = null!;

        [TestInitialize]
        public void Setup()
        {
            _logger = new TestLogger();
            _originalConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", _tempDir);

            using (var context = new ComicRentalDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            _authService = new AuthenticationService(_logger);
            _comicService = new ComicService(_logger);
            _memberService = new MemberService(_logger, new DummyComicService());
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", _originalConfigHome);
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [TestMethod]
        public void AuthenticationService_Register_AddsUser()
        {
            var result = _authService.Register("testuser", "password", UserRole.Member);
            Assert.IsTrue(result);
            using var context = new ComicRentalDbContext();
            var user = context.Users.SingleOrDefault(u => u.Username == "testuser");
            Assert.IsNotNull(user);
            Assert.AreEqual(UserRole.Member, user!.Role);
        }

        [TestMethod]
        public void AuthenticationService_Register_Duplicate_ReturnsFalse()
        {
            _authService.Register("dupuser", "pass1", UserRole.Member);
            var result = _authService.Register("dupuser", "pass2", UserRole.Member);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AuthenticationService_Login_Success()
        {
            _authService.Register("loginuser", "mypw", UserRole.Admin);
            var user = _authService.Login("loginuser", "mypw");
            Assert.IsNotNull(user);
            Assert.AreEqual("loginuser", user!.Username);
        }

        [TestMethod]
        public void AuthenticationService_Login_WrongPassword_Increments()
        {
            _authService.Register("loginfail", "right", UserRole.Member);
            var result = _authService.Login("loginfail", "wrong");
            Assert.IsNull(result);
            using var context = new ComicRentalDbContext();
            var user = context.Users.Single(u => u.Username == "loginfail");
            Assert.AreEqual(1, user.FailedLoginAttempts);
        }

        [TestMethod]
        public void AuthenticationService_Login_LockoutAfterFiveAttempts()
        {
            _authService.Register("lockme", "good", UserRole.Member);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsNull(_authService.Login("lockme", "bad"));
            }
            using var context = new ComicRentalDbContext();
            var user = context.Users.Single(u => u.Username == "lockme");
            Assert.IsTrue(user.LockoutEndDate.HasValue);
        }

        [TestMethod]
        public void ComicService_AddComic_SavesToDb()
        {
            var comic = new Comic { Title = "A", Author = "B", Isbn = "1", Genre = "G" };
            _comicService.AddComic(comic);
            using var context = new ComicRentalDbContext();
            var fromDb = context.Comics.Single(c => c.Title == "A");
            Assert.AreNotEqual(0, fromDb.Id);
        }

        [TestMethod]
        public void ComicService_AddComic_Invalid_Throws()
        {
            var comic = new Comic { Author = "B", Isbn = "1", Genre = "G", Title = "" };
            Assert.ThrowsException<ArgumentException>(() => _comicService.AddComic(comic));
        }

        [TestMethod]
        public void MemberService_AddMember_SavesToDb()
        {
            var member = new Member { Name = "M", PhoneNumber = "099", Username = "u" };
            _memberService.AddMember(member);
            using var context = new ComicRentalDbContext();
            var fromDb = context.Members.Single(m => m.Name == "M");
            Assert.AreNotEqual(0, fromDb.Id);
        }

        [TestMethod]
        public void MemberService_AddMember_Invalid_Throws()
        {
            var member = new Member { Name = "", PhoneNumber = "" };
            Assert.ThrowsException<ArgumentException>(() => _memberService.AddMember(member));
        }
    }
}
