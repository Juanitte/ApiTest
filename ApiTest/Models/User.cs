using Microsoft.AspNetCore.Identity;

namespace ApiTest.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<Ticket> Tickets { get; set; }
    }
}
