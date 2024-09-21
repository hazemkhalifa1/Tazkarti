using DAL.Entities;

namespace BLL.Interfaces
{
    public interface ITicketRepository : IGenaricRepository<Ticket>
    {
        public void Book(int EventId, string UserId, int NoumOfTic);
    }
}
