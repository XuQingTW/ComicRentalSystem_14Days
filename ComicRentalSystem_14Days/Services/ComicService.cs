using System;
using System.Collections.Generic;
using System.Linq;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces; // 技術點3: 引用介面命名空間
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Services
{
    public class ComicService
    {
        private readonly FileHelper _fileHelper;
        private readonly string _comicFileName = "comics.csv"; // 漫畫資料檔名
        private List<Comic> _comics; // 記憶體中的漫畫快取
        private readonly ILogger _logger; // 技術點3: 介面 - 儲存 ILogger 實例

        // 技術點5: 委派與執行緒 (這裡先定義委派和事件，之後可以用到)
        public delegate void ComicDataChangedEventHandler(object? sender, EventArgs e);
        public event ComicDataChangedEventHandler? ComicsChanged; // 事件，當漫畫資料變更時觸發

        // 建構函式修改以接收 ILogger
        // 技術點4: 多型 (可以傳入任何實作 ILogger 的物件)
        public ComicService(FileHelper fileHelper, ILogger logger)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // 初始化 Logger

            _logger.Log("ComicService initializing."); // 技術點3 & 4: 透過介面呼叫 Log()
            LoadComics();
            _logger.Log($"ComicService initialized. Loaded {_comics.Count} comics.");
        }

        private void LoadComics()
        {
            _logger.Log($"Attempting to load comics from file: '{_comicFileName}'.");
            try
            {
                // 技術點8: 檔案與資料夾處理 (間接透過 FileHelper)
                _comics = _fileHelper.ReadCsvFile<Comic>(_comicFileName, Comic.FromCsvString);
                _logger.Log($"Successfully loaded {_comics.Count} comics from '{_comicFileName}'.");
            }
            catch (Exception ex) // 技術點5: 例外處理
            {
                // 技術點4: 過載 & 多型 (使用 LogError 的過載，透過 ILogger 介面呼叫)
                _logger.LogError($"Error loading comics from '{_comicFileName}'. Initializing with an empty list.", ex);
                _comics = new List<Comic>(); // 發生錯誤時，使用空列表以避免程式崩潰
            }
        }

        private void SaveComics()
        {
            _logger.Log($"Attempting to save {_comics.Count} comics to file: '{_comicFileName}'.");
            try
            {
                // 技術點8: 檔案與資料夾處理 (間接透過 FileHelper)
                _fileHelper.WriteCsvFile<Comic>(_comicFileName, _comics, comic => comic.ToCsvString());
                _logger.Log($"Successfully saved {_comics.Count} comics to '{_comicFileName}'.");
                OnComicsChanged(); // 觸發資料變更事件
            }
            catch (Exception ex) // 技術點5: 例外處理
            {
                _logger.LogError($"Error saving comics to '{_comicFileName}'.", ex);
                throw; // 重新拋出，讓呼叫者（如UI層）知道儲存失敗並可以適當處理
            }
        }

        protected virtual void OnComicsChanged() // 觸發事件的方法
        {
            ComicsChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("ComicsChanged event invoked.");
        }

        public List<Comic> GetAllComics()
        {
            _logger.Log("GetAllComics called.");
            // 返回一個複本，避免外部直接修改內部列表 (更符合封裝)
            return new List<Comic>(_comics);
        }

        public Comic? GetComicById(int id)
        {
            _logger.Log($"GetComicById called for ID: {id}.");
            Comic? comic = _comics.FirstOrDefault(c => c.Id == id);
            if (comic == null)
            {
                _logger.Log($"Comic with ID: {id} not found.");
            }
            else
            {
                _logger.Log($"Comic with ID: {id} found: Title='{comic.Title}'.");
            }
            return comic;
        }

        public void AddComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("Attempted to add a null comic object.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to add new comic: Title='{comic.Title}', Author='{comic.Author}'.");

            // 檢查ID是否已存在 (如果ID非0)
            if (comic.Id != 0 && _comics.Any(c => c.Id == comic.Id))
            {
                var ex = new InvalidOperationException($"Comic with ID {comic.Id} already exists.");
                _logger.LogError($"Failed to add comic: ID {comic.Id} (Title='{comic.Title}') already exists.", ex);
                throw ex;
            }
            // 檢查同名同作者的書 (可選)
            if (_comics.Any(c => c.Title.Equals(comic.Title, StringComparison.OrdinalIgnoreCase) &&
                                 c.Author.Equals(comic.Author, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Log($"Warning: A comic with the same title='{comic.Title}' and author='{comic.Author}' already exists.");
                // 根據需求，這裡可以選擇拋出例外或僅記錄警告
            }

            // 產生新的 ID (如果傳入的 comic.Id 是0或預設值)
            if (comic.Id == 0)
            {
                comic.Id = GetNextId(); // GetNextId 內部已有日誌
                _logger.Log($"Generated new ID {comic.Id} for comic '{comic.Title}'.");
            }

            _comics.Add(comic);
            _logger.Log($"Comic '{comic.Title}' (ID: {comic.Id}) added to in-memory list. Total comics: {_comics.Count}.");
            SaveComics(); // SaveComics 內部已有日誌記錄
        }

        public void UpdateComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("Attempted to update with a null comic object.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to update comic with ID: {comic.Id} (Title='{comic.Title}').");

            Comic? existingComic = _comics.FirstOrDefault(c => c.Id == comic.Id);
            if (existingComic == null)
            {
                var ex = new InvalidOperationException($"Comic with ID {comic.Id} not found for update.");
                _logger.LogError($"Failed to update comic: ID {comic.Id} (Title='{comic.Title}') not found.", ex);
                throw ex;
            }

            // 更新屬性
            existingComic.Title = comic.Title;
            existingComic.Author = comic.Author;
            existingComic.Isbn = comic.Isbn;
            existingComic.Genre = comic.Genre;
            existingComic.IsRented = comic.IsRented;
            existingComic.RentedToMemberId = comic.RentedToMemberId;
            _logger.Log($"Comic properties for ID {comic.Id} (Title='{existingComic.Title}') updated in-memory.");

            SaveComics(); // SaveComics 內部已有日誌記錄
            _logger.Log($"Comic with ID: {comic.Id} (Title='{existingComic.Title}') update persisted to file.");
        }

        public void DeleteComic(int id)
        {
            _logger.Log($"Attempting to delete comic with ID: {id}.");
            Comic? comicToRemove = _comics.FirstOrDefault(c => c.Id == id);

            if (comicToRemove == null)
            {
                var ex = new InvalidOperationException($"Comic with ID {id} not found for deletion.");
                _logger.LogError($"Failed to delete comic: ID {id} not found.", ex);
                throw ex;
            }

            // 檢查漫畫是否正在被租借 (可選)
            if (comicToRemove.IsRented)
            {
                _logger.Log($"Warning: Comic '{comicToRemove.Title}' (ID: {id}) is currently rented but is being deleted. Rented by Member ID: {comicToRemove.RentedToMemberId}.");
                // 根據需求，可以拋出例外或只是警告
                // throw new InvalidOperationException($"Comic '{comicToRemove.Title}' is currently rented and cannot be deleted.");
            }

            _comics.Remove(comicToRemove);
            _logger.Log($"Comic '{comicToRemove.Title}' (ID: {id}) removed from in-memory list. Total comics: {_comics.Count}.");
            SaveComics(); // SaveComics 內部已有日誌記錄
        }

        public int GetNextId()
        {
            int nextId = !_comics.Any() ? 1 : _comics.Max(c => c.Id) + 1;
            _logger.Log($"Next available comic ID determined as: {nextId}.");
            return nextId;
        }

        // 技術點4: 過載 - 示範一個服務層的過載方法
        // 根據類型篩選漫畫
        public List<Comic> GetComicsByGenre(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("GetComicsByGenre called with empty genre filter, returning all comics.");
                return new List<Comic>(_comics); // 或者可以拋出 ArgumentException，取決於業務需求
            }
            else
            {
                _logger.Log($"GetComicsByGenre called, filtering by genre: '{genreFilter}'.");
                List<Comic> filteredComics = _comics.Where(c => c.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                _logger.Log($"Found {filteredComics.Count} comics with genre '{genreFilter}'.");
                return filteredComics;
            }
        }

        // 過載版本：同時根據書名和作者篩選 (模糊查詢)
        // 技術點4: 過載
        public List<Comic> SearchComics(string? titleFilter = null, string? authorFilter = null)
        {
            _logger.Log($"SearchComics called with TitleFilter: '{titleFilter ?? "N/A"}', AuthorFilter: '{authorFilter ?? "N/A"}'.");
            var query = _comics.AsQueryable();

            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                query = query.Where(c => c.Title.IndexOf(titleFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                _logger.Log($"Applied title filter: '{titleFilter}'.");
            }
            if (!string.IsNullOrWhiteSpace(authorFilter))
            {
                query = query.Where(c => c.Author.IndexOf(authorFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                _logger.Log($"Applied author filter: '{authorFilter}'.");
            }

            if (string.IsNullOrWhiteSpace(titleFilter) && string.IsNullOrWhiteSpace(authorFilter))
            {
                _logger.Log("No search filters applied, returning all comics.");
            }

            List<Comic> results = query.ToList();
            _logger.Log($"SearchComics found {results.Count} matching comics.");
            return results;
        }
    }
}