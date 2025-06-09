
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Interfaces
{
    public interface IComicService
    {
        event ComicDataChangedEventHandler? ComicsChanged;

        Task ReloadAsync();

        // Synchronous methods - kept for now, but ideally would be removed in a future step
        // if all consuming code is updated. For this task, we mark implementations obsolete.
        List<Comic> GetAllComics();
        Comic? GetComicById(int id);
        void AddComic(Comic comic);
        void UpdateComic(Comic comic);
        void DeleteComic(int id);
        List<Comic> GetComicsByGenre(string genreFilter);
        List<Comic> SearchComics(string? searchTerm = null);
        List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers);

        // Asynchronous methods
        Task<List<Comic>> GetAllComicsAsync();
        Task<Comic?> GetComicByIdAsync(int id);
        Task AddComicAsync(Comic comic); // Already matched, just ensuring it's grouped with async
        Task UpdateComicAsync(Comic comic);
        Task DeleteComicAsync(int id);
        Task<List<Comic>> GetComicsByGenreAsync(string genreFilter);
        Task<List<Comic>> SearchComicsAsync(string? searchTerm = null);
        Task<List<AdminComicStatusViewModel>> GetAdminComicStatusViewModelsAsync(IEnumerable<Member> allMembers);
        Task<bool> HasComicsRentedByMemberAsync(int memberId);
    }
}
