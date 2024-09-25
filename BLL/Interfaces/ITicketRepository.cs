using DAL.Entities;

namespace BLL.Interfaces
{
    public interface ITicketRepository : IGenaricRepository<Ticket>
    {
        public Task BookAsync(int EventId, string UserId, int NoumOfTic);
        public IEnumerable<Ticket> Search(int? SearchValue);
        public Task Delete(int id);
    }
}
