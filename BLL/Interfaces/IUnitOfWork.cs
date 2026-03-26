using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenaricRepository<Event> EventRepository { get; set; }
        public ITicketRepository TicketRepository { get; set; }
        public Task SaveChangesAsync();
    }
}
