namespace Tazkarti.Models
{
    public class TicketVM
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public bool Valid { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
