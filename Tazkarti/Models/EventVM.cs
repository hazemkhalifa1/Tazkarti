using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class EventVM
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string NameAr { get; set; }
        [MaxLength(50)]
        public string place { get; set; }
        [MaxLength(50)]
        public string placeAr { get; set; }
        public DateTime? Time { get; set; }
        public int NoOfTickets { get; set; }
        public decimal Price { get; set; }
        [MaxLength(100)]
        public string? Info { get; set; }
        [MaxLength(100)]
        public string? InfoAr { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? Image { get; set; }

    }
}
