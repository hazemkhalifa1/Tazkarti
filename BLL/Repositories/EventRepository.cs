using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;

namespace BLL.Repositories
{
    public class EventRepository : GenaricRepository<Event>, IEventRepository
    {
        private readonly AppDbContext _dbContext;
        public EventRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public new async Task AddAsync(Event e)
        {
            await base.AddAsync(e);
            await AddTikAsync(e, e.NoOfTickets);
        }

        public async Task UpdateAsync(Event ev)
        {
            var e = await _dbContext.Events.FindAsync(ev.Id);
            if (e != null)
            {
                if (e.NoOfTickets < ev.NoOfTickets)
                    await AddTikAsync(ev, ev.NoOfTickets - e.NoOfTickets);
                else
                    _dbContext.Tickets.RemoveRange(_dbContext.Tickets.Where(t => t.Valid == true).TakeLast(e.NoOfTickets - ev.NoOfTickets));
                if (e.NoOfTickets == 0)
                {
                    e.NoOfTickets = ev.NoOfTickets;
                    e.NoOfAvailableTickets = 0;
                }
                else
                {
                    e.NoOfTickets = ev.NoOfTickets;
                }
                _dbContext.Events.Update(e);
            }
        }

        public async Task AddTikAsync(Event e, int num)
        {
            for (int i = 1; i <= num; i++)
            {
                await _dbContext.Tickets.AddAsync(new Ticket
                {
                    EventID = e.Id,
                    Valid = true
                });
            }
        }
    }
}
