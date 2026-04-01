using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Ticket : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Valid { get; set; }

        [ForeignKey(nameof(Event))]
        public Guid EventID { get; set; }
        public Event Event { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public AppUser User { get; set; }

    }
}
