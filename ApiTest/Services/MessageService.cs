using ApiTest.Models;
using ApiTest.Repositories;

namespace ApiTest.Services
{
    public class MessageService
    {
        private readonly GenericRepository<Message> _messageRepository;
        private readonly GenericRepository<Attachment> _attachmentRepository;

        public MessageService(GenericRepository<Message> messageRepository, GenericRepository<Attachment> attachmentRepository)
        {
            _messageRepository = messageRepository;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            var messages =  await _messageRepository.GetAllAsync();
            var attachments = await _attachmentRepository.GetAllAsync();

            foreach (var message in messages)
            {
                foreach (var attachment in attachments)
                {
                    if(message.Id == attachment.MessageID)
                    {
                        message.AttachmentPaths.Add(attachment);
                    }
                }
            }

            return messages;
        }
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            var message =  await _messageRepository.GetByIdAsync(messageId);
            var attachments = await _attachmentRepository.GetAllAsync();

            foreach (var attachment in attachments)
            {
                if (message.Id == attachment.MessageID)
                {
                    message.AttachmentPaths.Add(attachment);
                }
            }
            return message;
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            return await _messageRepository.AddAsync(message);
        }

        public async Task UpdateMessageAsync(int messageId, Message updatedMessage)
        {
            var existingMessage = await _messageRepository.GetByIdAsync(messageId);

            if (existingMessage != null)
            {
                // Actualizar propiedades del ticket.
                existingMessage.Content = updatedMessage.Content;
                existingMessage.AttachmentPaths = updatedMessage.AttachmentPaths;

                await _messageRepository.UpdateAsync(existingMessage);
            }
            // Puedes manejar el caso en que el ticket no existe según tus necesidades.
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            await _messageRepository.DeleteAsync(messageId);
            var attachments = await _attachmentRepository.GetAllAsync();

            foreach (var attachment in attachments)
            {
                if(attachment.MessageID == messageId)
                {
                    await _attachmentRepository.DeleteAsync(attachment.Id);
                }
            }
        }

        public async Task<List<Message>> GetMessagesByTicketAsync(int ticketId)
        {
            var messages = await _messageRepository.GetAllAsync();
            var attachments = await _attachmentRepository.GetAllAsync();

            if (messages == null)
            {
                return null;
            }
            List<Message> result = new List<Message>();
            foreach (var message in messages)
            {
                if (message.TicketID == ticketId)
                {
                    foreach (var attachment in attachments)
                    {
                        if(attachment.MessageID == message.Id)
                        {
                            message.AttachmentPaths.Add(attachment);
                        }
                    }
                    result.Add(message);
                }
            }
            return result;
        }
    }
}
