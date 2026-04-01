using Microsoft.AspNetCore.Identity;

namespace DAL.Entities
{
    public class AppUser : IdentityUser
    {
        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
