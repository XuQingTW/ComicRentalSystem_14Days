using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Tests.Mocks
{
    public class MockFileHelper : IFileHelper
    {
        // Dictionary to store file content in memory
        public Dictionary<string, string> FileContents = new Dictionary<string, string>();
        // Dictionary to store lines for generic ReadFile<T>
        public Dictionary<string, List<string>> FileLinesForGenericRead = new Dictionary<string, List<string>>();

        // Properties to control mock behavior
        public Func<string, bool>? FileExistsFunc { get; set; }
        public Action<string>? DeleteFileAction { get; set; }
        public Func<string, string>? ReadFileFunc { get; set; }
        public Func<string, Task<string>>? ReadFileAsyncFunc { get; set; }
        public Action<string, string>? WriteFileAction { get; set; }
        public Func<string, string, Task>? WriteFileAsyncAction { get; set; }


        public bool FileExists(string filePath)
        {
            if (FileExistsFunc != null)
            {
                return FileExistsFunc(filePath);
            }
            return FileContents.ContainsKey(filePath) || FileLinesForGenericRead.ContainsKey(filePath);
        }

        public void DeleteFile(string filePath)
        {
            if (DeleteFileAction != null)
            {
                DeleteFileAction(filePath);
                return;
            }
            FileContents.Remove(filePath);
            FileLinesForGenericRead.Remove(filePath);
        }

        public string ReadFile(string fileName)
        {
            if (ReadFileFunc != null)
            {
                return ReadFileFunc(fileName);
            }
            if (FileContents.TryGetValue(fileName, out var content))
            {
                return content;
            }
            throw new FileNotFoundException($"Mock file not found: {fileName}");
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            if (ReadFileAsyncFunc != null)
            {
                return await ReadFileAsyncFunc(filePath);
            }
            if (FileContents.TryGetValue(filePath, out var content))
            {
                return content;
            }
            throw new FileNotFoundException($"Mock async file not found: {filePath}");
        }

        public List<T> ReadFile<T>(string fileName, Func<string, T> parser)
        {
            if (FileLinesForGenericRead.TryGetValue(fileName, out var lines))
            {
                return lines.Select(parser).ToList();
            }
            if (FileContents.TryGetValue(fileName, out var content)) // Fallback to FileContents if specific lines aren't set
            {
                 // Basic split by newline, might need adjustment based on actual CSV structure if complex
                return content.Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(parser).ToList();
            }
            throw new FileNotFoundException($"Mock file for generic read not found: {fileName}");
        }

        public void WriteFile(string fileName, string content)
        {
            if (WriteFileAction != null)
            {
                WriteFileAction(fileName, content);
                return;
            }
            FileContents[fileName] = content;
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            if (WriteFileAsyncAction != null)
            {
                await WriteFileAsyncAction(filePath, content);
                return;
            }
            FileContents[filePath] = content;
            await Task.CompletedTask;
        }

        public void WriteFile<T>(string fileName, IEnumerable<T> items, Func<T, string> formatter)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.AppendLine(formatter(item));
            }
            FileContents[fileName] = sb.ToString().TrimEnd(Environment.NewLine.ToCharArray()); // Trim trailing newline
            // Also update FileLinesForGenericRead for consistency if it's read back with ReadFile<T>
            FileLinesForGenericRead[fileName] = items.Select(formatter).ToList();
        }

        public string GetFullFilePath(string fileName)
        {
            // For mocks, we can just return the fileName as is, or a predefined path
            return Path.Combine("C:\mock\path", fileName);
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            if (FileContents.TryGetValue(sourcePath, out var content))
            {
                FileContents[destinationPath] = content;
                FileContents.Remove(sourcePath);
            }
            else
            {
                throw new FileNotFoundException($"Mock source file not found for move: {sourcePath}");
            }
        }

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (FileContents.ContainsKey(destinationPath) && !overwrite)
            {
                throw new IOException($"Mock destination file exists and overwrite is false: {destinationPath}");
            }
            if (FileContents.TryGetValue(sourcePath, out var content))
            {
                FileContents[destinationPath] = content;
            }
            else
            {
                throw new FileNotFoundException($"Mock source file not found for copy: {sourcePath}");
            }
        }
    }
}
