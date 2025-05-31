using System;
using System.Collections.Generic;
using System.IO; // Added for IOException
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms; // Added for MessageBox
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Services
{
    public class ComicService
    {
        private readonly IFileHelper _fileHelper; // Changed to IFileHelper
        private readonly string _comicFileName = "comics.csv";
        private List<Comic> _comics = new List<Comic>();
        private readonly ILogger _logger;
        private readonly object _comicsLock = new object();

        public delegate void ComicDataChangedEventHandler(object? sender, EventArgs e);
        public event ComicDataChangedEventHandler? ComicsChanged;

        public async Task ReloadAsync()
        {
            _logger.Log("Async reload requested for ComicService.");
            List<Comic> loadedComics = await InternalLoadComicsAsync();
            lock (_comicsLock)
            {
                _comics = loadedComics;
            }
            OnComicsChanged();
            _logger.Log($"ComicService async reloaded. {_comics.Count} comics loaded.");
        }

        public ComicService(IFileHelper fileHelper, ILogger? logger) // Changed parameter to IFileHelper
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "ComicService 的記錄器不可為空。");

            _logger.Log("ComicService 初始化中。");

            LoadComicsFromFile(); // Synchronous load for constructor
            _logger.Log($"ComicService 初始化完成。已載入 {_comics.Count} 本漫畫。");
        }

        private void LoadComicsFromFile() // Renamed from LoadComics
        {
            _logger.Log($"正在嘗試從檔案載入漫畫 (同步): '{_comicFileName}'。");
            lock (_comicsLock)
            {
                try
                {
                    // This uses the synchronous FileHelper method
                    _comics = _fileHelper.ReadFile<Comic>(_comicFileName, Comic.FromCsvString);
                    _logger.Log($"成功從 '{_comicFileName}' (同步) 載入 {_comics.Count} 本漫畫。");
                }
                catch (Exception ex) when (ex is FormatException || ex is IOException)
                {
                    _logger.LogError($"嚴重錯誤: 漫畫資料檔案 '{_comicFileName}' (同步) 已損壞或無法讀取。詳細資訊: {ex.Message}", ex);
                    throw new ApplicationException($"無法從 '{_comicFileName}' (同步) 載入漫畫資料。應用程式可能無法正常運作。", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"從 '{_comicFileName}' (同步) 載入漫畫時發生未預期的錯誤。詳細資訊: {ex.Message}", ex);
                    throw new ApplicationException("載入漫畫資料期間 (同步) 發生未預期錯誤。", ex);
                }
            }
        }

        private async Task<List<Comic>> InternalLoadComicsAsync() // Renamed from LoadComicsAsync
        {
            _logger.Log($"正在嘗試從檔案非同步載入漫畫: '{_comicFileName}'。");
            try
            {
                string csvData = await _fileHelper.ReadFileAsync(_comicFileName);
                if (string.IsNullOrWhiteSpace(csvData))
                {
                    _logger.Log($"漫畫檔案 '{_comicFileName}' (非同步) 為空或找不到。");
                    return new List<Comic>();
                }

                var comicsList = new List<Comic>();
                var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Skip header line if present (assuming simple CSV structure)
                // Adjust Skip count if your CSV has no header or multiple header lines.
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        comicsList.Add(Comic.FromCsvString(line));
                    }
                    catch (FormatException formatEx)
                    {
                        _logger.LogError($"解析行失敗 (非同步): '{line}'. 錯誤: {formatEx.Message}", formatEx);
                        // Decide whether to skip the line or stop processing
                    }
                }
                _logger.Log($"成功從 '{_comicFileName}' (非同步) 載入並解析 {comicsList.Count} 本漫畫。");
                return comicsList;
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"漫畫檔案 '{_comicFileName}' (非同步) 找不到。返回空列表。");
                return new List<Comic>();
            }
            catch (IOException ioEx)
            {
                _logger.LogError($"讀取漫畫檔案 '{_comicFileName}' (非同步) 時發生IO錯誤: {ioEx.Message}", ioEx);
                return new List<Comic>(); // Or rethrow as critical
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 '{_comicFileName}' (非同步) 載入漫畫時發生未預期的錯誤: {ex.Message}", ex);
                return new List<Comic>(); // Or rethrow as critical
            }
        }

        private void SaveComics()
        {
            _logger.Log($"正在嘗試將 {_comics.Count} 本漫畫儲存到檔案: '{_comicFileName}'。");
            lock (_comicsLock)
            {
                try
                {
                    _fileHelper.WriteFile<Comic>(_comicFileName, _comics, comic => comic.ToCsvString());
                    _logger.Log($"已成功將 {_comics.Count} 本漫畫儲存到 '{_comicFileName}'。");
                    OnComicsChanged();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"將漫畫儲存到 '{_comicFileName}' 時發生錯誤。", ex);
                    throw;
                }
            }
        }

        protected virtual void OnComicsChanged()
        {
            ComicsChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("已觸發 ComicsChanged 事件。");
        }
        public List<Comic> GetAllComics()
        {
            _logger.Log("已呼叫 GetAllComics。");
            lock (_comicsLock)
            {
                return new List<Comic>(_comics);
            }
        }

        public Comic? GetComicById(int id)
        {
            _logger.Log($"已為ID: {id} 呼叫 GetComicById。");
            Comic? comic = _comics.FirstOrDefault(c => c.Id == id);
            if (comic == null)
            {
                _logger.Log($"找不到ID為: {id} 的漫畫。");
            }
            else
            {
                _logger.Log($"找到ID為: {id} 的漫畫: 書名='{comic.Title}'。");
            }
            return comic;
        }

        public void AddComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("嘗試新增空的漫畫物件。", ex);
                throw ex;
            }

            _logger.Log($"正在嘗試新增漫畫: 書名='{comic.Title}', 作者='{comic.Author}'。");

            lock (_comicsLock)
            {
                if (comic.Id != 0 && _comics.Any(c => c.Id == comic.Id))
                {
                    var ex = new InvalidOperationException($"ID為 {comic.Id} 的漫畫已存在。");
                    _logger.LogError($"新增漫畫失敗: ID {comic.Id} (書名='{comic.Title}') 已存在。", ex);
                    throw ex;
                }
                if (_comics.Any(c => c.Title.Equals(comic.Title, StringComparison.OrdinalIgnoreCase) &&
                                     c.Author.Equals(comic.Author, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning($"書名='{comic.Title}' 且作者='{comic.Author}' 相同的漫畫已存在。繼續新增。");
                }

                if (comic.Id == 0)
                {
                    comic.Id = GetNextIdInternal(); // Assuming GetNextId is also locked or made internal
                    _logger.Log($"已為漫畫 '{comic.Title}' 產生新的ID {comic.Id}。");
                }

                _comics.Add(comic);
                _logger.Log($"漫畫 '{comic.Title}' (ID: {comic.Id}) 已新增至記憶體列表。漫畫總數: {_comics.Count}。");
                SaveComics(); // SaveComics itself handles locking for file write and OnComicsChanged
            }
        }

        public void UpdateComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("嘗試使用空的漫畫物件進行更新。", ex);
                throw ex;
            }

            _logger.Log($"正在嘗試更新ID為: {comic.Id} (書名='{comic.Title}') 的漫畫。");

            lock (_comicsLock)
            {
                Comic? existingComic = _comics.FirstOrDefault(c => c.Id == comic.Id);
                if (existingComic == null)
                {
                    var ex = new InvalidOperationException($"找不到ID為 {comic.Id} 的漫畫進行更新。");
                    _logger.LogError($"更新漫畫失敗: 找不到ID {comic.Id} (書名='{comic.Title}')。", ex);
                    throw ex;
                }

                existingComic.Title = comic.Title;
                existingComic.Author = comic.Author;
                existingComic.Isbn = comic.Isbn;
                existingComic.Genre = comic.Genre;
                existingComic.IsRented = comic.IsRented;
                existingComic.RentedToMemberId = comic.RentedToMemberId;
                _logger.Log($"ID {comic.Id} (書名='{existingComic.Title}') 的漫畫屬性已在記憶體中更新。");

                SaveComics(); // SaveComics itself handles locking for file write and OnComicsChanged
                _logger.Log($"ID為: {comic.Id} (書名='{existingComic.Title}') 的漫畫更新已保存到檔案。");
            }
        }

        public void DeleteComic(int id)
        {
            _logger.Log($"正在嘗試刪除ID為: {id} 的漫畫。");
            lock (_comicsLock)
            {
                Comic? comicToRemove = _comics.FirstOrDefault(c => c.Id == id);

                if (comicToRemove == null)
                {
                    var ex = new InvalidOperationException($"找不到ID為 {id} 的漫畫進行刪除。");
                    _logger.LogError($"刪除漫畫失敗: 找不到ID {id}。", ex);
                    throw ex;
                }

                if (comicToRemove.IsRented)
                {
                    _logger.LogWarning($"已阻止刪除已租借的漫畫ID {id} ('{comicToRemove.Title}')。由會員ID: {comicToRemove.RentedToMemberId} 租借。");
                    throw new InvalidOperationException("無法刪除漫畫: 漫畫目前已租借。");
                }

                _comics.Remove(comicToRemove);
                _logger.Log($"漫畫 '{comicToRemove.Title}' (ID: {id}) 已從記憶體列表移除。漫畫總數: {_comics.Count}。");
                SaveComics(); // SaveComics itself handles locking for file write and OnComicsChanged
            }
        }

        // Renaming to GetNextIdInternal as it's only used internally and accesses _comics list directly
        private int GetNextIdInternal()
        {
            // This method should be called within a _comicsLock
            int nextId = !_comics.Any() ? 1 : _comics.Max(c => c.Id) + 1;
            _logger.Log($"下一個可用的漫畫ID已確定為: {nextId}。");
            return nextId;
        }

        public List<Comic> GetComicsByGenre(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("已呼叫 GetComicsByGenre，類型過濾器為空，返回所有漫畫。");
                return new List<Comic>(_comics);
            }
            else
            {
                _logger.Log($"已呼叫 GetComicsByGenre，依類型篩選: '{genreFilter}'。");
                List<Comic> filteredComics = _comics.Where(c => c.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                _logger.Log($"找到 {filteredComics.Count} 本類型為 '{genreFilter}' 的漫畫。");
                return filteredComics;
            }
        }

        public List<Comic> SearchComics(string? searchTerm = null) // Allow a single search term
        {
            _logger.Log($"已呼叫 SearchComics，搜尋詞: '{searchTerm ?? "N/A"}'。");
            var query = _comics.AsQueryable();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("搜尋詞為空，返回所有漫畫。");
                return _comics.ToList(); // Return all comics if search term is empty
            }

            query = query.Where(c =>
                (c.Title != null && c.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Author != null && c.Author.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Isbn != null && c.Isbn.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Genre != null && c.Genre.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Id.ToString().Equals(searchTerm))
            );
            _logger.Log($"已套用搜尋詞: '{searchTerm}'。");

            List<Comic> results = query.ToList();
            _logger.Log($"SearchComics 找到 {results.Count} 本相符的漫畫。");
            return results;
        }

        public List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers)
        {
            _logger.Log("Generating AdminComicStatusViewModels, using provided member list for lookups.");
            var allComics = this.GetAllComics(); // Assuming this gets all comics
            var memberLookup = allMembers.ToDictionary(m => m.Id);
            var comicStatuses = new List<AdminComicStatusViewModel>();

            foreach (var comic in allComics)
            {
                var viewModel = new AdminComicStatusViewModel
                {
                    Id = comic.Id,
                    Title = comic.Title,
                    Author = comic.Author,
                    Genre = comic.Genre,
                    Isbn = comic.Isbn,
                    RentalDate = comic.RentalDate,
                    ReturnDate = comic.ReturnDate,
                    ActualReturnTime = comic.ActualReturnTime // Ensure this property exists on AdminComicStatusViewModel
                };

                if (comic.IsRented && comic.RentedToMemberId != 0)
                {
                    viewModel.Status = "被借閱";
                    if (memberLookup.TryGetValue(comic.RentedToMemberId, out Member? borrower))
                    {
                        viewModel.BorrowerName = borrower.Name;
                        viewModel.BorrowerPhoneNumber = borrower.PhoneNumber;
                    }
                    else
                    {
                        viewModel.BorrowerName = "不明";
                        viewModel.BorrowerPhoneNumber = "不明";
                        _logger.LogWarning($"Could not find member with ID {comic.RentedToMemberId} in provided list for rented comic ID {comic.Id}");
                    }
                }
                else
                {
                    viewModel.Status = "在館中";
                }
                comicStatuses.Add(viewModel);
            }
            _logger.Log($"Generated {comicStatuses.Count} AdminComicStatusViewModels.");
            return comicStatuses;
        }
    }
}
                throw new ApplicationException($"無法從 '{_comicFileName}' (同步) 載入漫畫資料。應用程式可能無法正常運作。", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 '{_comicFileName}' (同步) 載入漫畫時發生未預期的錯誤。詳細資訊: {ex.Message}", ex);
                throw new ApplicationException("載入漫畫資料期間 (同步) 發生未預期錯誤。", ex);
            }
        }

        private async Task<List<Comic>> LoadComicsAsync()
        {
            _logger.Log($"正在嘗試從檔案非同步載入漫畫: '{_comicFileName}'。");
            try
            {
                string csvData = await _fileHelper.ReadFileAsync(_comicFileName);
                if (string.IsNullOrWhiteSpace(csvData))
                {
                    _logger.Log($"漫畫檔案 '{_comicFileName}' (非同步) 為空或找不到。");
                    return new List<Comic>();
                }

                var comicsList = new List<Comic>();
                var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Skip header line if present (assuming simple CSV structure)
                // Adjust Skip count if your CSV has no header or multiple header lines.
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        comicsList.Add(Comic.FromCsvString(line));
                    }
                    catch (FormatException formatEx)
                    {
                        _logger.LogError($"解析行失敗 (非同步): '{line}'. 錯誤: {formatEx.Message}", formatEx);
                        // Decide whether to skip the line or stop processing
                    }
                }
                _logger.Log($"成功從 '{_comicFileName}' (非同步) 載入並解析 {comicsList.Count} 本漫畫。");
                return comicsList;
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"漫畫檔案 '{_comicFileName}' (非同步) 找不到。返回空列表。");
                return new List<Comic>();
            }
            catch (IOException ioEx)
            {
                _logger.LogError($"讀取漫畫檔案 '{_comicFileName}' (非同步) 時發生IO錯誤: {ioEx.Message}", ioEx);
                return new List<Comic>(); // Or rethrow as critical
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 '{_comicFileName}' (非同步) 載入漫畫時發生未預期的錯誤: {ex.Message}", ex);
                return new List<Comic>(); // Or rethrow as critical
            }
        }

        private void SaveComics()
        {
            _logger.Log($"正在嘗試將 {_comics.Count} 本漫畫儲存到檔案: '{_comicFileName}'。");
            lock (_comicsLock)
            {
                try
                {
                    _fileHelper.WriteFile<Comic>(_comicFileName, _comics, comic => comic.ToCsvString());
                    _logger.Log($"已成功將 {_comics.Count} 本漫畫儲存到 '{_comicFileName}'。");
                    OnComicsChanged();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"將漫畫儲存到 '{_comicFileName}' 時發生錯誤。", ex);
                    throw;
                }
            }
        }

        protected virtual void OnComicsChanged()
        {
            ComicsChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("已觸發 ComicsChanged 事件。");
        }
        public List<Comic> GetAllComics()
        {
            _logger.Log("已呼叫 GetAllComics。");
            lock (_comicsLock)
            {
                return new List<Comic>(_comics);
            }
        }

        public Comic? GetComicById(int id)
        {
            _logger.Log($"已為ID: {id} 呼叫 GetComicById。");
            Comic? comic = _comics.FirstOrDefault(c => c.Id == id);
            if (comic == null)
            {
                _logger.Log($"找不到ID為: {id} 的漫畫。");
            }
            else
            {
                _logger.Log($"找到ID為: {id} 的漫畫: 書名='{comic.Title}'。");
            }
            return comic;
        }

        public void AddComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("嘗試新增空的漫畫物件。", ex);
                throw ex;
            }

            _logger.Log($"正在嘗試新增漫畫: 書名='{comic.Title}', 作者='{comic.Author}'。");

            if (comic.Id != 0 && _comics.Any(c => c.Id == comic.Id))
            {
                var ex = new InvalidOperationException($"ID為 {comic.Id} 的漫畫已存在。");
                _logger.LogError($"新增漫畫失敗: ID {comic.Id} (書名='{comic.Title}') 已存在。", ex);
                throw ex;
            }
            if (_comics.Any(c => c.Title.Equals(comic.Title, StringComparison.OrdinalIgnoreCase) &&
                                 c.Author.Equals(comic.Author, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"書名='{comic.Title}' 且作者='{comic.Author}' 相同的漫畫已存在。繼續新增。");
            }

            if (comic.Id == 0)
            {
                comic.Id = GetNextId();
                _logger.Log($"已為漫畫 '{comic.Title}' 產生新的ID {comic.Id}。");
            }

            _comics.Add(comic);
            _logger.Log($"漫畫 '{comic.Title}' (ID: {comic.Id}) 已新增至記憶體列表。漫畫總數: {_comics.Count}。");
            SaveComics();
        }

        public void UpdateComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("嘗試使用空的漫畫物件進行更新。", ex);
                throw ex;
            }

            _logger.Log($"正在嘗試更新ID為: {comic.Id} (書名='{comic.Title}') 的漫畫。");

            Comic? existingComic = _comics.FirstOrDefault(c => c.Id == comic.Id);
            if (existingComic == null)
            {
                var ex = new InvalidOperationException($"找不到ID為 {comic.Id} 的漫畫進行更新。");
                _logger.LogError($"更新漫畫失敗: 找不到ID {comic.Id} (書名='{comic.Title}')。", ex);
                throw ex;
            }

            existingComic.Title = comic.Title;
            existingComic.Author = comic.Author;
            existingComic.Isbn = comic.Isbn;
            existingComic.Genre = comic.Genre;
            existingComic.IsRented = comic.IsRented;
            existingComic.RentedToMemberId = comic.RentedToMemberId;
            _logger.Log($"ID {comic.Id} (書名='{existingComic.Title}') 的漫畫屬性已在記憶體中更新。");

            SaveComics();
            _logger.Log($"ID為: {comic.Id} (書名='{existingComic.Title}') 的漫畫更新已保存到檔案。");
        }

        public void DeleteComic(int id)
        {
            _logger.Log($"正在嘗試刪除ID為: {id} 的漫畫。");
            Comic? comicToRemove = _comics.FirstOrDefault(c => c.Id == id);

            if (comicToRemove == null)
            {
                var ex = new InvalidOperationException($"找不到ID為 {id} 的漫畫進行刪除。");
                _logger.LogError($"刪除漫畫失敗: 找不到ID {id}。", ex);
                throw ex;
            }

            if (comicToRemove.IsRented)
            {
                _logger.LogWarning($"已阻止刪除已租借的漫畫ID {id} ('{comicToRemove.Title}')。由會員ID: {comicToRemove.RentedToMemberId} 租借。");
                throw new InvalidOperationException("無法刪除漫畫: 漫畫目前已租借。");
            }

            _comics.Remove(comicToRemove);
            _logger.Log($"漫畫 '{comicToRemove.Title}' (ID: {id}) 已從記憶體列表移除。漫畫總數: {_comics.Count}。");
            SaveComics();
        }

        public int GetNextId()
        {
            int nextId = !_comics.Any() ? 1 : _comics.Max(c => c.Id) + 1;
            _logger.Log($"下一個可用的漫畫ID已確定為: {nextId}。");
            return nextId;
        }

        public List<Comic> GetComicsByGenre(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("已呼叫 GetComicsByGenre，類型過濾器為空，返回所有漫畫。");
                return new List<Comic>(_comics);
            }
            else
            {
                _logger.Log($"已呼叫 GetComicsByGenre，依類型篩選: '{genreFilter}'。");
                List<Comic> filteredComics = _comics.Where(c => c.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                _logger.Log($"找到 {filteredComics.Count} 本類型為 '{genreFilter}' 的漫畫。");
                return filteredComics;
            }
        }

        public List<Comic> SearchComics(string? searchTerm = null) // Allow a single search term
        {
            _logger.Log($"已呼叫 SearchComics，搜尋詞: '{searchTerm ?? "N/A"}'。");
            var query = _comics.AsQueryable();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("搜尋詞為空，返回所有漫畫。");
                return _comics.ToList(); // Return all comics if search term is empty
            }

            query = query.Where(c =>
                (c.Title != null && c.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Author != null && c.Author.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Isbn != null && c.Isbn.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Genre != null && c.Genre.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (c.Id.ToString().Equals(searchTerm))
            );
            _logger.Log($"已套用搜尋詞: '{searchTerm}'。");

            List<Comic> results = query.ToList();
            _logger.Log($"SearchComics 找到 {results.Count} 本相符的漫畫。");
            return results;
        }

        public List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers)
        {
            _logger.Log("Generating AdminComicStatusViewModels, using provided member list for lookups.");
            var allComics = this.GetAllComics(); // Assuming this gets all comics
            var memberLookup = allMembers.ToDictionary(m => m.Id);
            var comicStatuses = new List<AdminComicStatusViewModel>();

            foreach (var comic in allComics)
            {
                var viewModel = new AdminComicStatusViewModel
                {
                    Id = comic.Id,
                    Title = comic.Title,
                    Author = comic.Author,
                    Genre = comic.Genre,
                    Isbn = comic.Isbn,
                    RentalDate = comic.RentalDate,
                    ReturnDate = comic.ReturnDate,
                    ActualReturnTime = comic.ActualReturnTime // Ensure this property exists on AdminComicStatusViewModel
                };

                if (comic.IsRented && comic.RentedToMemberId != 0)
                {
                    viewModel.Status = "被借閱";
                    if (memberLookup.TryGetValue(comic.RentedToMemberId, out Member? borrower))
                    {
                        viewModel.BorrowerName = borrower.Name;
                        viewModel.BorrowerPhoneNumber = borrower.PhoneNumber;
                    }
                    else
                    {
                        viewModel.BorrowerName = "不明";
                        viewModel.BorrowerPhoneNumber = "不明";
                        _logger.LogWarning($"Could not find member with ID {comic.RentedToMemberId} in provided list for rented comic ID {comic.Id}");
                    }
                }
                else
                {
                    viewModel.Status = "在館中";
                }
                comicStatuses.Add(viewModel);
            }
            _logger.Log($"Generated {comicStatuses.Count} AdminComicStatusViewModels.");
            return comicStatuses;
        }
    }
}