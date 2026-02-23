using SpotifyAPI.Web;

namespace SpotifyRoast.Services
{
    public interface ISpotifyService
    {
        Task<string> GetPlaylistDetailsAsync(string playlistUrl);
    }

    public class SpotifyService : ISpotifyService
    {

        public SpotifyService(IConfiguration config)
        {
            // API credentials no longer strictly needed for fetching public playlists 
            // since we are falling back to the embed scraper to bypass the 403 API blocking
        }

        public async Task<string> GetPlaylistDetailsAsync(string playlistUrl)
        {
            try
            {
                string playlistId = ExtractPlaylistId(playlistUrl);
                
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                
                var html = await client.GetStringAsync($"https://open.spotify.com/embed/playlist/{playlistId}");
                var match = System.Text.RegularExpressions.Regex.Match(html, @"<script id=""__NEXT_DATA__"" type=""application/json"">(.*?)</script>");
                
                if (!match.Success)
                {
                    throw new Exception("Failed to parse Spotify playlist data from the public embed.");
                }

                var jsonStr = match.Groups[1].Value;
                using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);
                
                var entity = doc.RootElement.GetProperty("props").GetProperty("pageProps").GetProperty("state").GetProperty("data").GetProperty("entity");
                var trackList = entity.GetProperty("trackList");
                var playlistName = entity.GetProperty("name").GetString();
                
                var tracksInfo = new List<string>();
                
                foreach (var track in trackList.EnumerateArray().Take(50))
                {
                    var title = track.GetProperty("title").GetString();
                    var subtitle = track.GetProperty("subtitle").GetString();
                    tracksInfo.Add($"- {title} by {subtitle}");
                }

                return $"Playlist: {playlistName}\nTracks:\n{string.Join("\n", tracksInfo)}";
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
