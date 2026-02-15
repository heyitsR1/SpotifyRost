using SpotifyRoast.Models;

namespace SpotifyRoast.Services
{
    public interface IRoastService
    {
        Task<string> GenerateRoast(string spotifyLink, int personalityId);
    }
}
