using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiTest.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public UserController(UserService userService, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<User> signInManager)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users-dto")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersDTO()
        {
            var usersDTO = await _userService.GetAllUsersDTOAsync();
            return Ok(usersDTO);
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

        [HttpGet("{id}-dto")]
        public async Task<ActionResult<UserDTO>> GetUserDTO(int id)
        {
            var user = await _userService.GetUserDTOByIdAsync(id);
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
            // No necesitas manejar el EntityState.Modified aquí, UserService puede encargarse de eso.
            var result = await _userService.UpdateUserAsync(id, user);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                // Maneja errores de actualización según sea necesario.
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
                // Maneja errores de eliminación según sea necesario.
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
                // Aquí puedes generar y devolver un token de autenticación JWT u otro tipo de respuesta apropiada.
                var token = GenerateJwtToken(user);
                return Ok(token);
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
                // Puedes agregar más reclamaciones según sea necesario
            };

            var token = new JwtSecurityToken(
                issuer: "ApiTest",
                audience: "SupportUser",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Cambia esto según tus requisitos de expiración
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}