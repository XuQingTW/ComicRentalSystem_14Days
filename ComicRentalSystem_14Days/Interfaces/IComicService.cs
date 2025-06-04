
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Interfaces
{
    public delegate void ComicDataChangedEventHandler(object? sender, EventArgs e);

    public interface IComicService
    {
        event ComicDataChangedEventHandler? ComicsChanged;

        Task ReloadAsync();

        List<Comic> GetAllComics();
        Comic? GetComicById(int id);
        void AddComic(Comic comic);
        void UpdateComic(Comic comic);
        void DeleteComic(int id);
        List<Comic> GetComicsByGenre(string genreFilter);
        List<Comic> SearchComics(string? searchTerm = null);
        List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers);
    }
}
