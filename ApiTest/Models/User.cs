using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace ApiTest.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<Ticket?> Tickets { get; set; } = new List<Ticket?>();

    }
}
