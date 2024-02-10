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
        public byte[]? Attachment { get; set; }
        public int TicketID { get; set; }
        [ForeignKey("TicketID")]
        public Ticket? Ticket { get; set; }

        public Message()
        {
            Id = 0;
            Content = string.Empty;
            Attachment = null;
            TicketID = 0;
            Ticket = null;
        }

        public Message(string content, int ticketId)
        {
            Id = 0;
            Content = content;
            Attachment = null;
            TicketID = ticketId;
            Ticket = null;
        }

        public Message(string content, byte[] attachment, int ticketId)
        {
            Id = 0;
            Content = content;
            Attachment = attachment;
            TicketID = ticketId;
            Ticket = null;
        }
    }
}
