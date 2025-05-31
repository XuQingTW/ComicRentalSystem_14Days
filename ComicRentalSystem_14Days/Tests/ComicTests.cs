using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Interfaces; // Added
using ComicRentalSystem_14Days.Services; // Added
using Moq; // Added
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Added

namespace ComicRentalSystem_14Days.Tests
{
    [TestClass]
    public class ComicTests
    {
        private Mock<IFileHelper> _mockFileHelper;
        private Mock<ILogger> _mockLogger;
        private const string TestComicsFilePath = "comics.csv"; // Matching ComicService

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFileHelper = new Mock<IFileHelper>();
            _mockLogger = new Mock<ILogger>();

            // Default setup for GetFullFilePath if ComicService uses it (it does implicitly via FileHelper base)
            _mockFileHelper.Setup(fh => fh.GetFullFilePath(TestComicsFilePath)).Returns(TestComicsFilePath);
        }

        // --- Existing tests for Comic model (FromCsvString, ToCsvString, ParseCsvLine) ---
        [TestMethod]
        public void FromCsvString_StandardData_ShouldParseCorrectly()
        {
            string csvLine = "1,\"The Amazing Spider-Man\",\"Stan Lee\",\"978-0785195612\",\"Superhero\",false,0";
            Comic comic = Comic.FromCsvString(csvLine);
            Assert.AreEqual(1, comic.Id);
            Assert.AreEqual("The Amazing Spider-Man", comic.Title);
            Assert.AreEqual("Stan Lee", comic.Author);
            Assert.AreEqual("978-0785195612", comic.Isbn);
            Assert.AreEqual("Superhero", comic.Genre);
            Assert.IsFalse(comic.IsRented);
            Assert.AreEqual(0, comic.RentedToMemberId);
        }

        [TestMethod]
        public void FromCsvString_DataWithCommasInQuotedFields_ShouldParseCorrectly()
        {
            string csvLine = "2,\"Batman: The Killing Joke, Deluxe Edition\",\"Alan Moore, Brian Bolland\",\"978-1401216672\",\"Graphic Novel\",true,101";
            Comic comic = Comic.FromCsvString(csvLine);
            Assert.AreEqual(2, comic.Id);
            Assert.AreEqual("Batman: The Killing Joke, Deluxe Edition", comic.Title);
            Assert.AreEqual("Alan Moore, Brian Bolland", comic.Author);
        }

        [TestMethod]
        public void FromCsvString_DataWithEscapedQuotesInFields_ShouldParseCorrectly()
        {
            string csvLine = "3,\"The \"\"Best\"\" Comic Ever\",\"A. Writer\",\"123-ABCDEF\",\"Fiction\",false,0";
            Comic comic = Comic.FromCsvString(csvLine);
            Assert.AreEqual(3, comic.Id);
            Assert.AreEqual("The \"Best\" Comic Ever", comic.Title);
        }

        [TestMethod]
        public void FromCsvString_EmptyFields_ShouldParseCorrectly()
        {
            string csvLine = "4,\"\",\"\",\"\",\"\",false,";
            Comic comic = Comic.FromCsvString(csvLine);
            Assert.AreEqual(4, comic.Id);
            Assert.AreEqual("", comic.Title);
        }

