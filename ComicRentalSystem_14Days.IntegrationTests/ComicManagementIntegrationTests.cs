using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using System.IO;
using System.Linq;
using System.Collections.Generic; // Required for List<T>
using System; // Required for DateTime

namespace ComicRentalSystem_14Days.IntegrationTests
{
    [TestClass]
    public class ComicManagementIntegrationTests
    {
        private const string TestComicsFilePath = "integration_test_comics.csv";
        private const string TestLogFilePath = "integration_test_comic_mgmt_log.txt";
        private IFileHelper _fileHelper = null!;
        private ILogger _logger = null!;
        private ComicService _comicService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Clean up any existing test files before each test
            if (File.Exists(TestComicsFilePath))
            {
                File.Delete(TestComicsFilePath);
            }
            if (File.Exists(TestLogFilePath))
            {
                File.Delete(TestLogFilePath);
            }

            _fileHelper = new FileHelper("users.csv", "members.csv", TestComicsFilePath); // Specify comics file path
            _logger = new FileLogger(TestLogFilePath);
            _comicService = new ComicService(_fileHelper, _logger);

            // Ensure the comic service starts with a clean slate (no comics loaded from a previous run)
            // The ComicService constructor already tries to load from TestComicsFilePath.
            // Since we delete it above, it will start empty.
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up test files after each test
            if (File.Exists(TestComicsFilePath))
            {
                File.Delete(TestComicsFilePath);
            }
            if (File.Exists(TestLogFilePath))
            {
                File.Delete(TestLogFilePath);
            }
        }

        [TestMethod]
        public void AddComic_NewComic_ShouldBeAddedAndPersisted()
        {
            // Arrange
            var newComic = new Comic { Title = "Integration Test Comic", Author = "Test Author", Isbn = "123-IT", Genre = "Testing", Price = 10.0m, Stock = 5, PublicationDate = DateTime.Now.Date };

            // Act
            _comicService.AddComic(newComic);
            var persistedComics = _fileHelper.ReadFile<Comic>(TestComicsFilePath, Comic.FromCsvString);
            var retrievedComic = _comicService.GetComicById(newComic.Id);

            // Assert
            Assert.AreEqual(1, persistedComics.Count, "Comic should be persisted to the file.");
            Assert.AreEqual("Integration Test Comic", persistedComics[0].Title);
            Assert.IsNotNull(retrievedComic, "Comic should be retrievable from service after adding.");
            Assert.AreEqual("Integration Test Comic", retrievedComic.Title);
        }

