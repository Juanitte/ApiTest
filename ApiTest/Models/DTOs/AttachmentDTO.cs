namespace ApiTest.Models.DTOs
{
    public class AttachmentDTO
    {
        public string Path { get; set; }
        public int MessageID { get; set; }

        public AttachmentDTO()
        {
            this.Path = string.Empty;
            this.MessageID = 0;
        }

        public AttachmentDTO(string path, int messageID)
        {
            this.Path = path;
            this.MessageID = messageID;
        }
    }
}
