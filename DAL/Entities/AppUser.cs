using Microsoft.AspNetCore.Identity;

namespace DAL.Entities
{
    public class AppUser : IdentityUser
    {
        public ICollection<Ticket> tickets { get; set; }
    }
}
