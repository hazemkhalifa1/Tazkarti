using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;

namespace BLL.Repositories
{
    public class EventRepository : GenaricRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public void Add(Event e)
        {
            _dbContext.Events.Add(e);
            for (int i = 1; i <= e.NoOfTickets; i++)
            {
                _dbContext.Tickets.Add(new Ticket
                {
                    EventID = e.Id,
                    Valid = true,
                    Event = e
                });
            }
            _dbContext.SaveChanges();
        }
    }
}
