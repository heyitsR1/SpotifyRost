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
            }

            // Create Roles if they don't exist
            if (!_db.Roles.Any(r => r.Name == "Admin"))
            {
                _db.Roles.Add(new Role { Name = "Admin" });
                _db.Roles.Add(new Role { Name = "User" });
                _db.SaveChanges();
            }

            // Create Admin User if doesn't exist
            if (!_db.Users.Any(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = "admin", // Plain text for now as per previous checks
                    SpotifyId = "admin_spotify"
                };
                _db.Users.Add(adminUser);
                _db.SaveChanges();

                var adminRole = _db.Roles.FirstOrDefault(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    _db.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
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
                    new RoastPersonality
                    {
                        Name = "The Music Snob",
                        SystemPrompt = "You are a pretentious music critic who hates everything popular.",
                        Icon = "bi-eyeglasses"
                    },
                    new RoastPersonality
                    {
                        Name = "The Gen Z Hater",
                        SystemPrompt = "You are a Gen Z TikToker who thinks anything older than 2020 is cringe.",
                        Icon = "bi-phone"
                    },
                    new RoastPersonality
                    {
                        Name = "The Boomer Dad",
                        SystemPrompt = "You are a dad who only likes Classic Rock and doesn't understand 'computer music'.",
                        Icon = "bi-guitar"
                    }
                );
                _db.SaveChanges();
            }
        }
    }
}
