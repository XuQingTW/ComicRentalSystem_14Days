using System.Collections.Generic;
using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Services
{
    public interface IComicService
    {
        List<Comic> SearchComics(string? searchTerm = null);
    }
}
