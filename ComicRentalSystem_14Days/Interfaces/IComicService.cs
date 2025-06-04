
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ComicRentalSystem_14Days.Interfaces
{
    public interface IComicService
    {
        event ComicService.ComicDataChangedEventHandler? ComicsChanged;

        Task ReloadAsync();
        List<Comic> GetAllComics();
        Comic? GetComicById(int id);
        void AddComic(Comic comic);
      
        Task AddComicAsync(Comic comic);
        void UpdateComic(Comic comic);
        void DeleteComic(int id);
        List<Comic> GetComicsByGenre(string genreFilter);
      
        List<Comic> SearchComics(string? searchTerm = null);
        List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers);
    }
}
