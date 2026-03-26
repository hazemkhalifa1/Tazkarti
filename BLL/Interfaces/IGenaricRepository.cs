using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IGenaricRepository<T> where T : BaseEntity
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetbyIdAsync(Guid id);

        public Task<IEnumerable<T>> Search(string? SearchValue);

        public Task AddAsync(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
