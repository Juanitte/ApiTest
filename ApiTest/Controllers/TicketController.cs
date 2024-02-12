using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiTest.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        //[Authorize(Roles = "SupportManager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            if (tickets != null)
            {
                return tickets;
            }
            return BadRequest();
        }

        [HttpGet("tickets-dto")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsDTO()
        {
            var ticketsDTO = await _ticketService.GetAllTicketsDTOAsync();
            if (ticketsDTO != null)
            {
                return ticketsDTO;
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        [HttpGet("{id}-dto")]
        public async Task<ActionResult<TicketDTO>> GetTicketDTO(int id)
        {
            var ticket = await _ticketService.GetTicketDTOByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return ticket;
        }

        //[Authorize(Roles = "SupportManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            // No necesitas manejar el EntityState.Modified aquí, UserService puede encargarse de eso.
            await _ticketService.UpdateTicketAsync(id, ticket);

            return Ok();
        }

        [HttpPut("{id}-add-message")]
        public async Task<IActionResult> AddMessageToTicket(int id, MessageDTO messageDTO)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);

            if(ticket == null)
            {
                return BadRequest();
            }
            ticket.Messages.Add(new Message(messageDTO.Content, messageDTO.TicketID));

            await _ticketService.UpdateTicketAsync(id, ticket);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(TicketDTO ticketDTO, MessageDTO messageDTO)
        {
            if (ticketDTO == null)
            {
                return Problem("Entity 'ticket' is null.");
            }
            if (messageDTO == null)
            {
                return Problem("Entity 'message' is null");
            }

            Ticket ticket = new Ticket(ticketDTO.Name, ticketDTO.Email);

            Message message = new Message(messageDTO.Content, messageDTO.TicketID);
            if (!messageDTO.Attachments.IsNullOrEmpty())
            {
                foreach (var attachment in messageDTO.Attachments)
                {
                    if(attachment != null)
                    {
                        string attachmentPath = await SaveAttachmentToFileSystem(attachment);
                        message.AttachmentPaths.Add(attachmentPath);
                    }
                }
            }

            ticket.Messages.Add(message);

            await _ticketService.CreateTicketAsync(ticket);

            return Ok(ticket);
        }

        //[Authorize(Roles = "SupportManager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            await _ticketService.DeleteTicketAsync(id);

            return Ok() ;
        }

        //[Authorize(Roles = "SupportManager")]
        [HttpPut("{id}-prio")]
        public async Task<IActionResult> ChangePriority(int ticketId, Priorities priority)
        {
            var result = await _ticketService.ChangePriority(ticketId, priority);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

       // [Authorize(Roles = "SupportManager")]
        [HttpPut("{id}-state")]
        public async Task<IActionResult> ChangeState(int ticketId, States state)
        {
            var result = await _ticketService.ChangeState(ticketId, state);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        //[Authorize(Roles = "SupportManager")]
        [HttpPut("{id}-asign")]
        public async Task<IActionResult> AsignTicket(int ticketId, int userId)
        {
            var result = await _ticketService.AsignTicket(ticketId, userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        //[Authorize(Roles = "SupportTechnician")]
        [HttpGet("{id}-{user.id}")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsDTOByUser(int userId)
        {
            var ticketsDTO = await _ticketService.GetTicketsDTOByUser(userId);
            if (ticketsDTO == null)
            {
                return BadRequest();
            }
            return ticketsDTO;
        }

        private async Task<string> SaveAttachmentToFileSystem(IFormFile attachment)
        {
            // Generar un nombre de archivo único
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(attachment.FileName);
            // Ruta donde se guardará el archivo en el sistema de archivos
            var filePath = Path.Combine("C:/ProyectoIoT/Back/ApiTest/AttachmentStorage", fileName);

            // Guardar el archivo en el sistema de archivos
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
