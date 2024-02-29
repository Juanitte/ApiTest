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
        private readonly UserService _userService;

        public TicketController(TicketService ticketService, UserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
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

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket([FromForm] TicketCreationDTO ticketCreatorDTO)
        {
            Console.WriteLine("Datos recibidos en el controlador:");
            Console.WriteLine("Title: " + ticketCreatorDTO.TicketDTO.Title);
            Console.WriteLine("Name: " + ticketCreatorDTO.TicketDTO.Name);
            Console.WriteLine("Email: " + ticketCreatorDTO.TicketDTO.Email);
            Console.WriteLine("Content: " + ticketCreatorDTO.MessageDTO.Content);
            Console.WriteLine("Attachments: " + ticketCreatorDTO.MessageDTO.Attachments);

            if (ticketCreatorDTO == null)
            {
                return Problem("Entity 'ticketCreatorDTO' is null.");
            }
            if (ticketCreatorDTO.TicketDTO == null)
            {
                return Problem("Entity 'ticketDTO' is null");
            }
            if(ticketCreatorDTO.MessageDTO == null)
            {
                return Problem("Entity 'messageDTO' is null");
            }

            Ticket ticket = new Ticket(ticketCreatorDTO.TicketDTO.Title, ticketCreatorDTO.TicketDTO.Name, ticketCreatorDTO.TicketDTO.Email);
            
            Message message = new Message(ticketCreatorDTO.MessageDTO.Content, ticket.Id);

            if (!ticketCreatorDTO.MessageDTO.Attachments.IsNullOrEmpty())
            {
                foreach (var attachment in ticketCreatorDTO.MessageDTO.Attachments)
                {
                    if (attachment != null)
                    {
                        string attachmentPath = await SaveAttachmentToFileSystem(attachment);
                        Attachment newAttachment = new Attachment(attachmentPath, message.Id);
                        message.AttachmentPaths.Add(newAttachment);
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
        [HttpPut("{ticketId}-prio-{priority}")]
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
        [HttpPut("{ticketId}-state-{state}")]
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
        [HttpPut("{ticketId}-asign-{userId}")]
        public async Task<IActionResult> AsignTicket(int ticketId, int userId)
        {
            var result = await _ticketService.AsignTicket(ticketId, userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        //[Authorize(Roles = "SupportManager, SupportTechnician")]
        [HttpGet("tickets-{userId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByUser(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserAsync(userId);
            Console.WriteLine("Datos a enviar al front:");
            Console.WriteLine("Id: " + tickets.First().Id);
            if (tickets == null)
            {
                return BadRequest();
            }
            return Ok(tickets);
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
