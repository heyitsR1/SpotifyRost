using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Services;

namespace SpotifyRoast.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenu _menuService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuViewComponent(IMenu menuService, IHttpContextAccessor httpContextAccessor)
        {
            _menuService = menuService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var menus = await _menuService.GetMenusForUser(userId.Value);
                return View(menus);
            }
            return View(new List<SpotifyRoast.Models.Menu>());
        }
    }
}
