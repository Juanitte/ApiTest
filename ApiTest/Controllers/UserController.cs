using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public UserController(UserService userService, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public async Task<ActionResult<User>> PostUser(UserDTO userDTO, string password)
        {
            var user = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber
            };
            
            var createUser = await _userManager.CreateAsync(user, password);

            if (!createUser.Succeeded)
            {
                return Problem();
            }
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
    }
}