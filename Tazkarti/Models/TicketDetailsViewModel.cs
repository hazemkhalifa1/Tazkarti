using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class TicketDetailsViewModel
    {
        public Guid EventId { get; set; }
        public int NumberOfTickets { get; set; }
        public List<TicketInfo> Tickets { get; set; }
    }

    public class TicketInfo
    {
        [Required(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number Is Required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
