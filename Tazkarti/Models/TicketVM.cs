using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class TicketVM
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public string EventNameAr { get; set; }
        public bool Valid { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required")]
        public string Email { get; set; }
        [Phone]
        [Required(ErrorMessage = "Phone Number Is Required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }
    }
}
