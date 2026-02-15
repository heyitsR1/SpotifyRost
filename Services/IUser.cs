using SpotifyRoast.Dtos;
using SpotifyRoast.Models;

namespace SpotifyRoast.Services
{
    public interface IUser : IGeneric<User>
    {
        ResponseDto<User> Login(string username, string password);
        ResponseDto<User> Register(User user, string roleName);
        Task<bool> UserExists(string username);
    }
}
