using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;


        public IEventRepository EventRepository { get; set; }
        public ITicketRepository TicketRepository { get; set; }
        public UnitOfWork(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            TicketRepository = new TicketRepository(_dbContext, userManager);
            EventRepository = new EventRepository(_dbContext);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
