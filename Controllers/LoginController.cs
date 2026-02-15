using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Services;
using SpotifyRoast.Models;

namespace SpotifyRoast.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUser _userService;

        public LoginController(IUser userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            var response = _userService.Login(username, password);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Set Session/Cookie (Simplified for now - real app uses ClaimsIdentity)
                // Since we configured Auth in Program.cs, we should ideally sign them in properly or use Session
                HttpContext.Session.SetString("Username", response.Data.Username);
                // For a proper implementation we would use HttpContext.SignInAsync but let's stick to Session for this "simple" version
                // OR duplicate the Reference Project's logic?
                // Reference project uses: `HttpContext.Session.SetString("Token", token);` and middleware reads it.
                // My Program.cs setup expects standard Auth or Session.
                // Let's iterate: Set Session.
                
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = response.Message;
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.Register(user, "User"); // Default to User role
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Error = response.Message;
            }
            return View(user);
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
