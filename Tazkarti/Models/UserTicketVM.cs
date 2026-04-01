namespace Tazkarti.Models
{
    public class UserTicketVM
    {
        public Guid Id { get; set; }
        public EventVM EventVM { get; set; }
        public string UserName { get; set; }
        public bool Valid { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
    }
}
