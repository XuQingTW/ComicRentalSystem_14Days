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

        [Obsolete("Use asynchronous version GetAllComicsAsync instead.")]
        public List<Comic> GetAllComics()
        {
            _logger.Log("GetAllComics called (obsolete).");
            return _context.Comics.OrderBy(c => c.Title).ToList();
        }

        public async Task<List<Comic>> GetAllComicsAsync()
        {
            _logger.Log("GetAllComicsAsync called.");
            return await _context.Comics.AsNoTracking().OrderBy(c => c.Title).ToListAsync();
        }

        [Obsolete("Use asynchronous version GetComicByIdAsync instead.")]
        public Comic? GetComicById(int id)
        {
            _logger.Log($"GetComicById called for ID: {id} (obsolete).");
            var comic = _context.Comics.Find(id);
            if (comic == null)
            {
                _logger.Log($"Comic with ID: {id} not found (obsolete).");
            }
            else
            {
                _logger.Log($"Comic with ID: {id} found: Title='{comic.Title}' (obsolete).");
            }
            return comic;
        }

        public async Task<Comic?> GetComicByIdAsync(int id)
        {
            _logger.Log($"GetComicByIdAsync called for ID: {id}.");
            var comic = await _context.Comics.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (comic == null)
            {
                _logger.Log($"Comic with ID: {id} not found using GetComicByIdAsync.");
            }
            else
            {
                _logger.Log($"Comic with ID: {id} found using GetComicByIdAsync: Title='{comic.Title}'.");
            }
            return comic;
        }

        [Obsolete("Use asynchronous version AddComicAsync instead.")]
        public void AddComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("Attempted to add a null comic object (obsolete).", ex);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(comic.Title) ||
                string.IsNullOrWhiteSpace(comic.Author) ||
                string.IsNullOrWhiteSpace(comic.Isbn) ||
                string.IsNullOrWhiteSpace(comic.Genre))
            {
                var ex = new ArgumentException("Comic's essential information cannot be empty (obsolete).", nameof(comic));
                _logger.LogError("AddComic failed (obsolete): Essential information missing.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to add comic (obsolete): Title='{comic.Title}'. ID will be DB-generated if {comic.Id} is 0.");
            _context.Comics.Add(comic);
            try
            {
                if (comic.Id != 0 && _context.Comics.Any(c => c.Id == comic.Id))
                {
                    var ex = new InvalidOperationException($"ID為 {comic.Id} 的漫畫已存在。");
                    _logger.LogError($"新增漫畫失敗 (obsolete): ID {comic.Id} (書名='{comic.Title}') 已存在。", ex);
                    throw ex;
                }
                if (_context.Comics.Any(c =>
                        c.Title.ToUpperInvariant() == comic.Title.ToUpperInvariant() &&
                        c.Author.ToUpperInvariant() == comic.Author.ToUpperInvariant()))
                {
                    _logger.LogWarning($"書名='{comic.Title}' 且作者='{comic.Author}' 相同的漫畫已存在。繼續新增 (obsolete)。");
                }
                _context.SaveChanges();
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error adding comic '{comic.Title}' to database (obsolete).", ex);
                throw new InvalidOperationException($"Could not add comic (obsolete). Possible constraint violation or other database error.", ex);
            }
        }

        public async Task AddComicAsync(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("AddComicAsync: Attempted to add a null comic object.", ex);
                throw ex;
            }
            if (string.IsNullOrWhiteSpace(comic.Title) ||
                string.IsNullOrWhiteSpace(comic.Author) ||
                string.IsNullOrWhiteSpace(comic.Isbn) ||
                string.IsNullOrWhiteSpace(comic.Genre))
            {
                var ex = new ArgumentException("AddComicAsync: Comic's essential information cannot be empty.", nameof(comic));
                _logger.LogError("AddComicAsync: Essential information missing.", ex);
                throw ex;
            }

            _logger.Log($"AddComicAsync: Attempting to add comic Title='{comic.Title}'.");
            // It's good practice to check for duplicates based on Title/Author before adding,
            // though the original synchronous AddComic had a slightly different check.
            // For now, let's keep it simple and rely on database constraints or later validation if needed.
            // if (await _context.Comics.AnyAsync(c => c.Title == comic.Title && c.Author == comic.Author))
            // {
            //     _logger.LogWarning($"AddComicAsync: A comic with Title='{comic.Title}' and Author='{comic.Author}' already exists.");
            //     // Depending on requirements, you might throw or handle this differently.
            // }

            _context.Comics.Add(comic);
            try
            {
                await _context.SaveChangesAsync();
                _logger.Log($"AddComicAsync: Comic '{comic.Title}' (ID: {comic.Id}) added to database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"AddComicAsync: Error adding comic '{comic.Title}' to database.", ex);
                throw new InvalidOperationException($"AddComicAsync: Could not add comic. Possible constraint violation.", ex);
            }
        }

        [Obsolete("Use asynchronous version UpdateComicAsync instead.")]
        public void UpdateComic(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("UpdateComic (obsolete): Attempted to update with a null comic object.", ex);
                throw ex;
            }

            _logger.Log($"UpdateComic (obsolete): Attempting to update comic ID: {comic.Id} (Title='{comic.Title}').");
            var existingComic = _context.Comics.Find(comic.Id);
            if (existingComic == null)
            {
                var ex = new InvalidOperationException($"UpdateComic (obsolete): Cannot update: Comic with ID {comic.Id} not found.");
                _logger.LogError(ex.Message, ex);
                throw ex;
            }

            _context.Entry(existingComic).CurrentValues.SetValues(comic);

            try
            {
                _context.SaveChanges();
                _logger.Log($"UpdateComic (obsolete): Comic ID: {existingComic.Id} updated in database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"UpdateComic (obsolete): Error updating comic ID: {existingComic.Id} in database.", ex);
                throw new InvalidOperationException($"UpdateComic (obsolete): Could not update comic. Possible constraint violation.", ex);
            }
        }

        public async Task UpdateComicAsync(Comic comic)
        {
            if (comic == null)
            {
                var ex = new ArgumentNullException(nameof(comic));
                _logger.LogError("UpdateComicAsync: Attempted to update with a null comic object.", ex);
                throw ex;
            }

            _logger.Log($"UpdateComicAsync: Attempting to update comic ID: {comic.Id} (Title='{comic.Title}').");
            var existingComic = await _context.Comics.FindAsync(comic.Id);
            if (existingComic == null)
            {
                var ex = new InvalidOperationException($"UpdateComicAsync: Cannot update: Comic with ID {comic.Id} not found.");
                _logger.LogError(ex.Message, ex);
                throw ex;
            }

            _context.Entry(existingComic).CurrentValues.SetValues(comic);

            try
            {
                await _context.SaveChangesAsync();
                _logger.Log($"UpdateComicAsync: Comic ID: {existingComic.Id} updated in database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"UpdateComicAsync: Error updating comic ID: {existingComic.Id} in database.", ex);
                throw new InvalidOperationException($"UpdateComicAsync: Could not update comic. Possible constraint violation.", ex);
            }
        }

        [Obsolete("Use asynchronous version DeleteComicAsync instead.")]
        public void DeleteComic(int id)
        {
            _logger.Log($"DeleteComic (obsolete): Attempting to delete comic with ID: {id}.");
            var comicToRemove = _context.Comics.Find(id);
            if (comicToRemove == null)
            {
                var ex = new InvalidOperationException($"DeleteComic (obsolete): Cannot delete: Comic with ID {id} not found.");
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            if (comicToRemove.IsRented)
            {
                _logger.LogWarning($"DeleteComic (obsolete): Attempt to delete rented comic ID {id} ('{comicToRemove.Title}') was blocked.");
                throw new InvalidOperationException("DeleteComic (obsolete): Cannot delete a comic that is currently rented.");
            }

            _context.Comics.Remove(comicToRemove);
            try
            {
                _context.SaveChanges();
                _logger.Log($"DeleteComic (obsolete): Comic ID: {id} deleted from database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"DeleteComic (obsolete): Error deleting comic ID: {id} from database.", ex);
                throw new InvalidOperationException($"DeleteComic (obsolete): Could not delete comic. It might be in use or a DB error occurred.", ex);
            }
        }

        public async Task DeleteComicAsync(int id)
        {
            _logger.Log($"DeleteComicAsync: Attempting to delete comic with ID: {id}.");
            var comicToRemove = await _context.Comics.FindAsync(id);
            if (comicToRemove == null)
            {
                var ex = new InvalidOperationException($"DeleteComicAsync: Cannot delete: Comic with ID {id} not found.");
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            if (comicToRemove.IsRented)
            {
                _logger.LogWarning($"DeleteComicAsync: Attempt to delete rented comic ID {id} ('{comicToRemove.Title}') was blocked.");
                throw new InvalidOperationException("DeleteComicAsync: Cannot delete a comic that is currently rented.");
            }

            _context.Comics.Remove(comicToRemove);
            try
            {
                await _context.SaveChangesAsync();
                _logger.Log($"DeleteComicAsync: Comic ID: {id} deleted from database.");
                OnComicsChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"DeleteComicAsync: Error deleting comic ID: {id} from database.", ex);
                throw new InvalidOperationException($"DeleteComicAsync: Could not delete comic. It might be in use or a DB error occurred.", ex);
            }
        }

        [Obsolete("Use asynchronous version GetComicsByGenreAsync instead.")]
        public List<Comic> GetComicsByGenre(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("GetComicsByGenre (obsolete): Genre filter is empty, returning all comics.");
                return _context.Comics.ToList();
            }
            else
            {
                _logger.Log($"GetComicsByGenre (obsolete): Filtering by genre: '{genreFilter}'.");
                List<Comic> filteredComics = _context.Comics
                    .Where(c => c.Genre != null && c.Genre.ToUpperInvariant() == genreFilter.ToUpperInvariant())
                    .ToList();
                _logger.Log($"GetComicsByGenre (obsolete): Found {filteredComics.Count} comics of genre '{genreFilter}'.");
                return filteredComics;
            }
        }

        public async Task<List<Comic>> GetComicsByGenreAsync(string genreFilter)
        {
            if (string.IsNullOrWhiteSpace(genreFilter))
            {
                _logger.Log("GetComicsByGenreAsync: Genre filter is empty, returning all comics (AsNoTracking).");
                // Consistent with GetAllComicsAsync, using AsNoTracking and OrderBy
                return await _context.Comics.AsNoTracking().OrderBy(c => c.Title).ToListAsync();
            }
            else
            {
                _logger.Log($"GetComicsByGenreAsync: Filtering by genre: '{genreFilter}' (AsNoTracking).");
                List<Comic> filteredComics = await _context.Comics.AsNoTracking()
                    .Where(c => c.Genre != null && c.Genre.ToUpperInvariant() == genreFilter.ToUpperInvariant())
                    .OrderBy(c => c.Title) // Added OrderBy for consistency
                    .ToListAsync();
                _logger.Log($"GetComicsByGenreAsync: Found {filteredComics.Count} comics of genre '{genreFilter}'.");
                return filteredComics;
            }
        }

        [Obsolete("Use asynchronous version SearchComicsAsync instead.")]
        public List<Comic> SearchComics(string? searchTerm = null)
        {
            _logger.Log($"SearchComics (obsolete) called with searchTerm: '{searchTerm ?? "N/A"}'.");
            var query = _context.Comics.AsQueryable();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("SearchComics (obsolete): SearchTerm is empty, returning all comics ordered by Title.");
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
            _logger.Log($"SearchComics (obsolete): Search term '{searchTerm}' applied.");

            List<Comic> results = query.OrderBy(c => c.Title).ToList();
            _logger.Log($"SearchComics (obsolete): Found {results.Count} matching comics.");
            return results;
        }

        public async Task<List<Comic>> SearchComicsAsync(string? searchTerm = null)
        {
            _logger.Log($"SearchComicsAsync called with searchTerm: '{searchTerm ?? "N/A"}'.");
            var query = _context.Comics.AsQueryable(); // Starts as IQueryable

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("SearchComicsAsync: SearchTerm is empty, returning all comics ordered by Title (AsNoTracking).");
                return await query.AsNoTracking().OrderBy(c => c.Title).ToListAsync();
            }

            string searchUpper = searchTerm.ToUpperInvariant();
            // Ensure AsNoTracking is applied before ToListAsync
            query = query.Where(c =>
                (c.Title != null && c.Title.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Author != null && c.Author.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Isbn != null && c.Isbn.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Genre != null && c.Genre.ToUpperInvariant().Contains(searchUpper)) ||
                (c.Id.ToString().Equals(searchTerm)) // This Id search might be slow if not indexed well or if searchTerm is not purely numeric.
            ).AsNoTracking(); // Apply AsNoTracking here after Where predicates

            _logger.Log($"SearchComicsAsync: Search term '{searchTerm}' applied.");

            List<Comic> results = await query.OrderBy(c => c.Title).ToListAsync();
            _logger.Log($"SearchComicsAsync: Found {results.Count} matching comics.");
            return results;
        }

        [Obsolete("Use asynchronous version GetAdminComicStatusViewModelsAsync instead.")]
        public List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers)
        {
            _logger.Log("GetAdminComicStatusViewModels (obsolete): Generating AdminComicStatusViewModels using DB comics and provided member list.");
            var allComics = this.GetAllComics(); // Fetches from DB now (obsolete call)
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
                        _logger.LogWarning($"GetAdminComicStatusViewModels (obsolete): 在提供的列表中找不到ID為 {comic.RentedToMemberId} 的會員 (對應已租借的漫畫ID {comic.Id})");
                    }
                }
                else
                {
                    viewModel.Status = "在館中";
                }
                comicStatuses.Add(viewModel);
            }
            _logger.Log($"GetAdminComicStatusViewModels (obsolete): 已產生 {comicStatuses.Count} 個 AdminComicStatusViewModels。");
            return comicStatuses;
        }

        public async Task<List<AdminComicStatusViewModel>> GetAdminComicStatusViewModelsAsync(IEnumerable<Member> allMembers)
        {
            _logger.Log("GetAdminComicStatusViewModelsAsync: Generating AdminComicStatusViewModels using DB comics and provided member list.");
            var allComics = await this.GetAllComicsAsync(); // Use async version
            var memberLookup = allMembers.ToDictionary(m => m.Id); // This remains synchronous as per assumption
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
                        _logger.LogWarning($"GetAdminComicStatusViewModelsAsync: 在提供的列表中找不到ID為 {comic.RentedToMemberId} 的會員 (對應已租借的漫畫ID {comic.Id})");
                    }
                }
                else
                {
                    viewModel.Status = "在館中";
                }
                comicStatuses.Add(viewModel);
            }
            _logger.Log($"GetAdminComicStatusViewModelsAsync: 已產生 {comicStatuses.Count} 個 AdminComicStatusViewModels。");
            return comicStatuses;
        }

        public async Task<bool> HasComicsRentedByMemberAsync(int memberId)
        {
            _logger.Log($"HasComicsRentedByMemberAsync: Checking if member ID: {memberId} has active rentals.");
            bool hasRentals = await _context.Comics
                                      .AsNoTracking()
                                      .AnyAsync(c => c.IsRented && c.RentedToMemberId == memberId);
            _logger.Log($"HasComicsRentedByMemberAsync: Member ID: {memberId} has active rentals: {hasRentals}.");
            return hasRentals;
        }
    }
}