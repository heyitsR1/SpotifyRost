using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
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
        public async Task<IActionResult> Index(string username, string password)
        {
            var response = _userService.Login(username, password);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var user = response.Data;

                // Create Claims
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

                // Add Roles
                if (user.UserRoles != null)
                {
                    foreach (var userRole in user.UserRoles)
                    {
                        claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, userRole.Role.Name));
                    }
                }

                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                    claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, 
                    new System.Security.Claims.ClaimsPrincipal(claimsIdentity), 
                    authProperties);

                // Keep Session for MenuService (or refactor MenuService to use User.Identity)
                // Let's keep Session for now to minimize changes to dependencies
                HttpContext.Session.SetInt32("UserId", user.Id); 

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
        public IActionResult SignUp(SpotifyRoast.Models.User user)
        {
            // Remove UserRoles and Roasts from validation as they are empty on signup
            ModelState.Remove(nameof(SpotifyRoast.Models.User.UserRoles));
            ModelState.Remove(nameof(SpotifyRoast.Models.User.Roasts));
            ModelState.Remove("UserRoles"); 
            ModelState.Remove("Roasts");

            if (ModelState.IsValid)
            {
                var response = _userService.Register(user, "User"); 
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Success"] = "Account created! Please login.";
                    return RedirectToAction("Index");
                }
                ViewBag.Error = response.Message;
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Signup Validation Error: {error}");
                }
                ViewBag.Error = "Validation failed: " + string.Join(", ", errors);
            }
            return View(user);
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
