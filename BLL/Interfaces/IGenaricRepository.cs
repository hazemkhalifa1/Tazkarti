using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IGenaricRepository<T> where T : BaseEntity
    {
        public IEnumerable<T> GetAll();
        public T GetbyId(int id);

        public void Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
