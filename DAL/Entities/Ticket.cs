using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Ticket : BaseEntity
    {
        public bool Valid { get; set; }
        [ForeignKey("user")]
        public string? UserID { get; set; }
        public AppUser? user { get; set; }
        [ForeignKey("event")]
        public int EventID { get; set; }
        public Event Event { get; set; }

    }
}
