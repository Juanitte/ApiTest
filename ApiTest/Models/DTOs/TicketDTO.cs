namespace ApiTest.Models.DTOs
{
    public class TicketDTO
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public TicketDTO() 
        {
            this.Title = string.Empty;
            this.Name = string.Empty;
            this.Email = string.Empty;
        }

        public TicketDTO(string title, string name, string email) 
        {
            this.Title = title;
            this.Name = name;
            this.Email = email;
        }
        
    }
}
