using SpotifyAPI.Web;

namespace SpotifyRoast.Services
{
    public interface ISpotifyService
    {
        Task<string> GetPlaylistDetailsAsync(string playlistUrl);
    }

    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyClient _spotifyClient;

        public SpotifyService(IConfiguration config)
        {
            var clientId = config["CLIENT_ID"] ?? config["SPOTIFY_CLIENT_ID"];
            var clientSecret = config["CLIENT_SECRET"] ?? config["SPOTIFY_CLIENT_SECRET"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("Spotify Credentials not found in environment variables.");
            }

            var configSpotify = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(clientId, clientSecret);
            var response = new OAuthClient(configSpotify).RequestToken(request).GetAwaiter().GetResult();

            _spotifyClient = new SpotifyClient(configSpotify.WithToken(response.AccessToken));
        }

        public async Task<string> GetPlaylistDetailsAsync(string playlistUrl)
        {
            try
            {
                string playlistId = ExtractPlaylistId(playlistUrl);
                var playlist = await _spotifyClient.Playlists.Get(playlistId);
                
                var tracksInfo = new List<string>();
                
                if (playlist.Tracks?.Items != null)
                {
                    // Get up to 50 tracks to keep the LLM context size reasonable
                    foreach (var item in playlist.Tracks.Items.Take(50))
                    {
                        if (item.Track is FullTrack track)
                        {
                            var artists = string.Join(", ", track.Artists.Select(a => a.Name));
                            tracksInfo.Add($"- {track.Name} by {artists}");
                        }
                    }
                }

                return $"Playlist: {playlist.Name}\nTracks:\n{string.Join("\n", tracksInfo)}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch Spotify playlist: {ex.Message}");
            }
        }

        private string ExtractPlaylistId(string url)
        {
            try
            {
                // Example: https://open.spotify.com/playlist/37i9dQZF1DXcBWIGoYBM5M?si=123
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                // segments: ["", "playlist", "37i9..."]
                return segments.Last();
            }
            catch
            {
                throw new ArgumentException("Invalid Spotify Playlist URL format.");
            }
        }
    }
}
