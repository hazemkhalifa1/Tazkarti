using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IGenaricRepository<T> where T : BaseEntity
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetbyIdAsync(int id);

        public Task AddAsync(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
