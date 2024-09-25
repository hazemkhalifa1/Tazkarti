namespace BLL.Interfaces
{
    public interface IUnitOfWork
    {
        public IEventRepository EventRepository { get; set; }
        public ITicketRepository TicketRepository { get; set; }
        public Task SaveChangesAsync();
    }
}
