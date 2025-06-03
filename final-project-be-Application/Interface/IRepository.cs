using final_project_be_Domain.DTOs.Comment;
using System.Linq.Expressions;

namespace final_project_be_Application.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<T?> GetByIdAsync(object id);
		IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(object id);
    }
}
