namespace ApiTest.Models.DTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Email { get; set; }
        public DateTime Timestamp { get; set; }
        public string Priority { get; set; }
        public string State { get; set; }
        
    }
}