        [TestMethod]
        public void UpdateComic_ExistingComic_ShouldBeUpdatedAndPersisted()
        {
            // Arrange
            var comic = new Comic { Title = "Old Title", Author = "Old Author", Isbn = "IT-456", Genre = "Old Genre", Price = 5.0m, Stock = 2, PublicationDate = DateTime.Now.AddDays(-10).Date };
            _comicService.AddComic(comic); // ID will be assigned here

            var updatedInfo = new Comic { Id = comic.Id, Title = "New Title", Author = "New Author", Isbn = "IT-456-Updated", Genre = "New Genre", Price = 7.5m, Stock = 3, PublicationDate = DateTime.Now.Date };

            // Act
            _comicService.UpdateComic(updatedInfo);
            var persistedComics = _fileHelper.ReadFile<Comic>(TestComicsFilePath, Comic.FromCsvString);
            var retrievedComic = _comicService.GetComicById(comic.Id);

            // Assert
            Assert.AreEqual(1, persistedComics.Count, "Only one comic should exist.");
            Assert.AreEqual("New Title", persistedComics[0].Title, "Persisted comic should have the new title.");
            Assert.IsNotNull(retrievedComic);
            Assert.AreEqual("New Title", retrievedComic.Title, "Retrieved comic should have the new title.");
            Assert.AreEqual("New Author", retrievedComic.Author);
            Assert.AreEqual(7.5m, retrievedComic.Price);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateComic_NonExistentComic_ShouldThrowException()
        {
            // Arrange
            var nonExistentComic = new Comic { Id = 999, Title = "Non Existent", Author = "Ghost Writer", Isbn = "00000", Price = 0, Stock = 0, PublicationDate = DateTime.MinValue };

            // Act
            _comicService.UpdateComic(nonExistentComic);

            // Assert - Exception expected
        }


        [TestMethod]
        public void DeleteComic_ExistingComic_ShouldBeRemovedAndPersisted()
        {
            // Arrange
            var comic1 = new Comic { Title = "To Delete", Author = "Temp Author", Isbn = "IT-DEL1", Genre = "Temp", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date };
            var comic2 = new Comic { Title = "To Keep", Author = "Perm Author", Isbn = "IT-KEEP1", Genre = "Permanent", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date };
            _comicService.AddComic(comic1);
            _comicService.AddComic(comic2);
            int idToDelete = comic1.Id;

            // Act
            _comicService.DeleteComic(idToDelete);
            var persistedComics = _fileHelper.ReadFile<Comic>(TestComicsFilePath, Comic.FromCsvString);
            var retrievedComics = _comicService.GetAllComics();

            // Assert
            Assert.AreEqual(1, persistedComics.Count, "File should only contain one comic after deletion.");
            Assert.AreEqual("To Keep", persistedComics[0].Title);
            Assert.AreEqual(1, retrievedComics.Count, "Service should only contain one comic after deletion.");
            Assert.AreEqual("To Keep", retrievedComics[0].Title);
            Assert.IsNull(_comicService.GetComicById(idToDelete), "Deleted comic should not be retrievable.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteComic_NonExistentComic_ShouldThrowException()
        {
            // Arrange
            // No comics added, or add some and try to delete one that doesn't exist
            _comicService.AddComic(new Comic { Title = "Some Comic", Author = "Some Author", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });

            // Act
            _comicService.DeleteComic(999); // Non-existent ID

            // Assert - Exception expected
        }


        [TestMethod]
        public void GetComicById_ExistingComic_ShouldReturnCorrectComic()
        {
            // Arrange
            var comic = new Comic { Title = "Find Me", Author = "Specific Author", Isbn = "IT-FINDME", Genre = "Searchable", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date };
            _comicService.AddComic(comic);
            int idToFind = comic.Id;


            // Act
            var foundComic = _comicService.GetComicById(idToFind);

            // Assert
            Assert.IsNotNull(foundComic);
            Assert.AreEqual(idToFind, foundComic.Id);
            Assert.AreEqual("Find Me", foundComic.Title);
        }

        [TestMethod]
        public void GetComicById_NonExistentComic_ShouldReturnNull()
        {
            // Arrange
            // No comics in the system, or comics that don't match the ID

            // Act
            var foundComic = _comicService.GetComicById(12345); // An ID that's unlikely to exist

            // Assert
            Assert.IsNull(foundComic);
        }

        [TestMethod]
        public void GetAllComics_MultipleComics_ShouldReturnAll()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Comic One", Author = "Author A", Isbn = "IT-ALL1", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });
            _comicService.AddComic(new Comic { Title = "Comic Two", Author = "Author B", Isbn = "IT-ALL2", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });

            // Act
            var allComics = _comicService.GetAllComics();
            var persistedComics = _fileHelper.ReadFile<Comic>(TestComicsFilePath, Comic.FromCsvString);


            // Assert
            Assert.AreEqual(2, allComics.Count, "Service should return all added comics.");
            Assert.AreEqual(2, persistedComics.Count, "File should contain all added comics.");
            Assert.IsTrue(allComics.Any(c => c.Title == "Comic One"));
            Assert.IsTrue(allComics.Any(c => c.Title == "Comic Two"));
        }

        [TestMethod]
        public void SearchComics_ByTitle_ShouldReturnMatchingComics()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Alpha Search", Author = "Author X", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });
            _comicService.AddComic(new Comic { Title = "Beta Test", Author = "Author Y", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });
            _comicService.AddComic(new Comic { Title = "Alpha Query", Author = "Author Z", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });

            // Act
            var results = _comicService.SearchComics("Alpha");

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(c => c.Title.Contains("Alpha")));
        }

        [TestMethod]
        public void GetComicsByGenre_ShouldReturnMatchingComics()
        {
            // Arrange
            _comicService.AddComic(new Comic { Title = "Genre Test A", Author = "Auth G", Genre = "Fantasy", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });
            _comicService.AddComic(new Comic { Title = "Genre Test B", Author = "Auth G", Genre = "Sci-Fi", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });
            _comicService.AddComic(new Comic { Title = "Genre Test C", Author = "Auth G", Genre = "Fantasy", Price = 1, Stock = 1, PublicationDate = DateTime.Now.Date });

            // Act
            var results = _comicService.GetComicsByGenre("Fantasy");

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(c => c.Genre == "Fantasy"));
        }
    }
}
