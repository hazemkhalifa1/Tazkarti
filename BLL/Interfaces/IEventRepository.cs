using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IEventRepository : IGenaricRepository<Event>
    {
        public void Add(Event e);
    }
}
