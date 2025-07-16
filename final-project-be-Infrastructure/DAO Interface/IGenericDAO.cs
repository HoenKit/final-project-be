using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IGenericDAO<T> where T : class
    {
        IQueryable<T> GetAll();

        Task<T?> GetByIdAsync(object id);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task SaveChangesAsync();

        Task UpdateAsync(T entity);

        Task DeleteAsync(object id);

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
