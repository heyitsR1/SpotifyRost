using Microsoft.EntityFrameworkCore;
using SpotifyRoast.Data;
using SpotifyRoast.Models;
using SpotifyRoast.Services;

namespace SpotifyRoast.Repositories
{
    public class MenuRepository : GenericRepository<Menu>, IMenu
    {
        public MenuRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Menu>> GetMenusForUser(int userId)
        {
            // Get RoleIds for the user
            var userRoleIds = await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!userRoleIds.Any())
            {
                return new List<Menu>();
            }

            // Get MenuIds for these roles
            var menuIds = await _dbContext.MenuRoles
                .Where(mr => userRoleIds.Contains(mr.RoleId))
                .Select(mr => mr.MenuId)
                .ToListAsync();

            // Get distinct menus, ordered by SortOrder
            var menus = await _dbContext.Menus
                .Where(m => menuIds.Contains(m.Id) && m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return menus;
        }
    }
}
