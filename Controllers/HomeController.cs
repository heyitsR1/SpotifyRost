using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Models;
using SpotifyRoast.Services;

namespace SpotifyRoast.Controllers;

public class HomeController : Controller
{
    private readonly IGeneric<RoastPersonality> _personalityRepo;
    private readonly ISpotifyService _spotifyService;
    private readonly IRoastGenerationService _roastGenerationService;

    public HomeController(
        IGeneric<RoastPersonality> personalityRepo,
        ISpotifyService spotifyService,
        IRoastGenerationService roastGenerationService)
    {
        _personalityRepo = personalityRepo;
        _spotifyService = spotifyService;
        _roastGenerationService = roastGenerationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var response = _personalityRepo.GetAll();
        ViewBag.Personalities = response.StatusCode == System.Net.HttpStatusCode.OK ? response.Datas : new List<RoastPersonality>();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateRoast(string spotifyPlaylistUrl, int personalityId)
    {
        var personalitiesResponse = _personalityRepo.GetAll();
        ViewBag.Personalities = personalitiesResponse.StatusCode == System.Net.HttpStatusCode.OK ? personalitiesResponse.Datas : new List<RoastPersonality>();

        if (string.IsNullOrEmpty(spotifyPlaylistUrl))
        {
            ViewBag.Error = "Please provide a valid Spotify Playlist URL.";
            return View("Index");
        }

        var personalityResponse = _personalityRepo.GetById(personalityId);
        if (personalityResponse.StatusCode != System.Net.HttpStatusCode.OK || personalityResponse.Data == null)
        {
            ViewBag.Error = "Invalid Roast Personality selected.";
            return View("Index");
        }

        var personality = personalityResponse.Data;

        try
        {
            // 1. Fetch Playlist Data
            var playlistData = await _spotifyService.GetPlaylistDetailsAsync(spotifyPlaylistUrl);

            // 2. Generate Roast
            var roastResult = await _roastGenerationService.GenerateRoastAsync(personality.SystemPrompt, playlistData);

            ViewBag.Roast = roastResult;
            ViewBag.SelectedPersonalityName = personality.Name;
            
            return View("Index");
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Failed to generate roast: {ex.Message}";
            return View("Index");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
