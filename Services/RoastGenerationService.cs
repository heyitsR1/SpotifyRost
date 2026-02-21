using System.Text.Json;
using System.Text;

namespace SpotifyRoast.Services
{
    public interface IRoastGenerationService
    {
        Task<string> GenerateRoastAsync(string systemPrompt, string playlistData);
    }

    public class RoastGenerationService : IRoastGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public RoastGenerationService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["CLAUDE_API_KEY"] ?? throw new Exception("Claude API Key not found.");
            
            _httpClient.BaseAddress = new Uri("https://api.anthropic.com/");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SpotifyRoastApp/1.0");
        }

        public async Task<string> GenerateRoastAsync(string systemPrompt, string playlistData)
        {
            var requestBody = new
            {
                model = "claude-3-haiku-20240307",
                max_tokens = 1024,
                system = systemPrompt + "\n\nGive a brutal, funny roast of the user's music taste based on the following Spotify playlist. Be concise, witty, and directly address the user.",
                messages = new[]
                {
                    new { role = "user", content = $"Here is my Spotify Playlist data:\n{playlistData}\nRoast my music taste!" }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("v1/messages", jsonContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Claude API Error: {response.StatusCode} - {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseJson);
            
            // Anthropic API response format: { "content": [ { "text": "The actual roast..." } ] }
            var root = document.RootElement;
            var contentArray = root.GetProperty("content");
            if (contentArray.GetArrayLength() > 0)
            {
                return contentArray[0].GetProperty("text").GetString() ?? "I'm speechless at how bad your taste is. (Claude returned a null string)";
            }

            return "Error parsing Claude response.";
        }
    }
}
