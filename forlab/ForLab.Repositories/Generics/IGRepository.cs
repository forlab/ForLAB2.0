using ForLab.Core.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ForLab.Repositories.Generics
{
    public interface IGRepository<T> where T: class
    {
        #region Find Methods
        T Find(params object[] keys);
        Task<T> FindAsync(params object[] keys);
        T Find(Func<T, bool> where);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        #endregion

        #region Add Methods
        object Add(T entity);
        Task AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
        #endregion

        #region Count Methods
        int Count();
        Task<int> CountAsync();
        #endregion

        #region Remove Methods
        EntityEntry<T> Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        #endregion

        #region Get Methods
        IQueryable<T> GetAll();
        Task<IQueryable<T>> GetAllAsync();
        IQueryable<T> GetAll(Expression<Func<T, bool>> where);
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> where);
        IQueryable<object> GetAll(Expression<Func<T, bool>> where, Expression<Func<T, object>> select);
        Task<IQueryable<object>> GetAllAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> select);
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includedProperties);
        Task<IQueryable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includedProperties);
        T GetFirst();
        Task<T> GetFirstAsync();
        T GetFirstOrDefault();
        Task<T> GetFirstOrDefaultAsync();
        T GetLast();
        Task<T> GetLastAsync();
        T GetLastOrDefault();
        Task<T> GetLastOrDefaultAsync();
        T GetFirst(Expression<Func<T, bool>> where);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> where);
        T GetFirstOrDefault(Expression<Func<T, bool>> where);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> where);
        T GetLast(Expression<Func<T, bool>> where);
        Task<T> GetLastAsync(Expression<Func<T, bool>> where);
        T GetLastOrDefault(Expression<Func<T, bool>> where);
        Task<T> GetLastOrDefaultAsync(Expression<Func<T, bool>> where);
        #endregion

        #region Update Method
        EntityEntry<T> Update(T entity);
        #endregion

        #region Pagination Methods
        PaginatedList<T> Paginate<TKey>(
           int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        PaginatedList<T> PaginateDescending<TKey>(
            int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector);

        PaginatedList<T> PaginateDescending<TKey>(
            int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        #endregion

        #region Release Unmanaged Resources
        void Dispose(bool disposing);
        #endregion

        #region Minimum Methods
        T GetMin();
        Task<T> GetMinAsync();
        object GetMin(Expression<Func<T, object>> selector);
        Task<object> GetMinAsync(Expression<Func<T, object>> selector);
        #endregion

        #region Maximum
        T GetMax();
        Task<T> GetMaxAsync();
        object GetMax(Expression<Func<T, object>> selector);
        Task<object> GetMaxAsync(Expression<Func<T, object>> selector);
        #endregion
    }
}
