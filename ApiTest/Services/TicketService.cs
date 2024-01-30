using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Repositories;
using Microsoft.AspNetCore.Authorization;

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

        public async Task<List<TicketDTO>> GetAllTicketsDTOAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();

            var ticketDTOs = tickets.Select(ticket => new TicketDTO
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Content = ticket.Content,
                Email = ticket.Email,
                Timestamp = ticket.Timestamp,
                Priority = ticket.Priority,
                State = ticket.State
            }).ToList();

            return ticketDTOs;
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _ticketRepository.GetByIdAsync(ticketId);
        }

        public async Task<TicketDTO> GetTicketDTOByIdAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket != null)
            {
                var ticketDTO = new TicketDTO
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Content = ticket.Content,
                    Email = ticket.Email,
                    Timestamp = ticket.Timestamp,
                    Priority = ticket.Priority,
                    State = ticket.State
                };
                return ticketDTO;
            }
            return null;
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

        [Authorize(Roles = "SupportManager")]
        public async Task ChangePriority(int ticketId, Priorities priority)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.Priority = priority.ToString();
                await _ticketRepository.UpdateAsync(ticket);
            }
        }

        [Authorize(Roles = "SupportManager")]
        public async Task ChangeState(int ticketId, States state)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if(ticket != null)
            {
                ticket.State = state.ToString();
                await _ticketRepository.UpdateAsync(ticket);
            }
        }

        [Authorize(Roles = "SupportManager")]
        public async Task AsignTicket(int ticketId, User user)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.User = user;
                ticket.UserID = user.Id;
                await _ticketRepository.UpdateAsync(ticket);
            }
        }
    }
}