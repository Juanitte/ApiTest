using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiTest.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;
        private readonly TicketService _ticketService;

        public MessageController(MessageService messageService, TicketService ticketService)
        {
            _messageService = messageService;
            _ticketService = ticketService;
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
        public async Task<IActionResult> PutMessage(int id, MessageDTO messageDTO)
        {
            Message message = await _messageService.GetMessageByIdAsync(id);
            if (message == null)
            {
                return BadRequest();
            }

            message.Content = messageDTO.Content;
            message.TicketID = messageDTO.TicketID;
            if (!messageDTO.Attachments.IsNullOrEmpty())
            {
                message.AttachmentPaths.Clear();
                foreach (var attachment in messageDTO.Attachments)
                {
                    if (attachment != null)
                    {
                        string attachmentPath = await SaveAttachmentToFileSystem(attachment);
                        Attachment newAttachment = new Attachment(attachmentPath, message.Id);
                        message.AttachmentPaths.Add(newAttachment);
                    }
                }
            }

            // No necesitas manejar el EntityState.Modified aquí, UserService puede encargarse de eso.
            await _messageService.UpdateMessageAsync(id, message);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(MessageDTO messageDTO)
        {
            Message message;
            if(messageDTO.Attachments.IsNullOrEmpty())
            {
                message = new Message(messageDTO.Content, messageDTO.TicketID);
            }
            else
            {
                message = new Message(messageDTO.Content, messageDTO.TicketID);
                if (!messageDTO.Attachments.IsNullOrEmpty())
                {
                    foreach (var attachment in messageDTO.Attachments)
                    {
                        if (attachment != null)
                        {
                            string attachmentPath = await SaveAttachmentToFileSystem(attachment);
                            Attachment newAttachment = new Attachment(attachmentPath, message.Id);
                            message.AttachmentPaths.Add(newAttachment);
                        }
                    }
                }
            }
            message.Ticket = await _ticketService.GetTicketByIdAsync(messageDTO.TicketID);
            if (message == null)
            {
                return Problem("Entity 'message' is null.");
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
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByTicket(int ticketId)
        {
            var messages = await _messageService.GetMessagesByTicketAsync(ticketId);
            if (messages == null)
            {
                return BadRequest();
            }
            return messages;
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
