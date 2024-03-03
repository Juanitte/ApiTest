using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public UserController(UserService userService, TicketService ticketService, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<User> signInManager)
        {
            _userService = userService;
            _ticketService = ticketService;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            if(users == null)
            {
                return BadRequest();
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            User user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.PhoneNumber = userDTO.PhoneNumber;
            var result = await _userService.UpdateUserAsync(id, user);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return Problem("Error updating user.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDTO userDTO)
        {
            var user = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber
            };
            
            var createUser = await _userManager.CreateAsync(user, userDTO.Password);

            if (!createUser.Succeeded)
            {
                var errorMessage = string.Join(", ", createUser.Errors.Select(error => error.Description));
                return BadRequest(errorMessage);
            }

            await _userManager.AddToRoleAsync(user, "SupportTechnician");

            return Ok(createUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return Problem("Error deleting user.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email");
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.IsNullOrEmpty())
                {
                    Console.WriteLine("Roles is Empty");
                }else
                {
                    Console.WriteLine("Roles is not Empty");
                }
                var userRole = roles.FirstOrDefault();
                var token = GenerateJwtToken(user);
                return Ok(new { token, userId = user.Id, userName = user.UserName, email = user.Email, role = userRole });
            }
            else
            {
                return Unauthorized("Invalid password");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("!$Uw6e~T4%tQ@z#sXv9&gYb2^hV*pN7cF")); // Cambia esto por una clave secreta segura
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim("userName", user.UserName.ToString()),
                new Claim("email", user.Email.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "ApiTest",
                Audience = "SupportUser",
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidateToken(out string token)
        {
            token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("!$Uw6e~T4%tQ@z#sXv9&gYb2^hV*pN7cF");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "ApiTest",
                    ValidateAudience = true,
                    ValidAudience = "SupportUser",
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}