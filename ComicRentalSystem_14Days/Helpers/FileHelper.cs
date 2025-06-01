using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ComicRentalSystem_14Days.Interfaces; // 為 IFileHelper 加入

namespace ComicRentalSystem_14Days.Helpers
{
    public class FileHelper : IFileHelper // 實作 IFileHelper
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
                Console.WriteLine($"建立資料目錄 '{_baseDataPath}' 時發生錯誤: {ex.Message}");
                throw new IOException($"無法建立或存取資料目錄: {_baseDataPath}", ex);
            }
        }

        // 取得完整路徑的公用方法，可以是介面的一部分或內部使用
        public string GetFullFilePath(string fileName)
        {
            return Path.Combine(_baseDataPath, fileName);
        }

        // AuthenticationService 的實作 (非泛型)
        public string ReadFile(string fileName)
        {
            string filePath = GetFullFilePath(fileName);
            if (!File.Exists(filePath))
            {
                // 對於 AuthenticationService，LoadUsers 預期可能會擲回 FileNotFoundException
                // 或者它會處理空字串以建立新的使用者清單。
                // 若找不到則擲回 FileNotFoundException 是一種方法，或傳回空字串。
                // 目前的 AuthenticationService 會特別攔截 FileNotFoundException。
                // 然而，如果檔案僅為空，服務也預期會是空清單。
                // 讓我們與 FileHelper<T> 的行為保持一致：若找不到則傳回空值，讓服務決定。
                // 對於 users.json，如果找不到，AuthenticationService 會傳回新的 List<User>()。
                // 如果 ReadFile 直接擲回 FileNotFoundException，則服務對它的 catch 區塊沒有問題。
                // 讓我們使其擲回例外狀況，以與服務預期特定例外狀況的方式保持一致。
                // 更正：AuthN 服務預期 FileNotFound 時為空清單，而不是 ReadFile 本身擲回的例外狀況。
                // 如果由 File.ReadAllText 直接擲回，它會攔截。
                // 因此，如果找不到檔案，則傳回空字串。
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

        // ComicService、MemberService 的實作 (泛型)
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
            string fullPath = GetFullFilePath(filePath); // 使用 GetFullFilePath 以確保一致性
            // 考慮檔案不存在時的行為，類似於同步的 ReadFile
            if (!File.Exists(fullPath))
            {
                return string.Empty; // 與非泛型 ReadFile 的同步版本一致
            }
            try
            {
                return await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"非同步讀取檔案 '{fullPath}' 時發生錯誤: {ioEx.Message}");
                throw; // 或更優雅地處理，例如記錄並傳回空值
            }
            catch (Exception ex)
            {
                Console.WriteLine($"非同步讀取 '{fullPath}' 時發生未預期的錯誤: {ex.Message}");
                throw; // 或處理
            }
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            string fullPath = GetFullFilePath(filePath); // 使用 GetFullFilePath 以確保一致性
            try
            {
                await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"非同步寫入檔案 '{fullPath}' 時發生錯誤: {ioEx.Message}");
                throw; // 或處理
            }
            catch (Exception ex)
            {
                Console.WriteLine($"非同步寫入 '{fullPath}' 時發生未預期的錯誤: {ex.Message}");
                throw; // 或處理
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
            // 如果要避免不存在檔案的例外狀況，請考慮使用 File.Exists 檢查
            // 然而，如果檔案不存在，File.Delete 不會擲回例外狀況。
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
