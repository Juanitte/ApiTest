namespace ApiTest.Models.DTOs
{
    public class TicketCreationDTO
    {
        public TicketDTO TicketDTO { get; set; }
        public TMessageDTO MessageDTO { get; set; }

        public TicketCreationDTO()
        {
            this.TicketDTO = new TicketDTO();
            this.MessageDTO = null;
        }
        public TicketCreationDTO(TicketDTO ticket, TMessageDTO message) 
        { 
            this.TicketDTO = ticket;
            this.MessageDTO = message;
        }
    }
}
