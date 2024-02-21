using ApiTest.Models;
using ApiTest.Models.DTOs;
using ApiTest.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

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
                existingTicket.Name = updatedTicket.Name;
                existingTicket.Priority = updatedTicket.Priority;
                existingTicket.State = updatedTicket.State;
                existingTicket.Messages = updatedTicket.Messages;

                await _ticketRepository.UpdateAsync(existingTicket);
            }
            // Puedes manejar el caso en que el ticket no existe según tus necesidades.
        }

        public async Task DeleteTicketAsync(int ticketId)
        {
            await _ticketRepository.DeleteAsync(ticketId);
        }

        public async Task<bool> ChangePriority(int ticketId, Priorities priority)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.Priority = priority.ToString();
                await _ticketRepository.UpdateAsync(ticket);
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeState(int ticketId, States state)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if(ticket != null)
            {
                ticket.State = state.ToString();
                await _ticketRepository.UpdateAsync(ticket);
                return true;
            }
            return false;
        }

        public async Task<bool> AsignTicket(int ticketId, int userId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.UserID = userId;
                await _ticketRepository.UpdateAsync(ticket);   
                return true;
            }
            return false;
        }

        public async Task<List<Ticket?>> GetTicketsByUserAsync(User user)
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var result = new List<Ticket>();
            if(tickets != null)
            {
                foreach (var ticket in tickets)
                {
                    if (ticket != null)
                    {
                        if (ticket.UserID == user.Id)
                        {                         
                            result.Add(ticket);
                        }
                    }
                }
                return result;
            }
            return null;
        }
    }
}