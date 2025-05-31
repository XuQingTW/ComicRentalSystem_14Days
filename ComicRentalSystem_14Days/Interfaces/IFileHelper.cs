using System;
using System.Collections.Generic;

namespace ComicRentalSystem_14Days.Interfaces
{
    public interface IFileHelper
    {
        string ReadFile(string fileName); // For AuthenticationService
        void WriteFile(string fileName, string content); // For AuthenticationService
        List<T> ReadFile<T>(string fileName, Func<string, T> parser); // For ComicService, MemberService
        void WriteFile<T>(string fileName, IEnumerable<T> items, Func<T, string> formatter); // For ComicService, MemberService
        string GetFullFilePath(string fileName); // Utility, might be useful for mock setup or internal use
        // bool Exists(string fileName); // Potentially useful, but not strictly needed by current services directly

        Task<string> ReadFileAsync(string filePath);
        Task WriteFileAsync(string filePath, string content);
    }
}
