using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);

        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> GetAllByFilter(Expression<Func<T, bool>> expression);
        Task Add(T entity, int userId);
        Task Update(T entity, int userId);
        Task Delete(T entity, int userId);
    }
}
