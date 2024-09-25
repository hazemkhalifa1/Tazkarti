using Microsoft.AspNetCore.Identity;

namespace DAL.Entities
{
    public class AppUser : IdentityUser
    {
        public bool Agree { get; set; }
        public ICollection<Ticket> tickets { get; set; }

    }
}
