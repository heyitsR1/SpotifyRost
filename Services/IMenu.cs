using SpotifyRoast.Dtos;
using SpotifyRoast.Models;

namespace SpotifyRoast.Services
{
    public interface IMenu : IGeneric<Menu>
    {
        Task<List<Menu>> GetMenusForUser(int userId);
    }
}
