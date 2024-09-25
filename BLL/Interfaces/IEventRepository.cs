using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IEventRepository : IGenaricRepository<Event>
    {
        public Task UpdateAsync(Event ev);
        public Task AddTikAsync(Event e, int num);
    }
}
