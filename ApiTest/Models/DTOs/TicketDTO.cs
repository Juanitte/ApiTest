namespace ApiTest.Models.DTOs
{
    public class TicketDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public TicketDTO() 
        {
            this.Name = string.Empty;
            this.Email = string.Empty;
        }

        public TicketDTO(string name, string email) 
        {
            this.Name = name;
            this.Email = email;
        }
        
    }
}
