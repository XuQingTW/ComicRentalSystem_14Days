using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Interfaces
{
    public interface IComicService
    {
        event EventHandler? ComicsChanged;
        Task ReloadAsync();
        List<Comic> GetAllComics();
        Comic? GetComicById(int id);
        void AddComic(Comic comic);
        void UpdateComic(Comic comic);
        void DeleteComic(int id);
        List<Comic> SearchComics(string? searchTerm = null);
        List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers);
    }
}
