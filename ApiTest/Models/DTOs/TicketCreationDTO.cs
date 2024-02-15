namespace ApiTest.Models.DTOs
{
    public class TicketCreationDTO
    {
        public TicketDTO TicketDTO { get; set; }
        public MessageDTO? MessageDTO { get; set; }

        public TicketCreationDTO()
        {
            this.TicketDTO = new TicketDTO();
            this.MessageDTO = null;
        }

        public TicketCreationDTO(TicketDTO ticket)
        {
            this.TicketDTO = ticket;
            this.MessageDTO = null;
        }
        public TicketCreationDTO(TicketDTO ticket, MessageDTO message) 
        { 
            this.TicketDTO = ticket;
            this.MessageDTO = message;
        }
    }
}
