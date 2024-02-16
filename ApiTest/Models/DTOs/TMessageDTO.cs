namespace ApiTest.Models.DTOs
{
    public class TMessageDTO
    {
        public string Content { get; set; }
        public List<IFormFile?> Attachments { get; set; }

        public TMessageDTO()
        {
            this.Content = string.Empty;
            this.Attachments = new List<IFormFile?>();
        }

        public TMessageDTO(string content)
        {
            this.Content = content;
            this.Attachments = new List<IFormFile?>();
        }

        public TMessageDTO(string content, List<IFormFile?> attachments)
        {
            this.Content = content;
            this.Attachments = attachments;
        }
    }
}
