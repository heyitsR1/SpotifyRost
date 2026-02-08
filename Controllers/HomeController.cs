using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Models;

namespace SpotifyRoast.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // This action handles application errors by returning the Error view.
        // The ResponseCache attribute ensures that error pages are not cached by the client or proxies.
        // It passes an ErrorViewModel containing a RequestId (from the current Activity or TraceIdentifier)
        // to the view, which is used to correlate the error with server-side logs.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
