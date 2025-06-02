using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Models;
using System;
using System.Globalization; // Required for DateTime parsing assertions

namespace ComicRentalSystem_14Days.Tests
{
    [TestClass]
    public class ComicTests
    {
        [TestMethod]
        public void ToCsvString_ValidComic_ReturnsCorrectCsvFormat()
        {
            // Arrange
            var comic = new Comic
            {
                Id = 1,
                Title = "Spider-Man",
                Author = "Stan Lee",
                Isbn = "1234567890",
                Genre = "Superhero",
                IsRented = false,
                RentedToMemberId = 0,
                RentalDate = new DateTime(2023, 1, 15, 10, 30, 0, DateTimeKind.Unspecified),
                ReturnDate = new DateTime(2023, 2, 15, 10, 30, 0, DateTimeKind.Unspecified),
                ActualReturnTime = null
            };
            var expectedCsv = "1,\"Spider-Man\",\"Stan Lee\",\"1234567890\",\"Superhero\",False,0,2023-01-15T10:30:00,2023-02-15T10:30:00,";

            // Act
            var actualCsv = comic.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsv, actualCsv);
        }

        [TestMethod]
        public void ToCsvString_ComicWithQuotesInTitle_ReturnsCsvWithEscapedQuotes()
        {
            // Arrange
            var comic = new Comic
            {
                Id = 2,
                Title = "The \"Amazing\" Spider-Man",
                Author = "Stan Lee",
                Isbn = "0987654321",
                Genre = "Superhero",
                IsRented = true,
                RentedToMemberId = 101,
                RentalDate = null,
                ReturnDate = null,
                ActualReturnTime = null
            };
            var expectedCsv = "2,\"The \"\"Amazing\"\" Spider-Man\",\"Stan Lee\",\"0987654321\",\"Superhero\",True,101,,,";


            // Act
            var actualCsv = comic.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsv, actualCsv);
        }

        [TestMethod]
        public void ToCsvString_ComicWithNullDates_ReturnsCsvWithEmptyDateFields()
        {
            // Arrange
            var comic = new Comic
            {
                Id = 3,
                Title = "Hulk",
                Author = "Stan Lee",
                Isbn = "1122334455",
                Genre = "Superhero",
                IsRented = false,
                RentedToMemberId = 0,
                RentalDate = null,
                ReturnDate = null,
                ActualReturnTime = null
            };
            var expectedCsv = "3,\"Hulk\",\"Stan Lee\",\"1122334455\",\"Superhero\",False,0,,,";

            // Act
            var actualCsv = comic.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsv, actualCsv);
        }

        [TestMethod]
        public void FromCsvString_ValidLine_PopulatesComicCorrectly()
        {
            // Arrange
            var csvLine = "1,\"Spider-Man\",\"Stan Lee\",\"1234567890\",\"Superhero\",False,0,2023-01-15T10:30:00,2023-02-15T10:30:00,2023-02-14T12:00:00";

            // Act
            var comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(1, comic.Id);
            Assert.AreEqual("Spider-Man", comic.Title);
            Assert.AreEqual("Stan Lee", comic.Author);
            Assert.AreEqual("1234567890", comic.Isbn);
            Assert.AreEqual("Superhero", comic.Genre);
            Assert.IsFalse(comic.IsRented);
            Assert.AreEqual(0, comic.RentedToMemberId);
            Assert.AreEqual(new DateTime(2023, 1, 15, 10, 30, 0), comic.RentalDate);
            Assert.AreEqual(new DateTime(2023, 2, 15, 10, 30, 0), comic.ReturnDate);
            Assert.AreEqual(new DateTime(2023, 2, 14, 12, 0, 0), comic.ActualReturnTime);
        }

        [TestMethod]
        public void FromCsvString_LineWithMissingOptionalDates_PopulatesComicCorrectly()
        {
            // Arrange
            var csvLine = "2,\"Hulk\",\"Stan Lee\",\"1122334455\",\"Superhero\",True,101,,,";

            // Act
            var comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(2, comic.Id);
            Assert.AreEqual("Hulk", comic.Title);
            Assert.AreEqual("Stan Lee", comic.Author);
            Assert.IsTrue(comic.IsRented);
            Assert.AreEqual(101, comic.RentedToMemberId);
            Assert.IsNull(comic.RentalDate);
            Assert.IsNull(comic.ReturnDate);
            Assert.IsNull(comic.ActualReturnTime);
        }

        [TestMethod]
        public void FromCsvString_LineWithEmptyRentedToMemberId_PopulatesAsZero()
        {
            // Arrange
            var csvLine = "3,\"Captain America\",\"Joe Simon\",\"2233445566\",\"Superhero\",False,,,,";

            // Act
            var comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(3, comic.Id);
            Assert.AreEqual("Captain America", comic.Title);
            Assert.AreEqual(0, comic.RentedToMemberId);
            Assert.IsFalse(comic.IsRented);
            Assert.IsNull(comic.RentalDate);
            Assert.IsNull(comic.ReturnDate);
            Assert.IsNull(comic.ActualReturnTime);
        }

        [TestMethod]
        public void FromCsvString_LineWithQuotedFieldsContainingCommasAndQuotes_PopulatesComicCorrectly()
        {
            // Arrange
            var csvLine = "4,\"Alias, Vol. 1: \"\"The Pulse\"\"\",\"Bendis, Brian Michael\",\"3344556677\",\"Crime Noir\",False,0,,,";

            // Act
            var comic = Comic.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(4, comic.Id);
            Assert.AreEqual("Alias, Vol. 1: \"The Pulse\"", comic.Title);
            Assert.AreEqual("Bendis, Brian Michael", comic.Author);
            Assert.AreEqual("Crime Noir", comic.Genre);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_InsufficientFields_ThrowsFormatException()
        {
            // Arrange
            var csvLine = "5,\"Too Few\"";

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForId_ThrowsFormatException()
        {
            // Arrange
            var csvLine = "\"NotANumber\",\"Title\",\"Author\",\"Isbn\",\"Genre\",False,0,,,";

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForIsRented_ThrowsFormatException()
        {
            // Arrange
            var csvLine = "6,\"Title\",\"Author\",\"Isbn\",\"Genre\",\"NotABoolean\",0,,,";

            // Act
            Comic.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        public void ParseCsvLine_HandlesVariousComplexities()
        {
            // Test cases for Comic.ParseCsvLine (which is internal, so we test it via FromCsvString or make it internal visible for testing)
            var csvLineWithEmptyFields = "7,\"Title\",\"Author\",,,False,0,,,";
            var comic1 = Comic.FromCsvString(csvLineWithEmptyFields);
            Assert.AreEqual(string.Empty, comic1.Isbn);
            Assert.AreEqual(string.Empty, comic1.Genre);

            var csvLineWithSpaces = "8 , \"Spaced Title\" , \"Spaced Author\" , spaced_isbn , spaced_genre , False , 0 , ,,";
            var comic2 = Comic.FromCsvString(csvLineWithSpaces);
            Assert.AreEqual(8, comic2.Id);
            Assert.AreEqual("Spaced Title", comic2.Title);
            Assert.AreEqual("Spaced Author", comic2.Author);
            Assert.AreEqual("spaced_isbn", comic2.Isbn);
            Assert.AreEqual("spaced_genre", comic2.Genre);
        }
    }
}
