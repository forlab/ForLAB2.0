using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.FrequentlyAskedQuestion;
using ForLab.Repositories.CMS.FrequentlyAskedQuestion;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.FrequentlyAskedQuestion
{
    public class FrequentlyAskedQuestionService : GService<FrequentlyAskedQuestionDto, Data.DbModels.CMSSchema.FrequentlyAskedQuestion, IFrequentlyAskedQuestionRepository>, IFrequentlyAskedQuestionService
    {
        private readonly IFrequentlyAskedQuestionRepository _frequentlyAskedQuestionRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public FrequentlyAskedQuestionService(IMapper mapper,
            IResponseDTO response,
            IFrequentlyAskedQuestionRepository frequentlyAskedQuestionRepository,
            IUnitOfWork<AppDbContext> unitOfWork) : base(frequentlyAskedQuestionRepository, mapper)
        {
            _frequentlyAskedQuestionRepository = frequentlyAskedQuestionRepository;
            _response = response;
            _unitOfWork = unitOfWork;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, FrequentlyAskedQuestionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.CMSSchema.FrequentlyAskedQuestion> query = null;
            try
            {
                query = _frequentlyAskedQuestionRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Question))
                    {
                        query = query.Where(x => x.Question.Trim().ToLower().Contains(filterDto.Question.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Answer))
                    {
                        query = query.Where(x => x.Answer.Trim().ToLower().Contains(filterDto.Answer.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<FrequentlyAskedQuestionDto>>(query.ToList());

                _response.Data = new
                {
                    List = dataList,
                    Page = pageIndex ?? 0,
                    pageSize = pageSize ?? 0,
                    Total = total,
                    Pages = pageSize.HasValue && pageSize.Value > 0 ? total / pageSize : 1
                };

                _response.Message = "Ok";
                _response.IsPassed = true;

            }

            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public IResponseDTO GetAllAsDrp(FrequentlyAskedQuestionFilterDto filterDto = null)
        {
            try
            {
                var query = _frequentlyAskedQuestionRepository.GetAll(x => !x.IsDeleted && x.IsActive);


                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Question))
                    {
                        query = query.Where(x => x.Question.Trim().ToLower().Contains(filterDto.Question.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Answer))
                    {
                        query = query.Where(x => x.Answer.Trim().ToLower().Contains(filterDto.Answer.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.CMSSchema.FrequentlyAskedQuestion() { Id = i.Id, Question = i.Question });
                query = query.OrderBy(x => x.CreatedOn);
                var dataList = _mapper.Map<List<FrequentlyAskedQuestionDrp>>(query.ToList());

                _response.Data = dataList;
                _response.IsPassed = true;
                _response.Message = "Done";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> GetFrequentlyAskedQuestionDetails(int frequentlyAskedQuestionId)
        {
            try
            {
                var frequentlyAskedQuestion = await _frequentlyAskedQuestionRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == frequentlyAskedQuestionId);
                if (frequentlyAskedQuestion == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var frequentlyAskedQuestionDto = _mapper.Map<FrequentlyAskedQuestionDto>(frequentlyAskedQuestion);

                _response.Data = frequentlyAskedQuestionDto;
                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> CreateFrequentlyAskedQuestion(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto)
        {
            try
            {
                var frequentlyAskedQuestion = _mapper.Map<Data.DbModels.CMSSchema.FrequentlyAskedQuestion>(frequentlyAskedQuestionDto);

                // Add to the DB
                await _frequentlyAskedQuestionRepository.AddAsync(frequentlyAskedQuestion);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateFrequentlyAskedQuestion(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto)
        {
            try
            {
                var frequentlyAskedQuestionExist = await _frequentlyAskedQuestionRepository.GetFirstAsync(x => x.Id == frequentlyAskedQuestionDto.Id);
                if (frequentlyAskedQuestionExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var frequentlyAskedQuestion = _mapper.Map<Data.DbModels.CMSSchema.FrequentlyAskedQuestion>(frequentlyAskedQuestionDto);

                _frequentlyAskedQuestionRepository.Update(frequentlyAskedQuestion);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = null;
                _response.IsPassed = true;
                _response.Message = "Ok";

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int frequentlyAskedQuestionId, bool IsActive)
        {
            try
            {
                var frequentlyAskedQuestion = await _frequentlyAskedQuestionRepository.GetFirstAsync(x => x.Id == frequentlyAskedQuestionId);
                if (frequentlyAskedQuestion == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                frequentlyAskedQuestion.IsActive = IsActive;
                frequentlyAskedQuestion.UpdatedBy = loggedInUserId;
                frequentlyAskedQuestion.UpdatedOn = DateTime.Now;


                // Update on the Database
                _frequentlyAskedQuestionRepository.Update(frequentlyAskedQuestion);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }

            return _response;
        }
        public async Task<IResponseDTO> RemoveFrequentlyAskedQuestion(int frequentlyAskedQuestionId, int loggedInUserId)
        {
            try
            {
                var frequentlyAskedQuestion = await _frequentlyAskedQuestionRepository.GetFirstOrDefaultAsync(x => x.Id == frequentlyAskedQuestionId);
                if (frequentlyAskedQuestion == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                frequentlyAskedQuestion.IsDeleted = true;
                frequentlyAskedQuestion.UpdatedBy = loggedInUserId;
                frequentlyAskedQuestion.UpdatedOn = DateTime.Now;


                // Update on the Database
                _frequentlyAskedQuestionRepository.Update(frequentlyAskedQuestion);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.IsPassed = true;
                _response.Message = "Ok";
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return _response;
        }
        // Validators methods
        public bool IsQuestionUnique(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto)
        {
            var searchResult = _frequentlyAskedQuestionRepository.GetAll(x =>
                                              !x.IsDeleted
                                              && x.Id != frequentlyAskedQuestionDto.Id
                                              && x.Question.ToLower().Trim() == frequentlyAskedQuestionDto.Question.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
