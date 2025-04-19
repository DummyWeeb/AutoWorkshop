using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auto.Models;

namespace Auto.Data
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<CustomUser>>();

            string[] roleNames = { "IT", "Warehouse", "Procurement", "Administration" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Создание администратора
            var adminEmail = "admin@auto.ru";
            var adminPassword = "Aa12345678!";
            if (userManager.Users.All(u => u.Email != adminEmail))
            {
                var adminUser = new CustomUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Surname = "Admin",
                    Ima = "Admin",
                    SecSurname = "Admin",
                    PodrazdelenieId = 3
                };
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "IT");
                }
            }
        }
    }
}