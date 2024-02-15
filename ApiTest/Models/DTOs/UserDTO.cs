namespace ApiTest.Models.DTOs
{
    public class UserDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public UserDTO()
        {
            this.UserName = string.Empty;
            this.Email = string.Empty;
            this.PhoneNumber = string.Empty;
        }
    }
}
