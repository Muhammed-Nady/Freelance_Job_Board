using Freelify.Models.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace Freelify.Data
{
    public static class ApplicationDbInitializer
    {

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Client", "Freelancer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var email = "admin@freelify.com";

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {


                admin = new ApplicationUser
                {
                    FullName = "Admin",
                    UserName= "Admin",
                    Email = email,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true

                };

                var result = await userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

        }
    }
}
