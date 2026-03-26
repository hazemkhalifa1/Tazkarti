using BLL.Interfaces;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Repositories
{
    public class GenaricRepository<T> : IGenaricRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _dbContext;

        public GenaricRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }


        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbContext.Set<T>().Select(x => x).ToListAsync();

        public async Task<T?> GetbyIdAsync(Guid id) => await _dbContext.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> Search(string? SearchValue)
        => SearchValue is not null ? await _dbContext.Set<T>().Where(x => x.Id.ToString().Contains(SearchValue)).ToListAsync() : await GetAllAsync();


        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }
    }
}