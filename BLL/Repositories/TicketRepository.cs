using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Repositories
{
    public class TicketRepository : GenaricRepository<Ticket>, ITicketRepository
    {
        private readonly AppDbContext _dbContext;
        public TicketRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Ticket>> Search(string? SearchValue)
             => SearchValue is not null ?
            await _dbContext.Set<Ticket>().Where(x => x.Id.ToString().Contains(SearchValue)).Include(t => t.Event).ToListAsync() :
            await _dbContext.Set<Ticket>().Include(t => t.Event).ToListAsync();

        public new async Task<Ticket?> GetbyIdAsync(Guid id)
            => await _dbContext.Tickets.Where(t => Equals(t.Id, id))
            .Include(t => t.Event)
            .FirstOrDefaultAsync();

    }
}
