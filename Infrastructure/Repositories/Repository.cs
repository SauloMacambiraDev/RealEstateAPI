using Infrastructure.Data;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Models.Interfaces;

namespace Infrastructure.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly RealEStateDbContext dbContext;
        protected readonly DbSet<T> dbSet;

        public Repository(RealEStateDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = this.dbContext.Set<T>();
        }

        public async Task Add(T entity, int userId)
        {
            dbSet.Add(entity);
            await dbContext.SaveChangesAsync(userId);
        }

        public async Task Delete(T entity, int userId)
        {
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync(userId);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByFilter(Expression<Func<T, bool>> expression)
        {
            return await dbSet.Where(expression).ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await dbSet.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task Update(T entity, int userId)
        {
            dbSet.Update(entity);
            await dbContext.SaveChangesAsync(userId);
        }
    }
}
