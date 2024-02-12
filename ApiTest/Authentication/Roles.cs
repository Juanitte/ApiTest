using ApiTest.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiTest.Authentication
{
    public static class Roles
    {
        public static async Task Initialize(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            // Crear roles predeterminados
            string[] roleNames = { "SupportManager", "SupportTechnician" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(roleName));

                    if (!roleResult.Succeeded)
                    {
                        // Manejar error al crear el rol
                        throw new Exception($"Error creating role {roleName}. Errors: {string.Join(", ", roleResult.Errors)}");
                    }
                }
            }

            // Crear un usuario predeterminado solo si no existe
            var supportManagerEmail = "manager@example.com";
            var user = await userManager.FindByEmailAsync(supportManagerEmail);

            if (user == null)
            {
                var supportManager = new User
                {
                    UserName = "SupportManager",
                    Email = supportManagerEmail,
                    PhoneNumber = "123456789"
                };

                string supportManagerPassword = "Admin123#";
                var createSupportManager = await userManager.CreateAsync(supportManager, supportManagerPassword);

                if (!createSupportManager.Succeeded)
                {
                    // Manejar error al crear el usuario
                    throw new Exception($"Error creating SupportManager user. Errors: {string.Join(", ", createSupportManager.Errors)}");
                }

                // Asignar el rol de SupportManager al usuario
                var addToRoleResult = await userManager.AddToRoleAsync(supportManager, "SupportManager");

                if (!addToRoleResult.Succeeded)
                {
                    // Manejar error al asignar el rol al usuario
                    throw new Exception($"Error adding SupportManager user to role. Errors: {string.Join(", ", addToRoleResult.Errors)}");
                }
            }
        }
    }
}
