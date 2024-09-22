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
            AddTik(e, e.NoOfTickets);
            _dbContext.SaveChanges();
        }

        public void Update(Event ev)
        {
            var e = _dbContext.Events.Find(ev.Id);
            if (e != null)
            {
                if (e.NoOfTickets < ev.NoOfTickets)
                {
                    AddTik(ev, ev.NoOfTickets - e.NoOfTickets);
                }
                _dbContext.Events.Update(ev);
            }
        }

        public void AddTik(Event e, int num)
        {
            for (int i = 1; i <= num; i++)
            {
                _dbContext.Tickets.Add(new Ticket
                {
                    EventID = e.Id,
                    Valid = true,
                    Event = e
                });
            }
        }
    }
}
