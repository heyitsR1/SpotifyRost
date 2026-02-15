using SpotifyRoast.Models;

namespace SpotifyRoast.Services
{
    public class RoastService : IRoastService
    {
        private readonly IGeneric<RoastPersonality> _personalityRepo;

        public RoastService(IGeneric<RoastPersonality> personalityRepo)
        {
            _personalityRepo = personalityRepo;
        }

        public async Task<string> GenerateRoast(string spotifyLink, int personalityId)
        {
            var personality = _personalityRepo.GetById(personalityId);
            string prompt = personality.StatusCode == System.Net.HttpStatusCode.OK 
                ? personality.Data.SystemPrompt 
                : "You are a generic roast bot.";

            // Mocking AI response for now
            await Task.Delay(500); // Simulate API latency
            return $"[AI ROAST MODE: {prompt}] Wow, listing to {spotifyLink}? That's... a choice.";
        }
    }
}
