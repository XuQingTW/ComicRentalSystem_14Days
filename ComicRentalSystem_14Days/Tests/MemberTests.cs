using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicRentalSystem_14Days.Models;
using System;

namespace ComicRentalSystem_14Days.Tests
{
    [TestClass]
    public class MemberTests
    {
        [TestMethod]
        public void FromCsvString_StandardData_ShouldParseCorrectly()
        {
            // Arrange
            string csvLine = "1,\"John Doe\",\"123-456-7890\"";

            // Act
            Member member = Member.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(1, member.Id);
            Assert.AreEqual("John Doe", member.Name);
            Assert.AreEqual("123-456-7890", member.PhoneNumber);
        }

        [TestMethod]
        public void FromCsvString_DataWithCommasInQuotedName_ShouldParseCorrectly()
        {
            // Arrange
            string csvLine = "2,\"Doe, John\",\"234-567-8901\"";

            // Act
            Member member = Member.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(2, member.Id);
            Assert.AreEqual("Doe, John", member.Name);
            Assert.AreEqual("234-567-8901", member.PhoneNumber);
        }

        [TestMethod]
        public void FromCsvString_DataWithEscapedQuotesInName_ShouldParseCorrectly()
        {
            // Arrange
            string csvLine = "3,\"Jane \"\"The Rock\"\" Doe\",\"345-678-9012\"";

            // Act
            Member member = Member.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(3, member.Id);
            Assert.AreEqual("Jane \"The Rock\" Doe", member.Name);
            Assert.AreEqual("345-678-9012", member.PhoneNumber);
        }

        [TestMethod]
        public void FromCsvString_EmptyFields_ShouldParseCorrectly()
        {
            // Arrange
            // Name and PhoneNumber can be empty.
            string csvLine = "4,\"\",\"\"";

            // Act
            Member member = Member.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(4, member.Id);
            Assert.AreEqual("", member.Name);
            Assert.AreEqual("", member.PhoneNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_TooFewFields_ShouldThrowFormatException()
        {
            // Arrange
            string csvLine = "5,\"Just Name\""; // Missing PhoneNumber

            // Act
            Member.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForId_ShouldThrowFormatException()
        {
            // Arrange
            string csvLine = "\"NotANumber\",\"Some Name\",\"555-5555\"";

            // Act
            Member.FromCsvString(csvLine);

            // Assert - Exception expected
        }

        [TestMethod]
        public void ToCsvString_StandardData_ShouldFormatCorrectly()
        {
            // Arrange
            Member member = new Member
            {
                Id = 1,
                Name = "John Doe",
                PhoneNumber = "123-456-7890"
            };
            string expectedCsvLine = "1,\"John Doe\",\"123-456-7890\"";

            // Act
            string csvLine = member.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsvLine, csvLine);
        }

        [TestMethod]
        public void ToCsvString_DataWithCommasAndQuotes_ShouldFormatCorrectly()
        {
            // Arrange
            Member member = new Member
            {
                Id = 2,
                Name = "Doe, \"The Hammer\" John",
                PhoneNumber = "987-654-3210, Ext 123"
            };
            string expectedCsvLine = "2,\"Doe, \"\"The Hammer\"\" John\",\"987-654-3210, Ext 123\"";

            // Act
            string csvLine = member.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsvLine, csvLine);
        }

        [TestMethod]
        public void ToCsvString_EmptyFields_ShouldFormatCorrectly()
        {
            // Arrange
            Member member = new Member
            {
                Id = 3,
                Name = "",
                PhoneNumber = ""
            };
            string expectedCsvLine = "3,\"\",\"\"";

            // Act
            string csvLine = member.ToCsvString();

            // Assert
            Assert.AreEqual(expectedCsvLine, csvLine);
        }

        [TestMethod]
        public void Member_RoundTripTest_StandardData()
        {
            // Arrange
            Member originalMember = new Member
            {
                Id = 10,
                Name = "Alice Wonderland",
                PhoneNumber = "555-0123"
            };

            // Act
            string csvLine = originalMember.ToCsvString();
            Member processedMember = Member.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(originalMember.Id, processedMember.Id);
            Assert.AreEqual(originalMember.Name, processedMember.Name);
            Assert.AreEqual(originalMember.PhoneNumber, processedMember.PhoneNumber);
        }

        [TestMethod]
        public void Member_RoundTripTest_WithQuotesAndCommas()
        {
            // Arrange
            Member originalMember = new Member
            {
                Id = 11,
                Name = "Bob \"The Builder\", Jr.",
                PhoneNumber = "555-0456, cell"
            };

            // Act
            string csvLine = originalMember.ToCsvString();
            Member processedMember = Member.FromCsvString(csvLine);

            // Assert
            Assert.AreEqual(originalMember.Id, processedMember.Id);
            Assert.AreEqual(originalMember.Name, processedMember.Name);
            Assert.AreEqual(originalMember.PhoneNumber, processedMember.PhoneNumber);
        }
    }
}
