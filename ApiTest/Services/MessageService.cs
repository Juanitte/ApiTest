using ApiTest.Models;
using ApiTest.Repositories;

namespace ApiTest.Services
{
    public class MessageService
    {
        private readonly GenericRepository<Message> _messageRepository;

        public MessageService(GenericRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            return await _messageRepository.GetAllAsync();
        }
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _messageRepository.GetByIdAsync(messageId);
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
        }

        public async Task<List<Message>> GetMessagesByTicketAsync(int ticketId)
        {
            var messages = await _messageRepository.GetAllAsync();

            if (messages == null)
            {
                return null;
            }
            List<Message> result = new List<Message>();
            foreach (var message in messages)
            {
                if (message.TicketID == ticketId)
                {
                    result.Add(message);
                }
            }
            return result;
        }
    }
}
