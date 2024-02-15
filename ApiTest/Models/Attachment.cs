using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiTest.Models
{
    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Path { get; set; }
        public int MessageID { get; set; }
        [ForeignKey("MessageID")]
        public Message Message { get; set; }

        public Attachment()
        { 
            this.Path = string.Empty;
        }

        public Attachment(string path, int MessageID)
        {
            this.Path = path;
            this.MessageID = MessageID;
        }
    }
}
