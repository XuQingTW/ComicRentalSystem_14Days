using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Models;
using System;

namespace ComicRentalSystem_14Days.Tests
{
    [TestClass]
    public class ComicTests
    {
        [TestMethod]
        public void FromCsvString_StandardData_ShouldParseCorrectly()
        {
            // Arrange
            string csvLine = "1,\"The Amazing Spider-Man\",\"Stan Lee\",\"978-0785195612\",\"Superhero\",false,0";

            // Act
            Comic comic = Comic.FromCsvString(csvLine);

            // Assert
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
            // Arrange
            string csvLine = "2,\"Batman: The Killing Joke, Deluxe Edition\",\"Alan Moore, Brian Bolland\",\"978-1401216672\",\"Graphic Novel\",true,101";

            // Act
            Comic comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(2, comic.Id);
            Assert.AreEqual("Batman: The Killing Joke, Deluxe Edition", comic.Title);
            Assert.AreEqual("Alan Moore, Brian Bolland", comic.Author);
            Assert.AreEqual("978-1401216672", comic.Isbn);
            Assert.AreEqual("Graphic Novel", comic.Genre);
            Assert.IsTrue(comic.IsRented);
            Assert.AreEqual(101, comic.RentedToMemberId);
        }

        [TestMethod]
        public void FromCsvString_DataWithEscapedQuotesInFields_ShouldParseCorrectly()
        {
            // Arrange
            string csvLine = "3,\"The \"\"Best\"\" Comic Ever\",\"A. Writer\",\"123-ABCDEF\",\"Fiction\",false,0";

            // Act
            Comic comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(3, comic.Id);
            Assert.AreEqual("The \"Best\" Comic Ever", comic.Title);
            Assert.AreEqual("A. Writer", comic.Author);
            Assert.AreEqual("123-ABCDEF", comic.Isbn);
            Assert.AreEqual("Fiction", comic.Genre);
            Assert.IsFalse(comic.IsRented);
            Assert.AreEqual(0, comic.RentedToMemberId);
        }

        [TestMethod]
        public void FromCsvString_EmptyFields_ShouldParseCorrectly()
        {
            // Arrange
            // Title, Author, Isbn, Genre can be empty. RentedToMemberId can be effectively empty for not rented.
            string csvLine = "4,\"\",\"\",\"\",\"\",false,"; // RentedToMemberId is empty, should default to 0

            // Act
            Comic comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(4, comic.Id);
            Assert.AreEqual("", comic.Title);
            Assert.AreEqual("", comic.Author);
            Assert.AreEqual("", comic.Isbn);
            Assert.AreEqual("", comic.Genre);
            Assert.IsFalse(comic.IsRented);
            Assert.AreEqual(0, comic.RentedToMemberId);
        }

