using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiTest.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Content { get; set; }
        public List<Attachment?> AttachmentPaths { get; set; } = new List<Attachment?>();
        public int TicketID { get; set; }
        [ForeignKey("TicketID")]
        public Ticket? Ticket { get; set; }

        public Message()
        {
            Id = 0;
            Content = string.Empty;
            TicketID = 0;
            Ticket = null;
        }

        public Message(string content, int ticketId)
        {
            Id = 0;
            Content = content;
            TicketID = ticketId;
            Ticket = null;
        }

        public Message(string content, List<Attachment?> attachmentPaths, int ticketId)
        {
            Id = 0;
            Content = content;
            AttachmentPaths = attachmentPaths;
            TicketID = ticketId;
            Ticket = null;
        }
    }
}
