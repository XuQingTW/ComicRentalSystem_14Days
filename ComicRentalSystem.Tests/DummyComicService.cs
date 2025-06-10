using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicRentalSystem.Tests
{
    public class DummyComicService : IComicService
    {
        public event ComicDataChangedEventHandler? ComicsChanged;
        public Task ReloadAsync() => Task.CompletedTask;
        public List<Comic> GetAllComics() => new();
        public Comic? GetComicById(int id) => null;
        public void AddComic(Comic comic) { }
        public Task AddComicAsync(Comic comic) { return Task.CompletedTask; }
        public void UpdateComic(Comic comic) { }
        public void DeleteComic(int id) { }
        public List<Comic> GetComicsByGenre(string genreFilter) => new();
        public List<Comic> SearchComics(string? searchTerm = null) => new();
        public List<AdminComicStatusViewModel> GetAdminComicStatusViewModels(IEnumerable<Member> allMembers) => new();
    }
}
