using final_project_be_Domain.DTOs.Comment;
using System.Linq.Expressions;

namespace final_project_be_Application.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(object id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(object id);
    }
}
