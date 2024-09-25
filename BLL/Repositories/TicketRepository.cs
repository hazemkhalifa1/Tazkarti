using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Repositories
{
    public class TicketRepository : GenaricRepository<Ticket>, ITicketRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;
        public TicketRepository(AppDbContext dbContext, UserManager<AppUser> userManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public new async Task<Ticket> GetbyIdAsync(int id) => await _dbContext.Tickets.Where(t => t.Id == id).Include(t => t.Event).Include(t => t.user).FirstOrDefaultAsync();
        public async Task Delete(int id)
        {
            Ticket ticket = await GetbyIdAsync(id);
            _dbContext.Tickets.Remove(ticket);
            ticket.Event.NoOfTickets--;
            if (ticket.Valid)
                ticket.Event.NoOfAvailableTickets--;

            _dbContext.Events.Update(ticket.Event);
        }
        public async Task BookAsync(int EventId, string UserId, int NoumOfTic)
        {
            var ev = await _dbContext.Events.Where(e => e.Id == EventId).FirstOrDefaultAsync();
            if (ev != null)
            {
                var tic = _dbContext.Tickets.Where(t => t.EventID == EventId && t.Valid == true).Take(NoumOfTic);
                foreach (Ticket t in tic)
                {
                    t.Valid = false;
                    t.UserID = UserId;
                    ev.NoOfAvailableTickets--;
                    t.user = await _userManager.FindByIdAsync(UserId);
                }
                _dbContext.Events.Update(ev);
                _dbContext.Tickets.UpdateRange(tic);
            }
        }

        public IEnumerable<Ticket> Search(int? SearchValue)
        {
            if (SearchValue != null)
                return _dbContext.Tickets.Where(t => t.Id.ToString().ToLower().Contains(SearchValue.ToString().ToLower())).Include(t => t.user);
            else
                return _dbContext.Tickets.Select(t => t).Include(t => t.user);
        }
    }
}
