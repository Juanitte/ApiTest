using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiTest.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserID { get; set; }
        public string Priority { get; set; }
        public string State { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        public Ticket() {
            this.Title = string.Empty;
            this.Content = string.Empty;
            this.Timestamp = DateTime.Now;
            this.UserID = -1;
            this.Priority = Priorities.NOT_SURE.ToString();
            this.State = States.PENDING.ToString();
            this.User = null;
        }
    }
}
