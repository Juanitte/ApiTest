using ApiTest.Models;
using ApiTest.Repositories;

namespace ApiTest.Services
{
    public class TicketService
    {
        private readonly GenericRepository<Ticket> _ticketRepository;

        public TicketService(GenericRepository<Ticket> ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _ticketRepository.GetByIdAsync(ticketId);
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            return await _ticketRepository.AddAsync(ticket);
        }

        public async Task UpdateTicketAsync(int ticketId, Ticket updatedTicket)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(ticketId);

            if (existingTicket != null)
            {
                // Actualizar propiedades del ticket.
                existingTicket.Title = updatedTicket.Title;
                existingTicket.Content = updatedTicket.Content;
                existingTicket.Priority = updatedTicket.Priority;
                existingTicket.State = updatedTicket.State;

                await _ticketRepository.UpdateAsync(existingTicket);
            }
            // Puedes manejar el caso en que el ticket no existe según tus necesidades.
        }

        public async Task DeleteTicketAsync(int ticketId)
        {
            await _ticketRepository.DeleteAsync(ticketId);
        }
    }
}