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
        public byte[] Attachment { get; set; }
        public int TicketID { get; set; }
        [ForeignKey("TicketID")]
        public Ticket Ticket { get; set; }

    }
}
