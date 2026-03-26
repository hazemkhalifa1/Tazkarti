using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Event : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string NameAr { get; set; }
        [MaxLength(50)]
        public string place { get; set; }
        [MaxLength(50)]
        public string placeAr { get; set; }
        public DateTime Time { get; set; }
        public int NoOfTickets { get; set; }
        public decimal Price { get; set; }
        public string Info { get; set; }
        public string InfoAr { get; set; }
        public string? ImageName { get; set; }
        public ICollection<Ticket>? tickets { get; set; }

    }
}
