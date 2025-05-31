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
    public class MemberTests
    {
        private Mock<IFileHelper> _mockFileHelper;
        private Mock<ILogger> _mockLogger;
        private const string TestMembersFilePath = "members.csv"; // Matching MemberService

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFileHelper = new Mock<IFileHelper>();
            _mockLogger = new Mock<ILogger>();

            _mockFileHelper.Setup(fh => fh.GetFullFilePath(TestMembersFilePath)).Returns(TestMembersFilePath);
        }

        // --- Existing tests for Member model (FromCsvString, ToCsvString, ParseCsvLine) ---
        [TestMethod]
        public void FromCsvString_StandardData_ShouldParseCorrectly()
        {
            string csvLine = "1,\"John Doe\",\"123-456-7890\"";
            Member member = Member.FromCsvString(csvLine);
            Assert.AreEqual(1, member.Id);
            Assert.AreEqual("John Doe", member.Name);
        }

        [TestMethod]
        public void FromCsvString_DataWithCommasInQuotedName_ShouldParseCorrectly()
        {
            string csvLine = "2,\"Doe, John\",\"234-567-8901\"";
            Member member = Member.FromCsvString(csvLine);
            Assert.AreEqual(2, member.Id);
            Assert.AreEqual("Doe, John", member.Name);
        }

        [TestMethod]
        public void FromCsvString_DataWithEscapedQuotesInName_ShouldParseCorrectly()
        {
            string csvLine = "3,\"Jane \"\"The Rock\"\" Doe\",\"345-678-9012\"";
            Member member = Member.FromCsvString(csvLine);
            Assert.AreEqual(3, member.Id);
            Assert.AreEqual("Jane \"The Rock\" Doe", member.Name);
        }

        [TestMethod]
        public void FromCsvString_EmptyFields_ShouldParseCorrectly()
        {
            string csvLine = "4,\"\",\"\"";
            Member member = Member.FromCsvString(csvLine);
            Assert.AreEqual(4, member.Id);
            Assert.AreEqual("", member.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_TooFewFields_ShouldThrowFormatException()
        {
            Member.FromCsvString("5,\"Just Name\"");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FromCsvString_MalformedLine_IncorrectDataTypeForId_ShouldThrowFormatException()
        {
            Member.FromCsvString("\"NotANumber\",\"Some Name\",\"555-5555\"");
        }

        [TestMethod]
        public void ToCsvString_StandardData_ShouldFormatCorrectly()
        {
            Member member = new Member { Id = 1, Name = "John Doe", PhoneNumber = "123-456-7890" };
            Assert.AreEqual("1,\"John Doe\",\"123-456-7890\"", member.ToCsvString());
        }

        [TestMethod]
        public void ToCsvString_DataWithCommasAndQuotes_ShouldFormatCorrectly()
        {
            Member member = new Member { Id = 2, Name = "Doe, \"The Hammer\" John", PhoneNumber = "987-654-3210, Ext 123" };
            Assert.AreEqual("2,\"Doe, \"\"The Hammer\"\" John\",\"987-654-3210, Ext 123\"", member.ToCsvString());
        }

        [TestMethod]
        public void ToCsvString_EmptyFields_ShouldFormatCorrectly()
        {
            Member member = new Member { Id = 3, Name = "", PhoneNumber = "" };
            Assert.AreEqual("3,\"\",\"\"", member.ToCsvString());
        }

        [TestMethod]
        public void Member_RoundTripTest_StandardData()
        {
            Member originalMember = new Member { Id = 10, Name = "Alice Wonderland", PhoneNumber = "555-0123" };
            Member processedMember = Member.FromCsvString(originalMember.ToCsvString());
            Assert.AreEqual(originalMember.Name, processedMember.Name);
        }

        [TestMethod]
        public void Member_RoundTripTest_WithQuotesAndCommas()
        {
            Member originalMember = new Member { Id = 11, Name = "Bob \"The Builder\", Jr.", PhoneNumber = "555-0456, cell" };
            Member processedMember = Member.FromCsvString(originalMember.ToCsvString());
            Assert.AreEqual(originalMember.Name, processedMember.Name);
        }

        [TestMethod]
        public void Test_ParseCsvLine_SpacesInQuotedField_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "  leading space", "field two", "trailing space  " }, Member.ParseCsvLine("\"  leading space\",\"field two\",\"trailing space  \""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_SpacesInUnquotedField_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "unquoted leading", "field two", "unquoted trailing" }, Member.ParseCsvLine("  unquoted leading,field two  ,unquoted trailing  "));
        }

        [TestMethod]
        public void Test_ParseCsvLine_FieldWithCommaQuoted_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "field1, with comma", "field2" }, Member.ParseCsvLine("\"field1, with comma\",\"field2\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_EscapedQuotes_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "field with \"escaped\" quotes", "field2" }, Member.ParseCsvLine("\"field with \"\"escaped\"\" quotes\",\"field2\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_EmptyQuotedField_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "", "field2" }, Member.ParseCsvLine("\"\",\"field2\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_EmptyUnquotedField_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "", "field2" }, Member.ParseCsvLine(",field2"));
        }

        [TestMethod]
        public void Test_ParseCsvLine_AllEmptyFields_Quoted_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "", "", "" }, Member.ParseCsvLine("\"\",,\"\""));
        }

        [TestMethod]
        public void Test_ParseCsvLine_AllEmptyFields_Unquoted_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "", "", "" }, Member.ParseCsvLine(",,"));
        }

        [TestMethod]
        public void Test_ParseCsvLine_MixedQuotedAndUnquoted_WithSpaces_Member()
        {
            CollectionAssert.AreEqual(new List<string> { "field1", "  field2 with spaces  ", "field3" }, Member.ParseCsvLine("  field1  ,\"  field2 with spaces  \", field3  "));
        }

        // --- New tests for MemberService.LoadMembers ---

        [TestMethod]
        public void Test_LoadMembers_FileNotFound()
        {
            _mockFileHelper.Setup(fh => fh.ReadFile<Member>(TestMembersFilePath, It.IsAny<Func<string, Member>>()))
                           .Returns(new List<Member>());

            var memberService = new MemberService(_mockFileHelper.Object, _mockLogger.Object);

            Assert.AreEqual(0, memberService.GetAllMembers().Count);
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains("Successfully loaded 0 members"))), Times.Once);
        }

        [TestMethod]
        public void Test_LoadMembers_EmptyFile()
        {
            _mockFileHelper.Setup(fh => fh.ReadFile<Member>(TestMembersFilePath, It.IsAny<Func<string, Member>>()))
                           .Returns(new List<Member>());

            var memberService = new MemberService(_mockFileHelper.Object, _mockLogger.Object);

            Assert.AreEqual(0, memberService.GetAllMembers().Count);
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains("Successfully loaded 0 members"))), Times.Once);
        }

        [TestMethod]
        public void Test_LoadMembers_CorruptedCsvFile_FormatException()
        {
            var formatException = new FormatException("Invalid CSV line for Member");
            _mockFileHelper.Setup(fh => fh.ReadFile<Member>(TestMembersFilePath, It.IsAny<Func<string, Member>>()))
                           .Throws(formatException);

            var appEx = Assert.ThrowsException<ApplicationException>(() =>
                new MemberService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.IsTrue(appEx.Message.Contains($"Failed to load member data from '{TestMembersFilePath}'"));
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains($"Critical error: Member data file '{TestMembersFilePath}' is corrupted or unreadable.")), formatException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadMembers_CorruptedCsvFile_IOException()
        {
            var ioException = new IOException("Disk error for Member read");
            _mockFileHelper.Setup(fh => fh.ReadFile<Member>(TestMembersFilePath, It.IsAny<Func<string, Member>>()))
                           .Throws(ioException);

            var appEx = Assert.ThrowsException<ApplicationException>(() =>
                new MemberService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.IsTrue(appEx.Message.Contains($"Failed to load member data from '{TestMembersFilePath}'"));
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains($"Critical error: Member data file '{TestMembersFilePath}' is corrupted or unreadable.")), ioException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadMembers_OtherUnexpectedException()
        {
            var genericException = new Exception("Unexpected problem for Member");
            _mockFileHelper.Setup(fh => fh.ReadFile<Member>(TestMembersFilePath, It.IsAny<Func<string, Member>>()))
                           .Throws(genericException);

            var appEx = Assert.ThrowsException<ApplicationException>(() =>
                new MemberService(_mockFileHelper.Object, _mockLogger.Object)
            );

            Assert.AreEqual("Unexpected error during member data loading.", appEx.Message);
            _mockLogger.Verify(log => log.LogError(It.Is<string>(s => s.Contains($"An unexpected error occurred while loading members from {TestMembersFilePath}")), genericException), Times.Once);
        }

        [TestMethod]
        public void Test_LoadMembers_SuccessfulLoad()
        {
            var sampleMembers = new List<Member>
            {
                new Member { Id = 1, Name = "Member One", PhoneNumber = "111-1111" },
                new Member { Id = 2, Name = "Member Two", PhoneNumber = "222-2222" }
            };
            _mockFileHelper.Setup(fh => fh.ReadFile<Member>(TestMembersFilePath, It.IsAny<Func<string, Member>>()))
                           .Returns(sampleMembers);

            var memberService = new MemberService(_mockFileHelper.Object, _mockLogger.Object);

            CollectionAssert.AreEqual(sampleMembers, memberService.GetAllMembers());
            _mockLogger.Verify(log => log.Log(It.Is<string>(s => s.Contains($"Successfully loaded {sampleMembers.Count} members"))), Times.Once);
        }
    }
}
