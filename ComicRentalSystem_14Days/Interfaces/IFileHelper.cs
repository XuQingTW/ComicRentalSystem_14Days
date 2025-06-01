using System;
using System.Collections.Generic;

namespace ComicRentalSystem_14Days.Interfaces
{
    public interface IFileHelper
    {
        string ReadFile(string fileName); 
        void WriteFile(string fileName, string content); 
        List<T> ReadFile<T>(string fileName, Func<string, T> parser); 
        void WriteFile<T>(string fileName, IEnumerable<T> items, Func<T, string> formatter); 
        string GetFullFilePath(string fileName); 

        Task<string> ReadFileAsync(string filePath);
        Task WriteFileAsync(string filePath, string content);
        bool FileExists(string filePath);
        void DeleteFile(string filePath);
        void MoveFile(string sourcePath, string destinationPath);
        void CopyFile(string sourcePath, string destinationPath, bool overwrite);
    }
}
