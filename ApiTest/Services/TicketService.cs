﻿using ApiTest.Models;
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

        public async Task<List<TicketDTO>> GetAllTicketsDTOAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();

            var ticketDTOs = tickets.Select(ticket => new TicketDTO
            {
                Id = ticket.Id,
                Name = ticket.Name,
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
                    Name = ticket.Name,
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
                existingTicket.Name = updatedTicket.Name;
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

        public async Task<bool> AsignTicket(int ticketId, User user)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.User = user;
                ticket.UserID = user.Id;
                await _ticketRepository.UpdateAsync(ticket);   
                return true;
            }
            return false;
        }

        public async Task<List<TicketDTO>> GetTicketsDTOByUser(User user)
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var result = new List<TicketDTO>();
            if(tickets != null)
            {
                foreach (var ticket in tickets)
                {
                    if (ticket.User.Id == user.Id)
                    {
                        var ticketDTO = new TicketDTO
                        {
                            Id = ticket.Id,
                            Name = ticket.Name,
                            Content = ticket.Content,
                            Email = ticket.Email,
                            Timestamp = ticket.Timestamp,
                            Priority = ticket.Priority,
                            State = ticket.State
                        };
                        result.Add(ticketDTO);
                    }
                }
                return result;
            }
            return null;
        }
    }
}