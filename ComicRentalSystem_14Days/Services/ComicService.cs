using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Threading.Tasks; 
using System.Windows.Forms; 
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Services
{
    public class ComicService
    {
        private readonly IFileHelper _fileHelper;
        private readonly string _comicFileName = "comics.csv";
        private List<Comic> _comics = new List<Comic>();
        private readonly ILogger _logger;
        private readonly object _comicsLock = new object();

        public delegate void ComicDataChangedEventHandler(object? sender, EventArgs e);
        public event ComicDataChangedEventHandler? ComicsChanged;

        public async Task ReloadAsync()
        {
            _logger.Log("ComicService 要求非同步重新載入。");
            List<Comic> loadedComics = await InternalLoadComicsAsync();
            lock (_comicsLock)
            {
                _comics = loadedComics;
            }
            OnComicsChanged();
            _logger.Log($"ComicService 已非同步重新載入。已載入 {_comics.Count} 本漫畫。");
        }

        public ComicService(IFileHelper fileHelper, ILogger? logger) 
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "ComicService 的記錄器不可為空。");

            _logger.Log("ComicService 初始化中。");

            LoadComicsFromFile(); 
            _logger.Log($"ComicService 初始化完成。已載入 {_comics.Count} 本漫畫。");
        }

        private void LoadComicsFromFile()
        {
            _logger.Log($"正在嘗試從檔案載入漫畫 (同步): '{_comicFileName}'。 (Existing log for context)");
            lock (_comicsLock)
            {
                _logger.Log($"LoadComicsFromFile [Before Read]: Attempting to read from file '{_comicFileName}'. Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'.");
                try
                {
                    _comics = _fileHelper.ReadFile<Comic>(_comicFileName, Comic.FromCsvString);
                    _logger.Log($"LoadComicsFromFile [After Read]: Successfully read and parsed. Loaded {_comics.Count} comics from '{_comicFileName}'.");
                    _logger.Log($"成功從 '{_comicFileName}' (同步) 載入 {_comics.Count} 本漫畫。 (Existing log for context)");
                }
                catch (Exception ex) when (ex is FormatException || ex is IOException)
                {
                    _logger.LogError($"嚴重錯誤: 漫畫資料檔案 '{_comicFileName}' (同步) 已損壞或無法讀取。Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. 詳細資訊: {ex.Message}", ex);
                    throw new ApplicationException($"無法從 '{_comicFileName}' (同步) 載入漫畫資料。應用程式可能無法正常運作。", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"從 '{_comicFileName}' (同步) 載入漫畫時發生未預期的錯誤。Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. 詳細資訊: {ex.Message}", ex);
                    throw new ApplicationException("載入漫畫資料期間 (同步) 發生未預期錯誤。", ex);
                }
            }
        }

        private async Task<List<Comic>> InternalLoadComicsAsync()
        {
            _logger.Log($"正在嘗試從檔案非同步載入漫畫: '{_comicFileName}'。 (Existing log for context)");
            try
            {
                _logger.Log($"InternalLoadComicsAsync [Before Read]: Attempting to read from file '{_comicFileName}'. Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'.");
                string csvData = await _fileHelper.ReadFileAsync(_comicFileName);
                
               
                _logger.Log($"InternalLoadComicsAsync [Raw Data]: Read from '{_comicFileName}'. Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. Data: '{csvData.Replace("\r", "\\r").Replace("\n", "\\n")}'.");

                if (string.IsNullOrWhiteSpace(csvData))
                {
                    _logger.Log($"漫畫檔案 '{_comicFileName}' (非同步) 為空或找不到。Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. (Enhanced log)");
                    return new List<Comic>();
                }

                var comicsList = new List<Comic>();
                var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        comicsList.Add(Comic.FromCsvString(line));
                    }
                    catch (FormatException formatEx)
                    {
                 
                        _logger.LogError($"解析行失敗 (非同步) for file '{_comicFileName}' (Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'): '{line}'. 錯誤: {formatEx.Message}", formatEx);
                    }
                }
                _logger.Log($"成功從 '{_comicFileName}' (非同步) 載入並解析 {comicsList.Count} 本漫畫。 (Existing log for context)");
                return comicsList;
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"漫畫檔案 '{_comicFileName}' (非同步) 找不到。Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. 返回空列表。 (Enhanced log)");
                return new List<Comic>();
            }
            catch (IOException ioEx)
            {
                _logger.LogError($"讀取漫畫檔案 '{_comicFileName}' (非同步) 時發生IO錯誤: {ioEx.Message}. Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. (Enhanced log)", ioEx);
                return new List<Comic>(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 '{_comicFileName}' (非同步) 載入漫畫時發生未預期的錯誤: {ex.Message}. Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'. (Enhanced log)", ex);
                return new List<Comic>();
            }
        }

        private void SaveComics()
        {
            _logger.Log($"正在嘗試將 {_comics.Count} 本漫畫儲存到檔案: '{_comicFileName}'。 (Existing log for context)"); 
            lock (_comicsLock)
            {
                _logger.Log($"SaveComics [Before Write]: Preparing to write {_comics.Count} comics to file '{_comicFileName}'. Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'.");
                
                try
                {
                    _fileHelper.WriteFile<Comic>(_comicFileName, _comics, comic => comic.ToCsvString());
                    
                    _logger.Log($"SaveComics [After Write]: Successfully called _fileHelper.WriteFile for '{_comicFileName}' with {_comics.Count} comics.");
                    
                    OnComicsChanged(); 

                    _logger.Log($"已成功將 {_comics.Count} 本漫畫儲存到 '{_comicFileName}'。 (Existing log for context)");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"將漫畫儲存到 '{_comicFileName}' 時發生錯誤。 Full path: '{_fileHelper.GetFullFilePath(_comicFileName)}'.", ex);
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
                if (_comics.Any(c =>
                        c.Title.ToUpperInvariant() == comic.Title.ToUpperInvariant() &&
                        c.Author.ToUpperInvariant() == comic.Author.ToUpperInvariant()))
                {
                    _logger.LogWarning($"書名='{comic.Title}' 且作者='{comic.Author}' 相同的漫畫已存在。繼續新增。");
                }

                if (comic.Id == 0)
                {
                    comic.Id = GetNextIdInternal();
                    _logger.Log($"已為漫畫 '{comic.Title}' 產生新的ID {comic.Id}。");
                }

                _comics.Add(comic);
                _logger.Log($"漫畫 '{comic.Title}' (ID: {comic.Id}) 已新增至記憶體列表。漫畫總數: {_comics.Count}。");
                SaveComics(); 
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

                SaveComics(); 
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
                SaveComics(); 
            }
        }

        private int GetNextIdInternal()
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
                List<Comic> filteredComics = _comics
                    .Where(c => c.Genre != null && c.Genre.ToUpperInvariant() == genreFilter.ToUpperInvariant())
                    .ToList();
                _logger.Log($"找到 {filteredComics.Count} 本類型為 '{genreFilter}' 的漫畫。");
                return filteredComics;
            }
        }

        public List<Comic> SearchComics(string? searchTerm = null) 
        {
            _logger.Log($"已呼叫 SearchComics，搜尋詞: '{searchTerm ?? "N/A"}'。");
            var query = _comics.AsQueryable();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("搜尋詞為空，返回所有漫畫。");
                return _comics.ToList();
            }

            string searchUpper = searchTerm.ToUpperInvariant();
            query = query.Where(c =>
                (c.Title != null && c.Title.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Author != null && c.Author.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Isbn != null && c.Isbn.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Genre != null && c.Genre.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Id.ToString().Equals(searchTerm))
            );
            _logger.Log($"已套用搜尋詞: '{searchTerm}'。");

            List<Comic> results = query.ToList();
            _logger.Log($"SearchComics 找到 {results.Count} 本相符的漫畫。");
            return results;
        }

        public List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers)
        {
            _logger.Log("正在產生 AdminComicStatusViewModels，使用提供的會員列表進行查詢。");
            var allComics = this.GetAllComics(); 
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
                    ActualReturnTime = comic.ActualReturnTime 
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
                        _logger.LogWarning($"在提供的列表中找不到ID為 {comic.RentedToMemberId} 的會員 (對應已租借的漫畫ID {comic.Id})");
                    }
                }
                else
                {
                    viewModel.Status = "在館中";
                }
                comicStatuses.Add(viewModel);
            }
            _logger.Log($"已產生 {comicStatuses.Count} 個 AdminComicStatusViewModels。");
            return comicStatuses;
        }
    }
}   