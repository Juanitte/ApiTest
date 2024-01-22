using ApiTest.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiTest.Authentication
{
    public static class Roles
    {
        public static void Initialize(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            // Crear roles predeterminados
            string[] roleNames = { "SupportManager", "SupportTechnician" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(roleName).Result;

                if (!roleExist)
                {
                    roleResult = roleManager.CreateAsync(new IdentityRole<int>(roleName)).Result;
                }
            }

            // Crear un usuario predeterminado solo si no existe
            var supportManagerEmail = "manager@example.com";
            var user = userManager.FindByEmailAsync(supportManagerEmail).Result;

            if (user == null)
            {
                var supportManager = new User
                {
                    UserName = "SupportManager",
                    Email = supportManagerEmail,
                };

                string supportManagerPassword = "Admin123#";
                var createSupportManager = userManager.CreateAsync(supportManager, supportManagerPassword).Result;

                if (createSupportManager.Succeeded)
                {
                    // Asignar el rol de SupportManager al usuario
                    userManager.AddToRoleAsync(supportManager, "SupportManager").Wait();
                }
            }
        }
    }
}
