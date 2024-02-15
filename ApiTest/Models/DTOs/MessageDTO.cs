namespace ApiTest.Models.DTOs
{
    public class MessageDTO
    {
        public string Content { get; set; }
        public List<IFormFile?> Attachments { get; set; }
        public int TicketID { get; set; }

        public MessageDTO()
        {
            this.Content = string.Empty;
            this.Attachments = new List<IFormFile?>();
            this.TicketID = 0;
        }

        public MessageDTO(string content, int ticketId)
        {
            this.Content = content;
            this.Attachments = new List<IFormFile?>();
            this.TicketID = ticketId;
        }

        public MessageDTO(string content, List<IFormFile?> attachments, int ticketId)
        {
            this.Content = content;
            this.Attachments = attachments;
            this.TicketID = ticketId;
        }
    }
}
