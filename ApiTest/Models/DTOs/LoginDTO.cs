namespace ApiTest.Models.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginDTO()
        {
            this.Email = string.Empty;
            this.Password = string.Empty;
        }
    }
}
