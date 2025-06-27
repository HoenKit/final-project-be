using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace final_project_be_Infrastructure.DAO
{
	public class GenericDAO<T> where T : class
	{
		private readonly ApplicationDbContext _context;
		private readonly DbSet<T> _dbSet;
		private IDbContextTransaction? _transaction;

		public GenericDAO(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public IQueryable<T> GetAll()
		{
			return _dbSet.AsQueryable();
		}

		public async Task<T?> GetByIdAsync(object id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(T entity)
		{
			_dbSet.Attach(entity);
			_context.Entry(entity).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(object id)
		{
			var entity = await GetByIdAsync(id);
			if (entity != null)
			{
				_dbSet.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}

        public virtual async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public virtual async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }
    }
}
