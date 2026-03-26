using DAL.Entities;

namespace BLL.Interfaces
{
    public interface ITicketRepository : IGenaricRepository<Ticket>
    {
        public Task<IEnumerable<Ticket>> Search(string? SearchValue);

        public Task<Ticket?> GetbyIdAsync(Guid id);
    }
}
