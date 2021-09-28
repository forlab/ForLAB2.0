using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.DTO.Common;
using ForLab.Repositories.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ForLab.Services.Generics
{
    public class GService<D, T, R> : IGService<D, T, R>
       where R : IGRepository<T>
       where T : class
    {      
        #region Private Fields
        protected readonly R _genericRepository;
        protected readonly IMapper _mapper;
        #endregion


        #region Constructor
        public GService(R genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }
        #endregion

        #region Add Method
        public virtual IResponseDTO Add(D dtoObject)
        {
            IResponseDTO IResponseDTO = new ResponseDTO();
            try
            {
                if (dtoObject is IAuditable)
                {
                    ((IAuditable)dtoObject).CreatedOn = DateTime.Now;
                }
                T entityObj = _mapper.Map<T>(dtoObject);

                object addedModel = _genericRepository.Add(entityObj);

                IResponseDTO.Data = _mapper.Map<D>(addedModel);
                IResponseDTO.IsPassed = true;
                IResponseDTO.Message = "Ok";
            }
            catch (Exception ex)
            {
                IResponseDTO.IsPassed = false;
                IResponseDTO.Message = "Internal Error" + ex.Message;
                return IResponseDTO;
            }
            return IResponseDTO;
        }
        public virtual async Task<IResponseDTO> AddAsync(D dtoObject)
        {
            IResponseDTO IResponseDTO = new ResponseDTO();
            try
            {
                if (dtoObject is IAuditable)
                {
                    ((IAuditable)dtoObject).CreatedOn = DateTime.Now;
                }
                T entityObj = _mapper.Map<T>(dtoObject);
                await _genericRepository.AddAsync(entityObj);
                IResponseDTO.IsPassed = true;
                IResponseDTO.Message = "Ok";
            }
            catch (Exception ex)
            {
                IResponseDTO.IsPassed = false;
                IResponseDTO.Message = "Internal Error" + ex.Message;
                return IResponseDTO;
            }
            return IResponseDTO;
        }
        public virtual IResponseDTO AddRange(IEnumerable<D> dtoObjects)
        {
            IResponseDTO IResponseDTO = new ResponseDTO();
            try
            {
                if (dtoObjects != null && dtoObjects.Count() > 0)
                {
                    foreach (D dtoObject in dtoObjects)
                    {
                        if (dtoObject is IAuditable)
                            ((IAuditable)dtoObject).CreatedOn = DateTime.Now;
                    }
                }
                ICollection<T> models = _mapper.Map<ICollection<T>>(dtoObjects);
                _genericRepository.AddRange(models);
                IResponseDTO.IsPassed = true;
                IResponseDTO.Message = "Ok";
            }
            catch (Exception ex)
            {
                IResponseDTO.IsPassed = false;
                IResponseDTO.Message = "Internal Error" + ex.Message;
            }
            return IResponseDTO;
        }
        public virtual async Task<IResponseDTO> AddRangeAsync(IEnumerable<D> dtoObjects)
        {
            IResponseDTO IResponseDTO = new ResponseDTO();
            try
            {
                if (dtoObjects != null && dtoObjects.Count() > 0)
                {
                    foreach (D dtoObject in dtoObjects)
                    {
                        if (dtoObject is IAuditable)
                            ((IAuditable)dtoObject).CreatedOn = DateTime.Now;
                    }
                }
                ICollection<T> models = _mapper.Map<ICollection<T>>(dtoObjects);
                await _genericRepository.AddRangeAsync(models);
                IResponseDTO.IsPassed = true;
                IResponseDTO.Message = "Ok";
            }
            catch (Exception ex)
            {
                IResponseDTO.IsPassed = false;
                IResponseDTO.Message = "Internal Error" + ex.Message;
                return IResponseDTO;
            }
            return IResponseDTO;
        }
        #endregion

        #region Count Methods
        public virtual IResponseDTO Count()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.Count());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> CountAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<Task<D>>(await _genericRepository.CountAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        #endregion

        #region Remove Method
        public virtual IResponseDTO Remove(D dtoObject)
        {
            IResponseDTO IResponseDTO = new ResponseDTO();
            try
            {
                if (dtoObject is IAuditable)
                {
                    ((IAuditable)dtoObject).UpdatedOn = DateTime.Now;
                }
                T entityObject = _mapper.Map<T>(dtoObject);

                ((IBaseEntity)entityObject).IsDeleted = true;
                IResponseDTO.Data = _genericRepository.Update(entityObject);


                IResponseDTO.IsPassed = true;
                IResponseDTO.Message = "Ok";
            }
            catch (Exception ex)
            {


                IResponseDTO.IsPassed = false;
                IResponseDTO.Message = "Internal Error" + ex.Message;
                return IResponseDTO;
            }
            return IResponseDTO;
        }
        public virtual IResponseDTO RemoveRange(IEnumerable<D> dtoObjects)
        {
            IResponseDTO IResponseDTO = new ResponseDTO();
            try
            {
                IEnumerable<T> entityObjects = _mapper.Map<IEnumerable<T>>(dtoObjects);

                if (entityObjects != null && entityObjects.Count() > 0)
                {
                    if (entityObjects.First() is ISoftDelete)
                    {
                        foreach (T entityObject in entityObjects)
                        {
                            ((ISoftDelete)entityObject).IsDeleted = true;
                            if (entityObject is IAuditable)
                            {
                                ((IAuditable)entityObject).UpdatedOn = DateTime.Now;
                            }
                            IResponseDTO.Data = _genericRepository.Update(entityObject);
                        }
                        IResponseDTO.IsPassed = true;
                        IResponseDTO.Message = "Ok";
                    }
                    else
                    {
                        _genericRepository.RemoveRange(entityObjects);
                        IResponseDTO.IsPassed = true;
                        IResponseDTO.Message = "Ok";
                    }
                }
            }
            catch (Exception ex)
            {
                IResponseDTO.IsPassed = false;
                IResponseDTO.Message = "Internal Error" + ex.Message;
                return IResponseDTO;
            }
            IResponseDTO.IsPassed = true;
            IResponseDTO.Message = "Ok";
            return IResponseDTO;
        }
        #endregion

        #region Find Methods
        public virtual IResponseDTO Find(params object[] keys)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.Find(keys));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> FindAsync(params object[] keys)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<Task<D>>(await _genericRepository.FindAsync(keys));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual IResponseDTO Find(Func<T, bool> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.Find(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> FindAsync(Expression<Func<T, bool>> match)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<Task<D>>(await _genericRepository.GetFirstAsync(match));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        #endregion

        #region Get Methods
        public virtual IResponseDTO GetAll(Expression<Func<T, bool>> whereCondition)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(_genericRepository.GetAll(whereCondition));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual IResponseDTO GetAll()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(_genericRepository.GetAll());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual IResponseDTO GetAll(Expression<Func<T, bool>> where, Expression<Func<T, object>> select)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(_genericRepository.GetAll(where, select));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> GetAllAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(await _genericRepository.GetAllAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> GetAllAsync(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(await _genericRepository.GetAllAsync(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> GetAllAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> select)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(await _genericRepository.GetAllAsync(where, select));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual IResponseDTO GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(_genericRepository.GetAllIncluding(includeProperties));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual async Task<IResponseDTO> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IQueryable<D>>(await _genericRepository.GetAllIncludingAsync(includeProperties));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual IResponseDTO GetFirstOrDefault()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetFirstOrDefault());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetFirst()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetFirst());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        public virtual IResponseDTO GetFirstOrDefault(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetFirstOrDefault(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetFirst(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetFirst(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetFirstOrDefaultAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetFirstOrDefaultAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetFirstAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetFirstAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetFirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetFirstOrDefaultAsync(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetFirstAsync(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetFirstAsync(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetLastOrDefault()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetLastOrDefault());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetLast()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetLast());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetLastOrDefault(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetLastOrDefault(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetLast(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetLast(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetLastOrDefaultAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetLastOrDefaultAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetLastAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetLastAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetLastOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetLastOrDefaultAsync(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetLastAsync(Expression<Func<T, bool>> where)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetLastAsync(where));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        #endregion

        #region Pagination Methods
        public virtual IResponseDTO Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IEnumerable<D>>(_genericRepository.Paginate(pageIndex, pageSize, keySelector, predicate));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO PaginateDescending<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IEnumerable<D>>(_genericRepository.PaginateDescending(pageIndex, pageSize, keySelector));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO PaginateDescending<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<IEnumerable<D>>(_genericRepository.PaginateDescending(pageIndex, pageSize, keySelector, predicate, includeProperties));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        #endregion

        #region Update Method
        public virtual IResponseDTO Update(D dtoObject)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                T entityToBeUpdated = _mapper.Map<T>(dtoObject);
                response.Data = _mapper.Map<D>(_genericRepository.Update(entityToBeUpdated));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        #endregion

        #region GetMinimum Methods
        public virtual IResponseDTO GetMin()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetMin());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetMinAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetMinAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetMin(Expression<Func<T, object>> selector)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetMin(selector));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetMinAsync(Expression<Func<T, object>> selector)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetMinAsync(selector));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }
        #endregion

        #region GetMaximum Methods
        public virtual IResponseDTO GetMax()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetMax());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetMaxAsync()
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetMaxAsync());
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual IResponseDTO GetMax(Expression<Func<T, object>> selector)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(_genericRepository.GetMax(selector));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        public virtual async Task<IResponseDTO> GetMaxAsync(Expression<Func<T, object>> selector)
        {
            IResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _mapper.Map<D>(await _genericRepository.GetMaxAsync(selector));
                response.IsPassed = true;
                response.Message = "Ok";
            }
            catch (Exception ex)
            {
                response.IsPassed = false;
                response.Message = "Internal Error" + ex.Message;
            }
            return response;
        }

        #endregion

    }
}
