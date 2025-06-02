using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ComicRentalSystem_14Days.Interfaces; 

namespace ComicRentalSystem_14Days.Helpers
{
    public class FileHelper : IFileHelper 
    {
        private readonly string _baseDataPath; 

        public FileHelper(string baseDataFolderName = "AppData")
        {
            _baseDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", baseDataFolderName);

            try
            {
                if (!Directory.Exists(_baseDataPath))
                {
                    Directory.CreateDirectory(_baseDataPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"建立資料目錄 '{_baseDataPath}' 時發生錯誤: {ex.Message}");
                throw new IOException($"無法建立或存取資料目錄: {_baseDataPath}", ex);
            }
        }

        public async Task WriteFileAsync<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvFunc)
        {
            string filePath = GetFullFilePath(fileName);
            try
            {
                List<string> lines = new List<string>();
                foreach (var record in records)
                {
                    lines.Add(toCsvFunc(record));
                }
                await File.WriteAllLinesAsync(filePath, lines, Encoding.UTF8);
            }
            catch (IOException ioEx) 
            {
                Console.WriteLine($"寫入檔案 '{filePath}' 時發生錯誤: {ioEx.Message}"); // Or use a proper logger if available
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"寫入 '{filePath}' 時發生未預期的錯誤: {ex.Message}"); // Or use a proper logger
                throw;
            }
        }

        public string GetFullFilePath(string fileName)
        {
            return Path.Combine(_baseDataPath, fileName);
        }

        public string ReadFile(string fileName)
        {
            string filePath = GetFullFilePath(fileName);
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            try
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"讀取檔案 '{filePath}' 時發生錯誤: {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取 '{filePath}' 時發生未預期的錯誤: {ex.Message}");
                throw;
            }
        }

        public void WriteFile(string fileName, string content)
        {
            string filePath = GetFullFilePath(fileName);
            try
            {
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"寫入檔案 '{filePath}' 時發生錯誤: {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"寫入 '{filePath}' 時發生未預期的錯誤: {ex.Message}");
                throw;
            }
        }

        public List<T> ReadFile<T>(string fileName, Func<string, T> parseFunc)
        {
            string filePath = GetFullFilePath(fileName);
            List<T> records = new List<T>();

            if (!File.Exists(filePath))
            {
                return records;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    records.Add(parseFunc(line));
                }
            }
            catch (IOException ioEx) 
            {
                Console.WriteLine($"讀取檔案 '{filePath}' 時發生錯誤: {ioEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取 '{filePath}' 時發生未預期的錯誤: {ex.Message}");
                throw;
            }
            return records;
        }

        public void WriteFile<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvFunc)
        {
            string filePath = GetFullFilePath(fileName);
            try
            {
                List<string> lines = new List<string>();
                foreach (var record in records)
                {
                    lines.Add(toCsvFunc(record));
                }
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (IOException ioEx) 
            {
                Console.WriteLine($"寫入檔案 '{filePath}' 時發生錯誤: {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"寫入 '{filePath}' 時發生未預期的錯誤: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            string fullPath = GetFullFilePath(filePath); 
            if (!File.Exists(fullPath))
            {
                return string.Empty;
            }
            try
            {
                return await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"非同步讀取檔案 '{fullPath}' 時發生錯誤: {ioEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"非同步讀取 '{fullPath}' 時發生未預期的錯誤: {ex.Message}");
                throw; 
            }
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            string fullPath = GetFullFilePath(filePath); 
            try
            {
                await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"非同步寫入檔案 '{fullPath}' 時發生錯誤: {ioEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"非同步寫入 '{fullPath}' 時發生未預期的錯誤: {ex.Message}");
                throw; 
            }
        }

        public bool FileExists(string filePath)
        {
            string fullPath = GetFullFilePath(filePath);
            return File.Exists(fullPath);
        }

        public void DeleteFile(string filePath)
        {
            string fullPath = GetFullFilePath(filePath);
            File.Delete(fullPath);
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            string fullSourcePath = GetFullFilePath(sourcePath);
            string fullDestinationPath = GetFullFilePath(destinationPath);
            File.Move(fullSourcePath, fullDestinationPath);
        }

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            string fullSourcePath = GetFullFilePath(sourcePath);
            string fullDestinationPath = GetFullFilePath(destinationPath);
            File.Copy(fullSourcePath, fullDestinationPath, overwrite);
        }
    }
}
