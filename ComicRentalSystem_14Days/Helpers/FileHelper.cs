using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ComicRentalSystem_14Days.Interfaces; // Added for IFileHelper

namespace ComicRentalSystem_14Days.Helpers
{
    public class FileHelper : IFileHelper // Implement IFileHelper
    {
        private readonly string _baseDataPath; // 資料檔案存放的基礎路徑

        public FileHelper(string baseDataFolderName = "AppData")
        {
            // 將資料檔案存放在 "我的文件" 資料夾下的指定子資料夾中
            // Environment.SpecialFolder.MyDocuments 取得 "我的文件" 路徑
            _baseDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", baseDataFolderName);

            // 確保資料夾存在
            try
            {
                if (!Directory.Exists(_baseDataPath))
                {
                    Directory.CreateDirectory(_baseDataPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating data directory '{_baseDataPath}': {ex.Message}");
                throw new IOException($"Failed to create or access data directory: {_baseDataPath}", ex);
            }
        }

        // Utility method to get the full path, can be part of the interface or used internally
        public string GetFullFilePath(string fileName)
        {
            return Path.Combine(_baseDataPath, fileName);
        }

        // Implementation for AuthenticationService (non-generic)
        public string ReadFile(string fileName)
        {
            string filePath = GetFullFilePath(fileName);
            if (!File.Exists(filePath))
            {
                // For AuthenticationService, LoadUsers expects FileNotFoundException to be potentially thrown
                // or it handles empty string for a new user list.
                // Throwing FileNotFoundException if not found is one way, or returning empty string.
                // The current AuthenticationService catches FileNotFoundException specifically.
                // However, services also expect an empty list if file is just empty.
                // Let's align with FileHelper<T> behavior: return empty if not found, let service decide.
                // For users.json, if it's not found, AuthenticationService returns new List<User>().
                // If ReadFile directly throws FileNotFoundException, then the service's catch block for it is fine.
                // Let's make it throw for consistency with how services might expect specific exceptions.
                // Correction: AuthN service expects empty list for FileNotFound, not the exception itself from ReadFile.
                // It catches it if thrown by File.ReadAllText directly.
                // So if file not found, return empty string.
                return string.Empty;
            }
            try
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading file '{filePath}': {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading '{filePath}': {ex.Message}");
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
                Console.WriteLine($"Error writing to file '{filePath}': {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while writing to '{filePath}': {ex.Message}");
                throw;
            }
        }

        // Implementation for ComicService, MemberService (generic)
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
                // 技術點 #1: 字串與陣列 (雖然ReadAllLines已處理，但parseFunc中會用)
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                // 跳過標頭行 (如果有的話，這裡假設第一行是標頭)
                // records = lines.Skip(1).Select(line => parseFunc(line)).ToList();
                // 假設沒有標頭行
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue; // 跳過空行
                    records.Add(parseFunc(line));
                }
            }
            catch (IOException ioEx) // 技術點 #5: 例外處理
            {
                Console.WriteLine($"Error reading file '{filePath}': {ioEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading '{filePath}': {ex.Message}");
                throw;
            }
            return records;
        }

        // T 是要儲存的物件類型
        // toCsvFunc 是將 T 物件轉換為 CSV 字串行的函式
        public void WriteFile<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvFunc)
        {
            string filePath = GetFullFilePath(fileName);
            try
            {
                // 技術點 #1: 字串與陣列 (雖然WriteLine已處理，但toCsvFunc會產生字串)
                List<string> lines = new List<string>();
                foreach (var record in records)
                {
                    lines.Add(toCsvFunc(record));
                }
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (IOException ioEx) // 技術點 #5: 例外處理
            {
                Console.WriteLine($"Error writing to file '{filePath}': {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while writing to '{filePath}': {ex.Message}");
                throw;
            }
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            string fullPath = GetFullFilePath(filePath); // Use GetFullFilePath to ensure consistency
            // Consider behavior if file doesn't exist, similar to synchronous ReadFile
            if (!File.Exists(fullPath))
            {
                return string.Empty; // Consistent with synchronous version for non-generic ReadFile
            }
            try
            {
                return await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading file asynchronously '{fullPath}': {ioEx.Message}");
                throw; // Or handle more gracefully, e.g., log and return empty
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading asynchronously '{fullPath}': {ex.Message}");
                throw; // Or handle
            }
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            string fullPath = GetFullFilePath(filePath); // Use GetFullFilePath
            try
            {
                await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error writing to file asynchronously '{fullPath}': {ioEx.Message}");
                throw; // Or handle
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while writing asynchronously '{fullPath}': {ex.Message}");
                throw; // Or handle
            }
        }
    }
}
