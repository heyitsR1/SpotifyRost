using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Models;
using SpotifyRoast.Services;
using System.Security.Claims;

namespace SpotifyRoast.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize] // Require Auth for Dashboard
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
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }
            
            var response = _roastRepository.GetAll(r => r.UserId == userId, "Personality");
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View(response.Datas);
            }
            return View(new List<Roast>());
        }

        public IActionResult NewRoast()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
