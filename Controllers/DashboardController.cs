using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Models;
using SpotifyRoast.Services;
using System.Security.Claims;

namespace SpotifyRoast.Controllers
{
    // [Authorize] // Kashish: Uncomment when Auth is ready
    public class DashboardController : Controller
    {
        private readonly IGeneric<Roast> _roastRepository;
        private readonly IGeneric<RoastPersonality> _personalityRepository;

        // Kashish: Injecting repositories
        public DashboardController(IGeneric<Roast> roastRepo, IGeneric<RoastPersonality> personalityRepo)
        {
            _roastRepository = roastRepo;
            _personalityRepository = personalityRepo;
        }

        public IActionResult Index()
        {
            // Get current user ID (mocked for now until Auth is live)
            // var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); 
            
            // For now, just show all roasts or empty list
            // In real impl, we would use _roastRepository.GetAll(r => r.UserId == userId, "Personality")
            
            var response = _roastRepository.GetAll(null, "Personality");
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View(response.Datas);
            }
            return View(new List<Roast>());
        }

        public IActionResult NewRoast()
        {
            // Load personalities for the dropdown
            var personalities = _personalityRepository.GetAll().Datas;
            ViewBag.Personalities = personalities;
            return View();
        }
    }
}
