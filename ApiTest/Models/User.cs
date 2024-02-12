using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Transactions;

namespace ApiTest.Models
{
    public class User : IdentityUser<int>
    {
        public List<Ticket?> Tickets { get; set; }

        public User()
        {
            Tickets = new List<Ticket?>();
        }
    }
}
