﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiTest.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserID { get; set; }
        public string Priority { get; set; }
        public string State { get; set; }
        public List<Message?> Messages { get; set; } = new List<Message?>();

        [ForeignKey("UserID")]
        public User? User { get; set; }

        public Ticket()
        {
            this.Title = string.Empty;
            this.Name = string.Empty;
            this.Email = string.Empty;
            this.Timestamp = DateTime.Now;
            this.UserID = 1;
            this.Priority = Priorities.NOT_SURE.ToString();
            this.State = States.PENDING.ToString();
        }
        public Ticket(string title, string name, string email)
        {
            this.Title = title;
            this.Name = name;
            this.Email = email;
            this.Timestamp = DateTime.Now;
            this.UserID = 1;
            this.Priority = Priorities.NOT_SURE.ToString();
            this.State = States.PENDING.ToString();
        }

        public Ticket(Message message) {
            this.Title = string.Empty;
            this.Name = string.Empty;
            this.Email = string.Empty;
            this.Timestamp = DateTime.Now;
            this.UserID = 1;
            this.Priority = Priorities.NOT_SURE.ToString();
            this.State = States.PENDING.ToString();
            this.Messages.Add(message);
        }
        public Ticket(string title, string name, string email, Message message)
        {
            this.Title = title;
            this.Name = name;
            this.Email = email;
            this.Timestamp = DateTime.Now;
            this.UserID = 1;
            this.Priority = Priorities.NOT_SURE.ToString();
            this.State = States.PENDING.ToString();
            this.Messages.Add(message);
        }
    }
}
