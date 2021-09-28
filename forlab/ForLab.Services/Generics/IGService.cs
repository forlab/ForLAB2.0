using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;

namespace ForLab.Services.Generics
{
    public interface IGService<D, T, R>
    {
        #region Add Method
        IResponseDTO Add(D dtoModel);
        Task<IResponseDTO> AddAsync(D dtoObject);
        IResponseDTO AddRange(IEnumerable<D> dtoObjects);
        Task<IResponseDTO> AddRangeAsync(IEnumerable<D> dtoObjects);
        #endregion

        #region Delete Method
        IResponseDTO Remove(D entity);
        IResponseDTO RemoveRange(IEnumerable<D> dtoObjects);
        #endregion

        #region Update Method
        IResponseDTO Update(D entity);

        #endregion

        #region Get Methods
        IResponseDTO GetAll();
        Task<IResponseDTO> GetAllAsync();
        IResponseDTO GetAll(Expression<Func<T, bool>> where);
        Task<IResponseDTO> GetAllAsync(Expression<Func<T, bool>> where);
        IResponseDTO GetAll(Expression<Func<T, bool>> where, Expression<Func<T, object>> select);
        Task<IResponseDTO> GetAllAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> select);
        IResponseDTO GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<IResponseDTO> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
        IResponseDTO GetFirstOrDefault();
        Task<IResponseDTO> GetFirstOrDefaultAsync();
        IResponseDTO GetFirst();
        Task<IResponseDTO> GetFirstAsync();
        IResponseDTO GetLastOrDefault();
        Task<IResponseDTO> GetLastOrDefaultAsync();
        IResponseDTO GetLast();
        Task<IResponseDTO> GetLastAsync();
        IResponseDTO GetFirstOrDefault(Expression<Func<T, bool>> where);
        Task<IResponseDTO> GetFirstOrDefaultAsync(Expression<Func<T, bool>> where);
        IResponseDTO GetFirst(Expression<Func<T, bool>> where);
        Task<IResponseDTO> GetFirstAsync(Expression<Func<T, bool>> where);
        IResponseDTO GetLastOrDefault(Expression<Func<T, bool>> where);
        Task<IResponseDTO> GetLastOrDefaultAsync(Expression<Func<T, bool>> where);
        IResponseDTO GetLast(Expression<Func<T, bool>> where);
        Task<IResponseDTO> GetLastAsync(Expression<Func<T, bool>> where);
        #endregion

        #region Pagination Methods


        IResponseDTO Paginate<TKey>(
            int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        IResponseDTO PaginateDescending<TKey>(
            int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector);

        IResponseDTO PaginateDescending<TKey>(
            int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        #endregion

        #region Count Methods
        IResponseDTO Count();
        Task<IResponseDTO> CountAsync();
        #endregion

        #region Find Methods
        IResponseDTO Find(params object[] keys);
        Task<IResponseDTO> FindAsync(params object[] keys);
        IResponseDTO Find(Func<T, bool> where);
        Task<IResponseDTO> FindAsync(Expression<Func<T, bool>> match);
        #endregion

        #region Aggregation Methods
        IResponseDTO GetMin();
        Task<IResponseDTO> GetMinAsync();
        IResponseDTO GetMin(Expression<Func<T, object>> selector);
        Task<IResponseDTO> GetMinAsync(Expression<Func<T, object>> selector);


        IResponseDTO GetMax();
        Task<IResponseDTO> GetMaxAsync();
        IResponseDTO GetMax(Expression<Func<T, object>> selector);
        Task<IResponseDTO> GetMaxAsync(Expression<Func<T, object>> selector);
        #endregion
    }
}
