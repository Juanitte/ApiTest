using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;
        private readonly UserService _userService;
        private readonly MessageService _messageService;

        public TicketController(TicketService ticketService, UserService userService, MessageService messageService)
        {
            _ticketService = ticketService;
            _userService = userService;
            _messageService = messageService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            var tickets = await _ticketService.GetAllTicketsAsync();
            if (tickets != null)
            {
                return tickets;
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            await _ticketService.UpdateTicketAsync(id, ticket);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket([FromForm] TicketCreationDTO ticketCreatorDTO)
        {

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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var messages = await _messageService.GetMessagesByTicketAsync(id);
            foreach(var message in messages)
            {
                if (message != null)
                {
                    await _messageService.DeleteMessageAsync(message.Id);
                }
            }

            await _ticketService.DeleteTicketAsync(id);

            return Ok() ;
        }

        [Authorize]
        [HttpPut("{ticketId}-prio-{priority}")]
        public async Task<IActionResult> ChangePriority(int ticketId, Priorities priority)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            var result = await _ticketService.ChangePriority(ticketId, priority);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPut("{ticketId}-state-{state}")]
        public async Task<IActionResult> ChangeState(int ticketId, States state)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            var result = await _ticketService.ChangeState(ticketId, state);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPut("{ticketId}-asign-{userId}")]
        public async Task<IActionResult> AsignTicket(int ticketId, int userId)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }
            var result = await _ticketService.AsignTicket(ticketId, userId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet("tickets-{userId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByUser(int userId)
        {
            if (!ValidateToken(out string token))
            {
                return Unauthorized();
            }

            var tickets = await _ticketService.GetTicketsByUserAsync(userId);
            if (tickets == null)
            {
                return BadRequest();
            }
            return Ok(tickets);
        }

        private async Task<string> SaveAttachmentToFileSystem(IFormFile attachment)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(attachment.FileName);
            var filePath = Path.Combine("C:/ProyectoIoT/Back/ApiTest/AttachmentStorage", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }

            return filePath;
        }

        private bool ValidateToken(out string token)
        {
            token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            Console.WriteLine(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("!$Uw6e~T4%tQ@z#sXv9&gYb2^hV*pN7cF");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "ApiTest",
                    ValidateAudience = true,
                    ValidAudience = "SupportUser",
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
