using ApiTest.Models;
using ApiTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiTest.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            var messages = await _messageService.GetAllMessagesAsync();
            if (messages != null)
            {
                return messages;
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _messageService.GetMessageByIdAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            // No necesitas manejar el EntityState.Modified aquí, UserService puede encargarse de eso.
            await _messageService.UpdateMessageAsync(id, message);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            if (message == null)
            {
                return Problem("Entity 'ticket' is null.");
            }
            await _messageService.CreateMessageAsync(message);

            return Ok(message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _messageService.GetMessageByIdAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            await _messageService.DeleteMessageAsync(id);

            return Ok();
        }

        [HttpGet("messages-ticket{ticket.id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByTicket(Ticket ticket)
        {
            var messages = await _messageService.GetMessagesByTicketAsync(ticket);
            if (messages == null)
            {
                return BadRequest();
            }
            return messages;
        }
    }
}