        [TestMethod]
        public void FromCsvString_RentedComicWithMemberId_ShouldParseCorrectly()
        {
            string csvLine = "5,\"Another Comic\",\"Some Author\",\"SN123\",\"GenreX\",true,55";
            Comic comic = Comic.FromCsvString(csvLine);
            Assert.AreEqual(5, comic.Id);
            Assert.IsTrue(comic.IsRented);
            Assert.AreEqual(55, comic.RentedToMemberId);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_TooFewFields_ShouldThrowFormatException()
        {
            string csvLine = "6,\"Title\",\"Author\"";
            Comic.FromCsvString(csvLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForId_ShouldThrowFormatException()
        {
            string csvLine = "\"NotANumber\",\"Title\",\"Author\",\"Isbn\",\"Genre\",false,0";
            Comic.FromCsvString(csvLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForIsRented_ShouldThrowFormatException()
        {
            string csvLine = "7,\"Title\",\"Author\",\"Isbn\",\"Genre\",\"NotBoolean\",0";
            Comic.FromCsvString(csvLine);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForRentedToMemberId_ShouldThrowFormatException()
        {
            string csvLine = "8,\"Title\",\"Author\",\"Isbn\",\"Genre\",true,\"NotAnInt\"";
            Comic.FromCsvString(csvLine);
        }

        [TestMethod]
        public void ToCsvString_StandardData_ShouldFormatCorrectly()
        {
            Comic comic = new Comic { Id = 1, Title = "The Amazing Spider-Man", Author = "Stan Lee", Isbn = "978-0785195612", Genre = "Superhero", IsRented = false, RentedToMemberId = 0 };
            string expectedCsvLine = "1,\"The Amazing Spider-Man\",\"Stan Lee\",\"978-0785195612\",\"Superhero\",False,0";
            Assert.AreEqual(expectedCsvLine, comic.ToCsvString());
        }

        [TestMethod]
        public void ToCsvString_DataWithCommasAndQuotes_ShouldFormatCorrectly()
        {
            Comic comic = new Comic { Id = 2, Title = "Batman: \"The Killing Joke\", Deluxe Edition", Author = "Alan Moore, Brian Bolland", Isbn = "978-1401216672", Genre = "Graphic Novel, Dark", IsRented = true, RentedToMemberId = 101 };
            string expectedCsvLine = "2,\"Batman: \"\"The Killing Joke\"\", Deluxe Edition\",\"Alan Moore, Brian Bolland\",\"978-1401216672\",\"Graphic Novel, Dark\",True,101";
            Assert.AreEqual(expectedCsvLine, comic.ToCsvString());
        }

        [TestMethod]
        public void ToCsvString_EmptyFields_ShouldFormatCorrectly()
        {
            Comic comic = new Comic { Id = 3, Title = "", Author = "", Isbn = "", Genre = "", IsRented = false, RentedToMemberId = 0 };
            string expectedCsvLine = "3,\"\",\"\",\"\",\"\",False,0";
            Assert.AreEqual(expectedCsvLine, comic.ToCsvString());
        }

        [TestMethod]
        public void Comic_RoundTripTest_StandardData()
        {
            Comic originalComic = new Comic { Id = 10, Title = "Round Trip Title", Author = "Round Trip Author", Isbn = "RT-12345", Genre = "Test Genre", IsRented = false, RentedToMemberId = 0 };
            Comic processedComic = Comic.FromCsvString(originalComic.ToCsvString());
            Assert.AreEqual(originalComic.Title, processedComic.Title);
        }

        [TestMethod]
        public void Comic_RoundTripTest_WithQuotesAndCommas()
        {
            Comic originalComic = new Comic { Id = 11, Title = "A \"Quoted\" Title, with Commas", Author = "Author, Sr.", Isbn = "ISBN, \"12345\"", Genre = "Complex, \"Genre\"", IsRented = true, RentedToMemberId = 77 };
            Comic processedComic = Comic.FromCsvString(originalComic.ToCsvString());
            Assert.AreEqual(originalComic.Title, processedComic.Title);
        }

        [TestMethod]
        public void Comic_RoundTripTest_EmptyRentedToMemberId()
        {
            Comic originalComic = new Comic { Id = 12, Title = "Not Rented Comic", Author = "N. A.", Isbn = "NR-001", Genre = "Availability", IsRented = false };
            Comic processedComic = Comic.FromCsvString(originalComic.ToCsvString());
            Assert.AreEqual(0, processedComic.RentedToMemberId);
        }

        [TestMethod]
        public void Test_ParseCsvLine_SpacesInQuotedField()
        {
            CollectionAssert.AreEqual(new List<string> { "  leading space", "field two", "trailing space  " }, Comic.ParseCsvLine("\"  leading space\",\"field two\",\"trailing space  \""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_SpacesInUnquotedField()
        {
            CollectionAssert.AreEqual(new List<string> { "unquoted leading", "field two", "unquoted trailing" }, Comic.ParseCsvLine("  unquoted leading,field two  ,unquoted trailing  "));
        }

        [TestMethod]
        public void Test_ParseCsvLine_FieldWithCommaQuoted()
        {
            CollectionAssert.AreEqual(new List<string> { "field1, with comma", "field2" }, Comic.ParseCsvLine("\"field1, with comma\",\"field2\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_EscapedQuotes()
        {
            CollectionAssert.AreEqual(new List<string> { "field with \"escaped\" quotes", "field2" }, Comic.ParseCsvLine("\"field with \"\"escaped\"\" quotes\",\"field2\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_EmptyQuotedField()
        {
            CollectionAssert.AreEqual(new List<string> { "", "field2" }, Comic.ParseCsvLine("\"\",\"field2\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_EmptyUnquotedField()
        {
            CollectionAssert.AreEqual(new List<string> { "", "field2" }, Comic.ParseCsvLine(",field2"));
        }

        [TestMethod]
        public void Test_ParseCsvLine_AllEmptyFields_Quoted()
        {
            CollectionAssert.AreEqual(new List<string> { "", "", "" }, Comic.ParseCsvLine("\"\",,\"\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_AllEmptyFields_Unquoted()
        {
            CollectionAssert.AreEqual(new List<string> { "", "", "" }, Comic.ParseCsvLine(",,"));
        }

        [TestMethod]
        public void Test_ParseCsvLine_MixedQuotedAndUnquoted_WithSpaces()
        {
            CollectionAssert.AreEqual(new List<string> { "field1", "  field2 with spaces  ", "field3" }, Comic.ParseCsvLine("  field1  ,\"  field2 with spaces  \", field3  "));
        }

        // --- New tests for ComicService.LoadComics ---

        [TestMethod]
        public void Test_LoadComics_FileNotFound()
        {
            _mockFileHelper.Setup(fh => fh.ReadFile<Comic>(TestComicsFilePath, It.IsAny<Func<string, Comic>>()))
                           .Returns(new List<Comic>()); // FileHelper returns empty list for not found

            var comicService = new ComicService(_mockFileHelper.Object, _mockLogger.Object);

            Assert.AreEqual(0, comicService.GetAllComics().Count);
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains("Successfully loaded 0 comics"))), Times.Once);
        }

        [TestMethod]
        public void Test_LoadComics_EmptyFile()
        {
            _mockFileHelper.Setup(fh => fh.ReadFile<Comic>(TestComicsFilePath, It.IsAny<Func<string, Comic>>()))
                           .Returns(new List<Comic>()); // Empty file results in empty list

            var comicService = new ComicService(_mockFileHelper.Object, _mockLogger.Object);

            Assert.AreEqual(0, comicService.GetAllComics().Count);
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains("Successfully loaded 0 comics"))), Times.Once);
        }

        [TestMethod]
        public void Test_LoadComics_CorruptedCsvFile_FormatException()
        {
            var formatException = new FormatException("Invalid CSV line");
            _mockFileHelper.Setup(fh => fh.ReadFile<Comic>(TestComicsFilePath, It.IsAny<Func<string, Comic>>()))
                           .Throws(formatException);

            var appEx = Assert.ThrowsException<ApplicationException>(() =>
                new ComicService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.IsTrue(appEx.Message.Contains($"Failed to load comic data from '{TestComicsFilePath}'"));
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains($"Critical error: Comic data file '{TestComicsFilePath}' is corrupted or unreadable.")), formatException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadComics_CorruptedCsvFile_IOException()
        {
            var ioException = new IOException("Disk error");
            _mockFileHelper.Setup(fh => fh.ReadFile<Comic>(TestComicsFilePath, It.IsAny<Func<string, Comic>>()))
                           .Throws(ioException);

            var appEx = Assert.ThrowsException<ApplicationException>(() =>
                new ComicService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.IsTrue(appEx.Message.Contains($"Failed to load comic data from '{TestComicsFilePath}'"));
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains($"Critical error: Comic data file '{TestComicsFilePath}' is corrupted or unreadable.")), ioException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadComics_OtherUnexpectedException()
        {
            var genericException = new Exception("Unexpected problem");
            _mockFileHelper.Setup(fh => fh.ReadFile<Comic>(TestComicsFilePath, It.IsAny<Func<string, Comic>>()))
                           .Throws(genericException);

            var appEx = Assert.ThrowsException<ApplicationException>(() =>
                new ComicService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.AreEqual("Unexpected error during comic data loading.", appEx.Message);
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains($"An unexpected error occurred while loading comics from {TestComicsFilePath}")), genericException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadComics_SuccessfulLoad()
        {
            var sampleComics = new List<Comic>
            {
                new Comic { Id = 1, Title = "Test Comic 1", Author = "Auth1" },
                new Comic { Id = 2, Title = "Test Comic 2", Author = "Auth2" }
            };
            _mockFileHelper.Setup(fh => fh.ReadFile<Comic>(TestComicsFilePath, It.IsAny<Func<string, Comic>>()))
                           .Returns(sampleComics);

            var comicService = new ComicService(_mockFileHelper.Object, _mockLogger.Object);

            CollectionAssert.AreEqual(sampleComics, comicService.GetAllComics());
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains($"Successfully loaded {sampleComics.Count} comics"))), Times.Once);
        }
    }
}
