using Microsoft.AspNetCore.Identity;

namespace OnlineCasino.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Create roles
            string[] roleNames = { "Admin", "Manager", "Player" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@casino.cz";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Create manager user
            var managerEmail = "manager@casino.cz";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);

            if (managerUser == null)
            {
                var manager = new IdentityUser
                {
                    UserName = "manager",
                    Email = managerEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(manager, "Manager123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "Manager");
                }
            }
        }
    }
}
