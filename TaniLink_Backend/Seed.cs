using Microsoft.AspNetCore.Identity;

namespace TaniLink_Backend
{
    public class Seed
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public Seed(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedDataContextAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Mitra"));
                await _roleManager.CreateAsync(new IdentityRole("Petani"));
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            else
                Console.WriteLine("Already seeded");
        }
    }
}
