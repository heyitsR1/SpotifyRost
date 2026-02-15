using Microsoft.EntityFrameworkCore;
using SpotifyRoast.Models;

namespace SpotifyRoast.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;

        public DbInitializer(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Initialize()
        {
            // Apply migrations if any
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                // Fallback: create the database from the model if migrations fail
                _db.Database.EnsureCreated();
            }

            // Create Roles if they don't exist
            if (!_db.Roles.Any(r => r.Name == "Admin"))
            {
                _db.Roles.Add(new Role { Name = "Admin" });
                _db.Roles.Add(new Role { Name = "User" });
                _db.SaveChanges();
            }

            // Ensure Admin User has Admin Role
            var adminUserRef = _db.Users.FirstOrDefault(u => u.Username == "admin");
            var adminRoleRef = _db.Roles.FirstOrDefault(r => r.Name == "Admin");
            
            if (adminUserRef != null && adminRoleRef != null)
            {
                if (!_db.UserRoles.Any(ur => ur.UserId == adminUserRef.Id && ur.RoleId == adminRoleRef.Id))
                {
                    _db.UserRoles.Add(new UserRole { UserId = adminUserRef.Id, RoleId = adminRoleRef.Id });
                    _db.SaveChanges();
                }
            }
            else if (adminUserRef == null)
            {
                // Create Admin if missing
                 adminUserRef = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = "admin", // Plain text
                    SpotifyId = "admin_spotify"
                };
                _db.Users.Add(adminUserRef);
                _db.SaveChanges();

                if (adminRoleRef != null)
                {
                    _db.UserRoles.Add(new UserRole { UserId = adminUserRef.Id, RoleId = adminRoleRef.Id });
                    _db.SaveChanges();
                }
            }

            // Create Standard User if doesn't exist
            if (!_db.Users.Any(u => u.Username == "user"))
            {
                var user = new User
                {
                    Username = "user",
                    Email = "user@example.com",
                    Password = "user",
                    SpotifyId = "user_spotify"
                };
                _db.Users.Add(user);
                _db.SaveChanges();

                var userRole = _db.Roles.FirstOrDefault(r => r.Name == "User");
                if (userRole != null)
                {
                    _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id });
                    _db.SaveChanges();
                }
            }

            // Create Roast Personalities
            if (!_db.RoastPersonalities.Any())
            {
                _db.RoastPersonalities.AddRange(
                    new RoastPersonality { Name = "The Music Snob", SystemPrompt = "...", Icon = "bi-eyeglasses" },
                    new RoastPersonality { Name = "The Gen Z Hater", SystemPrompt = "...", Icon = "bi-phone" },
                    new RoastPersonality { Name = "The Boomer Dad", SystemPrompt = "...", Icon = "bi-guitar" }
                );
                _db.SaveChanges();
            }

            // Seed Menus
            if (!_db.Menus.Any())
            {
                var dashboardMenu = new Menu { Name = "Dashboard", Url = "/Dashboard", Icon = "bi-speedometer", SortOrder = 1, IsActive = true, ParentId = 0 };
                var personalitiesMenu = new Menu { Name = "Manage Personalities", Url = "/RoastPersonality", Icon = "bi-robot", SortOrder = 2, IsActive = true, ParentId = 0 };
                var logoutMenu = new Menu { Name = "Logout", Url = "/Login/Logout", Icon = "bi-box-arrow-right", SortOrder = 99, IsActive = true, ParentId = 0 };

                _db.Menus.AddRange(dashboardMenu, personalitiesMenu, logoutMenu);
                _db.SaveChanges();

                // Seed MenuRoles
                var adminRole = _db.Roles.FirstOrDefault(r => r.Name == "Admin");
                var userRole = _db.Roles.FirstOrDefault(r => r.Name == "User");

                if (adminRole != null && userRole != null)
                {
                    // Admin gets all
                    _db.MenuRoles.Add(new MenuRole { RoleId = adminRole.Id, MenuId = dashboardMenu.Id });
                    _db.MenuRoles.Add(new MenuRole { RoleId = adminRole.Id, MenuId = personalitiesMenu.Id });
                    _db.MenuRoles.Add(new MenuRole { RoleId = adminRole.Id, MenuId = logoutMenu.Id });

                    // User gets Dashboard and Logout
                    _db.MenuRoles.Add(new MenuRole { RoleId = userRole.Id, MenuId = dashboardMenu.Id });
                    _db.MenuRoles.Add(new MenuRole { RoleId = userRole.Id, MenuId = logoutMenu.Id });
                    
                    _db.SaveChanges();
                }
            }
        }
    }
}
