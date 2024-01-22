using ApiTest.Models;
using ApiTest.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserRepository : GenericRepository<User>
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager, DbContext dbContext) : base(dbContext)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> ChangeUserPasswordAsync(int userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.ChangePasswordAsync(user, null, newPassword);
    }
}