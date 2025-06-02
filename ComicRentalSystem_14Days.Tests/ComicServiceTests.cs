using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Tests.Mocks; // Assuming Mocks are in this namespace
using System;
using System.Collections.Generic;
using System.IO; // For FileNotFoundException
using System.Linq;

namespace ComicRentalSystem_14Days.Tests
{
    [TestClass]
    public class ComicServiceTests
    {
        private MockFileHelper _mockFileHelper = null!;
        private MockLogger _mockLogger = null!;
        private ComicService _comicService = null!;
        private string _testFileName = "comics.csv";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFileHelper = new MockFileHelper();
            _mockLogger = new MockLogger();

            // Simulate an empty file initially for most tests, or a file that can be read
            _mockFileHelper.FileContents[_testFileName] = "";
            _mockFileHelper.FileLinesForGenericRead[_testFileName] = new List<string>();

            _comicService = new ComicService(_mockFileHelper, _mockLogger);
        }

        // --- AddComic Tests ---
        [TestMethod]
        public void AddComic_NewComic_AddsToListAndSaves()
        {
            // Arrange
            var comic = new Comic { Title = "Test Comic 1", Author = "Author A", Isbn = "1", Genre = "GenreA" };

            // Act
            _comicService.AddComic(comic);
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(1, allComics.Count);
            Assert.AreEqual("Test Comic 1", allComics[0].Title);
            Assert.IsTrue(_mockFileHelper.FileContents.ContainsKey(_testFileName));
            Assert.IsTrue(_mockFileHelper.FileContents[_testFileName].Contains("Test Comic 1"));
            Assert.AreEqual(1, comic.Id, "Comic ID should be assigned."); // Check if ID is generated
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("已新增至記憶體列表") && m.Contains("Test Comic 1")));
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("已成功將 1 本漫畫儲存到")));
        }

        [TestMethod]
        public void AddComic_MultipleComics_AssignsSequentialIds()
        {
            // Arrange
            var comic1 = new Comic { Title = "Comic A", Author = "Auth1", Isbn = "11", Genre = "G1" };
            var comic2 = new Comic { Title = "Comic B", Author = "Auth2", Isbn = "22", Genre = "G2" };

            // Act
            _comicService.AddComic(comic1);
            _comicService.AddComic(comic2);
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(2, allComics.Count);
            Assert.AreEqual(1, comic1.Id);
            Assert.AreEqual(2, comic2.Id);
            Assert.IsTrue(_mockFileHelper.FileContents[_testFileName].Contains("Comic A"));
            Assert.IsTrue(_mockFileHelper.FileContents[_testFileName].Contains("Comic B"));
            Assert.IsTrue(_mockLogger.LoggedMessages.Count(m => m.Contains("已為漫畫") && m.Contains("產生新的ID")) == 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddComic_NullComic_ThrowsArgumentNullException()
        {
            // Arrange
            Comic? comic = null;

            // Act
            _comicService.AddComic(comic!); // Test expects this to throw

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddComic_ComicWithExistingId_ThrowsInvalidOperationException()
        {
            // Arrange
            // Simulate loading a comic with Id = 1
             _mockFileHelper.FileLinesForGenericRead[_testFileName] = new List<string>
            {
                "1,\"PreExisting Comic\",\"AuthorX\",\"ISBNX\",\"GenreX\",False,0,,,"
            };
            _comicService = new ComicService(_mockFileHelper, _mockLogger); // Re-initialize to load the pre-existing comic

            var comicWithExistingId = new Comic { Id = 1, Title = "New Comic with Old ID", Author = "Author Y", Isbn = "ISBNY", Genre = "GenreY" };

            // Act
            _comicService.AddComic(comicWithExistingId);

            // Assert - Exception expected, check log for specific message
        }

        // Placeholder for more tests (Update, Delete, Get, Search, Load, etc.)

        // --- UpdateComic Tests ---
        [TestMethod]
        public void UpdateComic_ExistingComic_UpdatesDetailsAndSaves()
        {
            // Arrange
            var initialComic = new Comic { Id = 1, Title = "Old Title", Author = "Old Author", Isbn = "123", Genre = "OldGenre" };
            _comicService.AddComic(initialComic); // Add it first

            var updatedComicInfo = new Comic { Id = 1, Title = "New Title", Author = "New Author", Isbn = "123Updated", Genre = "NewGenre", IsRented = initialComic.IsRented, RentedToMemberId = initialComic.RentedToMemberId };

            // Act
            _comicService.UpdateComic(updatedComicInfo);
            var retrievedComic = _comicService.GetComicById(1);

            // Assert
            Assert.IsNotNull(retrievedComic);
            Assert.AreEqual("New Title", retrievedComic.Title);
            Assert.AreEqual("New Author", retrievedComic.Author);
            Assert.AreEqual("123Updated", retrievedComic.Isbn);
            Assert.AreEqual("NewGenre", retrievedComic.Genre);
            Assert.IsTrue(_mockFileHelper.FileContents[_testFileName].Contains("New Title"));
            Assert.IsFalse(_mockFileHelper.FileContents[_testFileName].Contains("Old Title"));
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("漫畫屬性已在記憶體中更新") && m.Contains("New Title")));
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("漫畫更新已保存到檔案") && m.Contains("New Title")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateComic_NullComic_ThrowsArgumentNullException()
        {
            // Act
            _comicService.UpdateComic(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateComic_NonExistentComic_ThrowsInvalidOperationException()
        {
            // Arrange
            var comicToUpdate = new Comic { Id = 999, Title = "Non Existent", Author = "Ghost", Isbn = "000", Genre = "Mystery" };

            // Act
            _comicService.UpdateComic(comicToUpdate);
        }

        // --- DeleteComic Tests ---
        [TestMethod]
        public void DeleteComic_ExistingNonRentedComic_RemovesFromListAndSaves()
        {
            // Arrange
            var comic = new Comic { Id = 1, Title = "To Be Deleted", Author = "Author D", Isbn = "del1", Genre = "Action", IsRented = false };
            _comicService.AddComic(comic);
            Assert.AreEqual(1, _comicService.GetAllComics().Count, "Comic should be added before deletion.");

            // Act
            _comicService.DeleteComic(1);
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(0, allComics.Count);
            Assert.IsFalse(_mockFileHelper.FileContents[_testFileName].Contains("To Be Deleted"));
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("已從記憶體列表移除") && m.Contains("To Be Deleted")));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteComic_NonExistentComic_ThrowsInvalidOperationException()
        {
            // Act
            _comicService.DeleteComic(999); // ID that doesn't exist
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteComic_RentedComic_ThrowsInvalidOperationException()
        {
            // Arrange
            var rentedComic = new Comic { Id = 1, Title = "Rented Comic", Author = "Author R", Isbn = "rent1", Genre = "Drama", IsRented = true, RentedToMemberId = 1 };
            _comicService.AddComic(rentedComic);

            // Act
            _comicService.DeleteComic(1); // Attempt to delete a rented comic
        }

        // --- GetComicById Tests ---
        [TestMethod]
        public void GetComicById_ExistingId_ReturnsComic()
        {
            // Arrange
            var comic1 = new Comic { Id = 1, Title = "FindMe", Author = "AuthorF", Isbn = "find1", Genre = "Search" };
            _comicService.AddComic(comic1);
             var comic2 = new Comic { Id = 2, Title = "AnotherComic", Author = "AuthorA", Isbn = "find2", Genre = "Search" };
            _comicService.AddComic(comic2);


            // Act
            var foundComic = _comicService.GetComicById(1);

            // Assert
            Assert.IsNotNull(foundComic);
            Assert.AreEqual(1, foundComic.Id);
            Assert.AreEqual("FindMe", foundComic.Title);
        }

        [TestMethod]
        public void GetComicById_NonExistentId_ReturnsNull()
        {
            // Arrange
            var comic1 = new Comic { Id = 1, Title = "Exists", Author = "AuthorE", Isbn = "exist1", Genre = "Present" };
            _comicService.AddComic(comic1);

            // Act
            var foundComic = _comicService.GetComicById(999);

            // Assert
            Assert.IsNull(foundComic);
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("找不到ID為: 999 的漫畫")));
        }

        // --- GetAllComics Tests ---
        [TestMethod]
        public void GetAllComics_WhenNoComics_ReturnsEmptyList()
        {
            // Arrange
            // _comicService is initialized with no comics by default in TestInitialize

            // Act
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.IsNotNull(allComics);
            Assert.AreEqual(0, allComics.Count);
        }

        [TestMethod]
        public void GetAllComics_WhenComicsExist_ReturnsAllComics()
        {
            // Arrange
            _comicService.AddComic(new Comic { Id = 1, Title = "Comic 1", Author = "A1", Isbn = "I1", Genre = "G1" });
            _comicService.AddComic(new Comic { Id = 2, Title = "Comic 2", Author = "A2", Isbn = "I2", Genre = "G2" });

            // Act
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.IsNotNull(allComics);
            Assert.AreEqual(2, allComics.Count);
            Assert.IsTrue(allComics.Any(c => c.Title == "Comic 1"));
            Assert.IsTrue(allComics.Any(c => c.Title == "Comic 2"));
        }

        // --- SearchComics Tests ---
        [TestMethod]
        public void SearchComics_ByTitle_ReturnsMatchingComics()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Spider-Man Adventures", Author = "Marvel", Isbn = "S1", Genre = "Superhero" });
            _comicService.AddComic(new Comic { Title = "Amazing Spider-Man", Author = "Marvel", Isbn = "S2", Genre = "Superhero" });
            _comicService.AddComic(new Comic { Title = "Batman Chronicles", Author = "DC", Isbn = "B1", Genre = "Superhero" });

            // Act
            var results = _comicService.SearchComics("Spider-Man");

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(c => c.Title.Contains("Spider-Man")));
        }

        [TestMethod]
        public void SearchComics_ByAuthor_ReturnsMatchingComics()
        {
            // Arrange
             _comicService.AddComic(new Comic { Title = "Comic X", Author = "Author Test", Isbn = "CX", Genre = "Test" });
            _comicService.AddComic(new Comic { Title = "Comic Y", Author = "Another Author", Isbn = "CY", Genre = "Test" });
            _comicService.AddComic(new Comic { Title = "Comic Z", Author = "Author Test", Isbn = "CZ", Genre = "Test" });


            // Act
            var results = _comicService.SearchComics("Author Test");

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(c => c.Author == "Author Test"));
        }

        [TestMethod]
        public void SearchComics_ById_ReturnsMatchingComic()
        {
            // Arrange
            _comicService.AddComic(new Comic { Id=1, Title = "Unique ID Comic", Author = "AuthorU", Isbn = "UID1", Genre = "Unique" });
             _comicService.AddComic(new Comic { Id=2, Title = "Another ID Comic", Author = "AuthorA", Isbn = "AID2", Genre = "Another" });


            // Act
            var results = _comicService.SearchComics("1"); // Search by ID as string

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Unique ID Comic", results[0].Title);
        }


        [TestMethod]
        public void SearchComics_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "X-Men", Author = "Marvel", Isbn = "X1", Genre = "Superhero" });

            // Act
            var results = _comicService.SearchComics("NonExistentTerm");

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void SearchComics_NullOrEmptyTerm_ReturnsAllComics()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Comic A", Author = "AuthA", Isbn = "CA", Genre = "G_A" });
            _comicService.AddComic(new Comic { Title = "Comic B", Author = "AuthB", Isbn = "CB", Genre = "G_B" });

            // Act
            var resultsNullTerm = _comicService.SearchComics(null);
            var resultsEmptyTerm = _comicService.SearchComics("");

            // Assert
            Assert.AreEqual(2, resultsNullTerm.Count);
            Assert.AreEqual(2, resultsEmptyTerm.Count);
        }

        // --- GetComicsByGenre Tests ---
        [TestMethod]
        public void GetComicsByGenre_ExistingGenre_ReturnsMatchingComics()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Hero Comic 1", Author = "Writer H", Isbn = "H1", Genre = "Superhero" });
            _comicService.AddComic(new Comic { Title = "SciFi Comic 1", Author = "Writer S", Isbn = "S1", Genre = "Sci-Fi" });
            _comicService.AddComic(new Comic { Title = "Hero Comic 2", Author = "Writer H2", Isbn = "H2", Genre = "Superhero" });

            // Act
            var results = _comicService.GetComicsByGenre("Superhero");

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(c => c.Genre == "Superhero"));
        }

        [TestMethod]
        public void GetComicsByGenre_NonExistentGenre_ReturnsEmptyList()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Manga One", Author = "Mangaka", Isbn = "M1", Genre = "Manga" });

            // Act
            var results = _comicService.GetComicsByGenre("Fantasy");

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetComicsByGenre_NullOrEmptyGenre_ReturnsAllComics()
        {
            // Arrange
             _comicService.AddComic(new Comic { Title = "Any Comic 1", Author = "AC1", Isbn = "AC1", Genre = "G1" });
            _comicService.AddComic(new Comic { Title = "Any Comic 2", Author = "AC2", Isbn = "AC2", Genre = "G2" });

            // Act
            var resultsNullGenre = _comicService.GetComicsByGenre(null!); // Test with null
            var resultsEmptyGenre = _comicService.GetComicsByGenre("");   // Test with empty string

            // Assert
            Assert.AreEqual(2, resultsNullGenre.Count, "Search with null genre should return all comics.");
            Assert.AreEqual(2, resultsEmptyGenre.Count, "Search with empty genre should return all comics.");
        }

        // --- Constructor and LoadComicsFromFile (implicitly tested) Tests ---
        [TestMethod]
        public void Constructor_LoadsComicsFromValidFile()
        {
            // Arrange
            _mockFileHelper.FileLinesForGenericRead[_testFileName] = new List<string>
            {
                "1,\"Loaded Comic 1\",\"Loader\",\"L1\",\"LoadGenre\",False,0,,,",
                "2,\"Loaded Comic 2\",\"Loader\",\"L2\",\"LoadGenre\",True,101,2023-01-01T00:00:00,,",
            };
            // Re-initialize ComicService to trigger constructor load with new mock setup
            _comicService = new ComicService(_mockFileHelper, _mockLogger);

            // Act
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(2, allComics.Count);
            Assert.IsTrue(allComics.Any(c => c.Title == "Loaded Comic 1"));
            Assert.IsTrue(allComics.Any(c => c.Title == "Loaded Comic 2" && c.IsRented));
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("成功從 'comics.csv' (同步) 載入 2 本漫畫。")));
        }

        [TestMethod]
        public void Constructor_HandlesEmptyFile_InitializesEmptyComicList()
        {
            // Arrange
            _mockFileHelper.FileLinesForGenericRead[_testFileName] = new List<string>(); // Empty file
             // Re-initialize ComicService
            _comicService = new ComicService(_mockFileHelper, _mockLogger);

            // Act
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(0, allComics.Count);
             Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("成功從 'comics.csv' (同步) 載入 0 本漫畫。")));
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))] // As per ComicService's error handling
        public void Constructor_HandlesCorruptedFile_ThrowsApplicationException()
        {
            // Arrange
            _mockFileHelper.ReadFileFunc = (fileName) => { throw new FormatException("Corrupted data"); };
            // This setup for ReadFile<T> will cause Comic.FromCsvString to fail if not handled by ReadFileFunc override
            _mockFileHelper.FileLinesForGenericRead[_testFileName] = new List<string> { "this is not valid csv" };

            // We need to ensure the generic ReadFile<T> in MockFileHelper throws FormatException when parser fails
            // or ensure the parser itself (Comic.FromCsvString) throws it and it's caught by ComicService.
            // For this test, we'll assume Comic.FromCsvString will throw FormatException for "this is not valid csv"
            // and ComicService constructor catches it and wraps it in ApplicationException.

            // Re-initialize ComicService
            _comicService = new ComicService(_mockFileHelper, _mockLogger);

            // Act & Assert: Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))] // As per ComicService's error handling
        public void Constructor_HandlesIOExceptionOnLoad_ThrowsApplicationException()
        {
            // Arrange
            _mockFileHelper.ReadFileFunc = (fileName) => { throw new IOException("Disk error"); };
             _mockFileHelper.FileLinesForGenericRead[_testFileName] = new List<string>(); // Need to set this up even if ReadFile throws
            // Re-initialize ComicService
            _comicService = new ComicService(_mockFileHelper, _mockLogger);
            // Act & Assert: Exception expected
        }

        // --- ReloadAsync Tests ---
        [TestMethod]
        public async Task ReloadAsync_LoadsComicsFromValidFile()
        {
            // Arrange
            // Service starts empty
            _comicService = new ComicService(new MockFileHelper(), _mockLogger); // Start with a fresh MockFileHelper for this test
            Assert.AreEqual(0, _comicService.GetAllComics().Count);

            // Setup the (new) mock file helper for ReloadAsync
            _mockFileHelper.FileContents[_testFileName] =
                "1,\"Reloaded Comic 1\",\"Reloader\",\"R1\",\"ReloadGenre\",False,0,,,
" +
                "2,\"Reloaded Comic 2\",\"Reloader\",\"R2\",\"ReloadGenre\",True,102,2023-02-01T00:00:00,,";
             _comicService = new ComicService(_mockFileHelper, _mockLogger); // Re-assign service with the mock that has data for async read

            // Act
            await _comicService.ReloadAsync();
            var allComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(2, allComics.Count);
            Assert.IsTrue(allComics.Any(c => c.Title == "Reloaded Comic 1"));
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains("ComicService 已非同步重新載入。已載入 2 本漫畫。")));
        }

        [TestMethod]
        public async Task ReloadAsync_FileNotFound_ReturnsEmptyListAndLogsWarning()
        {
            // Arrange
             _mockFileHelper.ReadFileAsyncFunc = async (filePath) =>
            {
                await Task.CompletedTask; // Simulate async behavior
                throw new FileNotFoundException("Async file not found");
            };
            _comicService = new ComicService(_mockFileHelper, _mockLogger); // Re-assign service with this specific mock setup

            // Act
            await _comicService.ReloadAsync();
            var comics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(0, comics.Count);
            Assert.IsTrue(_mockLogger.LoggedWarnings.Any(w => w.Contains($"漫畫檔案 '{_testFileName}' (非同步) 找不到。返回空列表。")));
        }


        // --- GetAdminComicStatusViewModels Tests ---
        [TestMethod]
        public void GetAdminComicStatusViewModels_NoComics_ReturnsEmptyList()
        {
            // Arrange
            var members = new List<Member>();

            // Act
            var viewModels = _comicService.GetAdminComicStatusViewModels(members);

            // Assert
            Assert.AreEqual(0, viewModels.Count);
        }

        [TestMethod]
        public void GetAdminComicStatusViewModels_ComicsExist_ReturnsCorrectViewModels()
        {
            // Arrange
            _comicService.AddComic(new Comic { Id = 1, Title = "Comic Alpha", Author = "AuthX", Genre = "GAlpha", IsRented = false, RentedToMemberId = 0 });
            _comicService.AddComic(new Comic { Id = 2, Title = "Comic Beta", Author = "AuthY", Genre = "GBeta", IsRented = true, RentedToMemberId = 101, RentalDate = DateTime.Now.AddDays(-5) });
            _comicService.AddComic(new Comic { Id = 3, Title = "Comic Gamma", Author = "AuthZ", Genre = "GGamma", IsRented = true, RentedToMemberId = 999 }); // Member does not exist

            var members = new List<Member>
            {
                new Member { Id = 101, Name = "John Doe", PhoneNumber = "555-1234" }
            };

            // Act
            var viewModels = _comicService.GetAdminComicStatusViewModels(members);

            // Assert
            Assert.AreEqual(3, viewModels.Count);

            var vmAlpha = viewModels.First(vm => vm.Id == 1);
            Assert.AreEqual("Comic Alpha", vmAlpha.Title);
            Assert.AreEqual("在館中", vmAlpha.Status);
            Assert.IsNull(vmAlpha.BorrowerName);

            var vmBeta = viewModels.First(vm => vm.Id == 2);
            Assert.AreEqual("Comic Beta", vmBeta.Title);
            Assert.AreEqual("被借閱", vmBeta.Status);
            Assert.AreEqual("John Doe", vmBeta.BorrowerName);
            Assert.AreEqual("555-1234", vmBeta.BorrowerPhoneNumber);
            Assert.IsNotNull(vmBeta.RentalDate);

            var vmGamma = viewModels.First(vm => vm.Id == 3);
            Assert.AreEqual("Comic Gamma", vmGamma.Title);
            Assert.AreEqual("被借閱", vmGamma.Status);
            Assert.AreEqual("不明", vmGamma.BorrowerName); // Member 999 not in list
            Assert.IsTrue(_mockLogger.LoggedWarnings.Any(w => w.Contains("找不到ID為 999 的會員")));
        }
    }
}
