using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ApiTest.Services
{
    public class UserService
    {
        private readonly GenericRepository<User> _userRepository;

        public UserService(GenericRepository<User> repository)
        {
            _userRepository = repository;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<List<UserDTO>> GetAllUsersDTOAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var userDTOs = users.Select(user => new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            }).ToList();

            return userDTOs;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<UserDTO> GetUserDTOByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                var userDTO = new UserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                return userDTO;
            }
            return null;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(int userId, User updatedUser)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);

            if (existingUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Actualizar propiedades del usuario.
            existingUser.UserName = updatedUser.UserName;
            existingUser.Email = updatedUser.Email;
            existingUser.PhoneNumber = updatedUser.PhoneNumber;

            await _userRepository.UpdateAsync(existingUser);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteUserAsync(int userId)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);

            if (existingUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            await _userRepository.DeleteAsync(userId);
            return IdentityResult.Success;
        }
    }
}