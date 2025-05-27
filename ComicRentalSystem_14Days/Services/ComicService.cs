// ComicRentalSystem_14Days/Services/ComicService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Services
{
    public class ComicService
    {
        private readonly FileHelper _fileHelper;
        private readonly string _comicFileName = "comics.csv";
        private List<Comic> _comics = new List<Comic>(); // 這裡的拼字也修正了
        private readonly ILogger _logger;

        public delegate void ComicDataChangedEventHandler(object? sender, EventArgs e);
        public event ComicDataChangedEventHandler? ComicsChanged;

        // 這個方法讓外部可以觸發重新整理
        public void Reload()
        {
            LoadComics();
            OnComicsChanged();
        }

        public ComicService(FileHelper fileHelper, ILogger? logger)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null for ComicService.");

            _logger.Log("ComicService initializing.");

            // Service 建立時，從檔案載入一次初始資料
            LoadComics();
            _logger.Log($"ComicService initialized. Loaded {_comics.Count} comics.");
        }

        private void LoadComics()
        {
            _logger.Log($"Attempting to load comics from file: '{_comicFileName}'.");
            try
            {
                _comics = _fileHelper.ReadCsvFile<Comic>(_comicFileName, Comic.FromCsvString);
                _logger.Log($"Successfully loaded {_comics.Count} comics from '{_comicFileName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading comics from '{_comicFileName}'. Initializing with an empty list.", ex);
                _comics = new List<Comic>();
            }
        }

        private void SaveComics()
        {
            _logger.Log($"Attempting to save {_comics.Count} comics to file: '{_comicFileName}'.");
            try
            {
                _fileHelper.WriteCsvFile<Comic>(_comicFileName, _comics, comic => comic.ToCsvString());
                _logger.Log($"Successfully saved {_comics.Count} comics to '{_comicFileName}'.");
                OnComicsChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving comics to '{_comicFileName}'.", ex);
                throw;
            }
        }

        protected virtual void OnComicsChanged()
        {
            ComicsChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("ComicsChanged event invoked.");
        }

        // *** 主要修改點在這裡 ***
        // GetAllComics 現在只回傳記憶體中的列表，不再重新讀取檔案
        public List<Comic> GetAllComics()
        {
            _logger.Log("GetAllComics called.");
            // LoadComics(); // <<<<<<<<<<<<< 移除這一行
            return _comics;
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

            if (comic.Id != 0 && _comics.Any(c => c.Id == comic.Id))
            {
                var ex = new InvalidOperationException($"Comic with ID {comic.Id} already exists.");
                _logger.LogError($"Failed to add comic: ID {comic.Id} (Title='{comic.Title}') already exists.", ex);
                throw ex;
            }
            if (_comics.Any(c => c.Title.Equals(comic.Title, StringComparison.OrdinalIgnoreCase) &&
                                 c.Author.Equals(comic.Author, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Log($"Warning: A comic with the same title='{comic.Title}' and author='{comic.Author}' already exists.");
            }

            if (comic.Id == 0)
            {
                comic.Id = GetNextId();
                _logger.Log($"Generated new ID {comic.Id} for comic '{comic.Title}'.");
            }

            _comics.Add(comic);
            _logger.Log($"Comic '{comic.Title}' (ID: {comic.Id}) added to in-memory list. Total comics: {_comics.Count}.");
            SaveComics();
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

            existingComic.Title = comic.Title;
            existingComic.Author = comic.Author;
            existingComic.Isbn = comic.Isbn;
            existingComic.Genre = comic.Genre;
            existingComic.IsRented = comic.IsRented;
            existingComic.RentedToMemberId = comic.RentedToMemberId;
            _logger.Log($"Comic properties for ID {comic.Id} (Title='{existingComic.Title}') updated in-memory.");

            SaveComics();
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

            if (comicToRemove.IsRented)
            {
                _logger.Log($"Warning: Comic '{comicToRemove.Title}' (ID: {id}) is currently rented but is being deleted. Rented by Member ID: {comicToRemove.RentedToMemberId}.");
            }

            _comics.Remove(comicToRemove);
            _logger.Log($"Comic '{comicToRemove.Title}' (ID: {id}) removed from in-memory list. Total comics: {_comics.Count}.");
            SaveComics();
        }

        public int GetNextId()
        {
            int nextId = !_comics.Any() ? 1 : _comics.Max(c => c.Id) + 1;
            _logger.Log($"Next available comic ID determined as: {nextId}.");
            return nextId;
        }

        public List<Comic> GetComicsByGenre(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("GetComicsByGenre called with empty genre filter, returning all comics.");
                return new List<Comic>(_comics);
            }
            else
            {
                _logger.Log($"GetComicsByGenre called, filtering by genre: '{genreFilter}'.");
                List<Comic> filteredComics = _comics.Where(c => c.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                _logger.Log($"Found {filteredComics.Count} comics with genre '{genreFilter}'.");
                return filteredComics;
            }
        }

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