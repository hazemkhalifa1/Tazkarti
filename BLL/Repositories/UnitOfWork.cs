using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;

namespace BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _context;
        public IGenaricRepository<Event> EventRepository { get; set; }
        public ITicketRepository TicketRepository { get; set; }

        public UnitOfWork(ITicketRepository ticketRepository, IGenaricRepository<Event> eventRepository, AppDbContext context)
        {
            TicketRepository = ticketRepository;
            EventRepository = eventRepository;
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
