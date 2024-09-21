using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace BLL.Repositories
{
    public class TicketRepository : GenaricRepository<Ticket>, ITicketRepository
    {
        private readonly UserManager<AppUser> _userManager;
        public TicketRepository(AppDbContext dbContext, UserManager<AppUser> userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        public void Book(int EventId, string UserId, int NoumOfTic)
        {
            var ev = _dbContext.Events.Where(e => e.Id == EventId).FirstOrDefault();
            if (ev != null)
            {
                var tic = _dbContext.Tickets.Where(t => t.EventID == EventId && t.Valid == true).Take(NoumOfTic);
                foreach (Ticket t in tic)
                {
                    t.Valid = false;
                    t.UserID = UserId;
                    ev.NoOfTickets--;
                    t.user = _userManager.FindByIdAsync(UserId).Result;
                }
                _dbContext.Events.Update(ev);
                _dbContext.Tickets.UpdateRange(tic);
                _dbContext.SaveChanges();
            }
        }
    }
}