        [TestMethod]
        public void FromCsvString_RentedComicWithMemberId_ShouldParseCorrectly()
        {
            // Arrange
            string csvLine = "5,\"Another Comic\",\"Some Author\",\"SN123\",\"GenreX\",true,55";

            // Act
            Comic comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(5, comic.Id);
            Assert.AreEqual("Another Comic", comic.Title);
            Assert.AreEqual("Some Author", comic.Author);
            Assert.AreEqual("SN123", comic.Isbn);
            Assert.AreEqual("GenreX", comic.Genre);
            Assert.IsTrue(comic.IsRented);
            Assert.AreEqual(55, comic.RentedToMemberId);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_TooFewFields_ShouldThrowFormatException()
        {
            // Arrange
            string csvLine = "6,\"Title\",\"Author\""; // Missing fields

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForId_ShouldThrowFormatException()
        {
            // Arrange
            string csvLine = "\"NotANumber\",\"Title\",\"Author\",\"Isbn\",\"Genre\",false,0";

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForIsRented_ShouldThrowFormatException()
        {
            // Arrange
            string csvLine = "7,\"Title\",\"Author\",\"Isbn\",\"Genre\",\"NotBoolean\",0";

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForRentedToMemberId_ShouldThrowFormatException()
        {
            // Arrange
            string csvLine = "8,\"Title\",\"Author\",\"Isbn\",\"Genre\",true,\"NotAnInt\"";

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        public void ToCsvString_StandardData_ShouldFormatCorrectly()
        {
            // Arrange
            Comic comic = new Comic
            {
                Id = 1,
                Title = "The Amazing Spider-Man",
                Author = "Stan Lee",
                Isbn = "978-0785195612",
                Genre = "Superhero",
                IsRented = false,
                RentedToMemberId = 0
            };
            string expectedCsvLine = "1,\"The Amazing Spider-Man\",\"Stan Lee\",\"978-0785195612\",\"Superhero\",False,0";


            // Act
            string csvLine = comic.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsvLine, csvLine);
        }

        [TestMethod]
        public void ToCsvString_DataWithCommasAndQuotes_ShouldFormatCorrectly()
        {
            // Arrange
            Comic comic = new Comic
            {
                Id = 2,
                Title = "Batman: \"The Killing Joke\", Deluxe Edition",
                Author = "Alan Moore, Brian Bolland",
                Isbn = "978-1401216672",
                Genre = "Graphic Novel, Dark",
                IsRented = true,
                RentedToMemberId = 101
            };
            // Note: Boolean.ToString() is "True" or "False" (capitalized)
            string expectedCsvLine = "2,\"Batman: \"\"The Killing Joke\"\", Deluxe Edition\",\"Alan Moore, Brian Bolland\",\"978-1401216672\",\"Graphic Novel, Dark\",True,101";

            // Act
            string csvLine = comic.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsvLine, csvLine);
        }

        [TestMethod]
        public void ToCsvString_EmptyFields_ShouldFormatCorrectly()
        {
            // Arrange
            Comic comic = new Comic
            {
                Id = 3,
                Title = "",
                Author = "",
                Isbn = "",
                Genre = "",
                IsRented = false,
                RentedToMemberId = 0
            };
            string expectedCsvLine = "3,\"\",\"\",\"\",\"\",False,0";

            // Act
            string csvLine = comic.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsvLine, csvLine);
        }

        [TestMethod]
        public void Comic_RoundTripTest_StandardData()
        {
            // Arrange
            Comic originalComic = new Comic
            {
                Id = 10,
                Title = "Round Trip Title",
                Author = "Round Trip Author",
                Isbn = "RT-12345",
                Genre = "Test Genre",
                IsRented = false,
                RentedToMemberId = 0
            };

            // Act
            string csvLine = originalComic.ToCsvString();
            Comic processedComic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(originalComic.Id, processedComic.Id);
            Assert.AreEqual(originalComic.Title, processedComic.Title);
            Assert.AreEqual(originalComic.Author, processedComic.Author);
            Assert.AreEqual(originalComic.Isbn, processedComic.Isbn);
            Assert.AreEqual(originalComic.Genre, processedComic.Genre);
            Assert.AreEqual(originalComic.IsRented, processedComic.IsRented);
            Assert.AreEqual(originalComic.RentedToMemberId, processedComic.RentedToMemberId);
        }

        [TestMethod]
        public void Comic_RoundTripTest_WithQuotesAndCommas()
        {
            // Arrange
            Comic originalComic = new Comic
            {
                Id = 11,
                Title = "A \"Quoted\" Title, with Commas",
                Author = "Author, Sr.",
                Isbn = "ISBN, \"12345\"",
                Genre = "Complex, \"Genre\"",
                IsRented = true,
                RentedToMemberId = 77
            };

            // Act
            string csvLine = originalComic.ToCsvString();
            Comic processedComic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(originalComic.Id, processedComic.Id);
            Assert.AreEqual(originalComic.Title, processedComic.Title);
            Assert.AreEqual(originalComic.Author, processedComic.Author);
            Assert.AreEqual(originalComic.Isbn, processedComic.Isbn);
            Assert.AreEqual(originalComic.Genre, processedComic.Genre);
            Assert.AreEqual(originalComic.IsRented, processedComic.IsRented);
            Assert.AreEqual(originalComic.RentedToMemberId, processedComic.RentedToMemberId);
        }

        [TestMethod]
        public void Comic_RoundTripTest_EmptyRentedToMemberId()
        {
            // Arrange
            Comic originalComic = new Comic
            {
                Id = 12,
                Title = "Not Rented Comic",
                Author = "N. A.",
                Isbn = "NR-001",
                Genre = "Availability",
                IsRented = false,
                // RentedToMemberId will be 0 by default or if explicitly set
            };
            // ToCsvString for RentedToMemberId = 0 will be ",0"
            // FromCsvString for ",0" or "" for RentedToMemberId (when IsRented=false) should result in 0.
            // The FromCsvString logic is: string.IsNullOrEmpty(values[6]) ? 0 : int.Parse(values[6])

            // Act
            string csvLine = originalComic.ToCsvString(); // Will produce ...,False,0
            Comic processedComic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(originalComic.Id, processedComic.Id);
            Assert.AreEqual(originalComic.Title, processedComic.Title);
            Assert.AreEqual(originalComic.Author, processedComic.Author);
            Assert.AreEqual(originalComic.Isbn, processedComic.Isbn);
            Assert.AreEqual(originalComic.Genre, processedComic.Genre);
            Assert.AreEqual(originalComic.IsRented, processedComic.IsRented); // False
            Assert.AreEqual(0, processedComic.RentedToMemberId); // Should be 0
        }
    }
}
