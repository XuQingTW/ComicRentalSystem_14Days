using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using Microsoft.EntityFrameworkCore; // For DbUpdateException

namespace ComicRentalSystem_14Days.Services
{
    public delegate void ComicDataChangedEventHandler(object? sender, EventArgs e);

    public class ComicService : IComicService
    {
        private readonly ComicRentalDbContext _context;
        private readonly ILogger _logger;

        public event ComicDataChangedEventHandler? ComicsChanged;

        public async Task ReloadAsync()
        {
            _logger.Log("ComicService ReloadAsync requested. Data is now live from DB.");
            // Potentially, this could re-query or refresh some view if needed,
            // but with direct DB access, explicit reload is less critical for the service itself.
            OnComicsChanged(); // Notify listeners that data *might* have changed or a refresh is requested
            await Task.CompletedTask; // Placeholder if no other async work
        }

        public ComicService(ComicRentalDbContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "ComicService logger cannot be null.");
            _logger.Log("ComicService initialized with ComicRentalDbContext.");
        }

        protected virtual void OnComicsChanged()
        {
            ComicsChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("ComicsChanged event triggered.");
        }

        public List<Comic> GetAllComics()
        {
            _logger.Log("GetAllComics called.");
            return _context.Comics.OrderBy(c => c.Title).ToList();
        }

        public Comic? GetComicById(int id)
        {
            _logger.Log($"GetComicById called for ID: {id}.");
            var comic = _context.Comics.Find(id);
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

            if (string.IsNullOrWhiteSpace(comic.Title) ||
                string.IsNullOrWhiteSpace(comic.Author) ||
                string.IsNullOrWhiteSpace(comic.Isbn) ||
                string.IsNullOrWhiteSpace(comic.Genre))
            {
                var ex = new ArgumentException("Comic's essential information cannot be empty.", nameof(comic));
                _logger.LogError("AddComic failed: Essential information missing.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to add comic: Title='{comic.Title}'. ID will be DB-generated if {comic.Id} is 0.");
            _context.Comics.Add(comic);
            try
            {
                if (comic.Id != 0 && _context.Comics.Any(c => c.Id == comic.Id))
                {
                    var ex = new InvalidOperationException($"ID為 {comic.Id} 的漫畫已存在。");
                    _logger.LogError($"新增漫畫失敗: ID {comic.Id} (書名='{comic.Title}') 已存在。", ex);
                    throw ex;
                }
                if (_context.Comics.Any(c =>
                        c.Title.ToUpperInvariant() == comic.Title.ToUpperInvariant() &&
                        c.Author.ToUpperInvariant() == comic.Author.ToUpperInvariant()))
                {
                    _logger.LogWarning($"書名='{comic.Title}' 且作者='{comic.Author}' 相同的漫畫已存在。繼續新增。");
                }

                _context.SaveChanges();
                _logger.Log($"Comic '{comic.Title}' (ID: {comic.Id}) added to database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error adding comic '{comic.Title}' to database.", ex);
                throw new InvalidOperationException($"Could not add comic. Possible constraint violation.", ex);
            }
        }

        public async Task AddComicAsync(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("Attempted to add a null comic object asynchronously.", ex);
                throw ex;
            }
            if (string.IsNullOrWhiteSpace(comic.Title) ||
                string.IsNullOrWhiteSpace(comic.Author) ||
                string.IsNullOrWhiteSpace(comic.Isbn) ||
                string.IsNullOrWhiteSpace(comic.Genre))
            {
                var ex = new ArgumentException("Comic's essential information cannot be empty for async add.", nameof(comic));
                _logger.LogError("AddComicAsync failed: Essential information missing.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to add comic asynchronously: Title='{comic.Title}'.");
            _context.Comics.Add(comic); // comic.Id will be handled by DB if 0
            try
            {
                await _context.SaveChangesAsync();
                _logger.Log($"Comic '{comic.Title}' (ID: {comic.Id}) added to database asynchronously.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error adding comic '{comic.Title}' asynchronously to database.", ex);
                throw new InvalidOperationException($"Could not add comic asynchronously. Possible constraint violation.", ex);
            }
        }

        public void UpdateComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("Attempted to update with a null comic object.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to update comic ID: {comic.Id} (Title='{comic.Title}').");
            var existingComic = _context.Comics.Find(comic.Id);
            if (existingComic == null)
            {
                var ex = new InvalidOperationException($"Cannot update: Comic with ID {comic.Id} not found.");
                _logger.LogError(ex.Message, ex);
                throw ex;
            }

            _context.Entry(existingComic).CurrentValues.SetValues(comic);

            try
            {
                _context.SaveChanges();
                _logger.Log($"Comic ID: {existingComic.Id} updated in database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error updating comic ID: {existingComic.Id} in database.", ex);
                throw new InvalidOperationException($"Could not update comic. Possible constraint violation.", ex);
            }
        }

        public void DeleteComic(int id)
        {
            _logger.Log($"Attempting to delete comic with ID: {id}.");
            var comicToRemove = _context.Comics.Find(id);
            if (comicToRemove == null)
            {
                var ex = new InvalidOperationException($"Cannot delete: Comic with ID {id} not found.");
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            if (comicToRemove.IsRented)
            {
                _logger.LogWarning($"Attempt to delete rented comic ID {id} ('{comicToRemove.Title}') was blocked.");
                throw new InvalidOperationException("Cannot delete a comic that is currently rented.");
            }

            _context.Comics.Remove(comicToRemove);
            try
            {
                _context.SaveChanges();
                _logger.Log($"Comic ID: {id} deleted from database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error deleting comic ID: {id} from database.", ex);
                throw new InvalidOperationException($"Could not delete comic. It might be in use or a DB error occurred.", ex);
            }
        }

        public List<Comic> GetComicsByGenre(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("GetComicsByGenre called with empty or whitespace genreFilter, returning all comics ordered by Title.");
                return _context.Comics.OrderBy(c => c.Title).ToList();
            }
            else
            {
                _logger.Log($"GetComicsByGenre called for genre: '{genreFilter}'.");
                var filteredComics = _context.Comics
                                   .Where(c => c.Genre != null && c.Genre.Equals(genreFilter, StringComparison.OrdinalIgnoreCase))
                                   .OrderBy(c => c.Title)
                                   .ToList();
                _logger.Log($"Found {filteredComics.Count} comics for genre '{genreFilter}'.");
                return filteredComics;
            }
        }

        public List<Comic> SearchComics(string? searchTerm = null)
        {
            _logger.Log($"SearchComics called with searchTerm: '{searchTerm ?? "N/A"}'.");
            var query = _context.Comics.AsQueryable();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("SearchTerm is empty, returning all comics ordered by Title.");
                return query.OrderBy(c => c.Title).ToList();
            }

            string searchUpper = searchTerm.ToUpperInvariant();
            query = query.Where(c =>
                (c.Title != null && c.Title.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Author != null && c.Author.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Isbn != null && c.Isbn.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Genre != null && c.Genre.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Id.ToString().Equals(searchTerm))
            );
            _logger.Log($"Search term '{searchTerm}' applied.");

            List<Comic> results = query.OrderBy(c => c.Title).ToList();
            _logger.Log($"SearchComics found {results.Count} matching comics.");
            return results;
        }

        public List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers)
        {
            _logger.Log("Generating AdminComicStatusViewModels using DB comics and provided member list.");
            var allComics = this.GetAllComics(); // Fetches from DB now
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