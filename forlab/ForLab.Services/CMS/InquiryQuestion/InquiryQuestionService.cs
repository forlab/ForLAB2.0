using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.CMS.InquiryQuestion;
using ForLab.DTO.CMS.InquiryQuestionReply;
using ForLab.Repositories.CMS.InquiryQuestion;
using ForLab.Repositories.CMS.InquiryQuestionReply;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.SendEmail;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.InquiryQuestion
{
    public class InquiryQuestionService : GService<InquiryQuestionDto, Data.DbModels.CMSSchema.InquiryQuestion, IInquiryQuestionRepository>, IInquiryQuestionService
    {
        private readonly IInquiryQuestionRepository _inquiryQuestionRepository;
        private readonly IInquiryQuestionReplyRepository _inquiryQuestionReplyRepository;
        private readonly IEmailService _emailService;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public InquiryQuestionService(IMapper mapper,
            IResponseDTO response,
            IInquiryQuestionRepository inquiryQuestionRepository,
            IInquiryQuestionReplyRepository inquiryQuestionReplyRepository,
            IEmailService emailService,
            IUnitOfWork<AppDbContext> unitOfWork) : base(inquiryQuestionRepository, mapper)
        {
            _inquiryQuestionRepository = inquiryQuestionRepository;
            _inquiryQuestionReplyRepository = inquiryQuestionReplyRepository;
            _emailService = emailService;
            _response = response;
            _unitOfWork = unitOfWork;

        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, InquiryQuestionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.CMSSchema.InquiryQuestion> query = null;
            try
            {
                query = _inquiryQuestionRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Message))
                    {
                        query = query.Where(x => x.Message.Trim().ToLower().Contains(filterDto.Message.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Email))
                    {
                        query = query.Where(x => x.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                    }
                    if (filterDto.ReplyProvided != null)
                    {
                        query = query.Where(x => x.ReplyProvided == filterDto.ReplyProvided);
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

                var dataList = _mapper.Map<List<InquiryQuestionDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(InquiryQuestionFilterDto filterDto = null)
        {
            try
            {
                var query = _inquiryQuestionRepository.GetAll(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Email))
                    {
                        query = query.Where(x => x.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Message))
                    {
                        query = query.Where(x => x.Message.Trim().ToLower().Contains(filterDto.Message.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.CMSSchema.InquiryQuestion() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<InquiryQuestionDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetInquiryQuestionDetails(int inquiryQuestionId)
        {
            try
            {
                var inquiryQuestion = await _inquiryQuestionRepository.GetAll()
                                        .Include(x => x.InquiryQuestionReplies)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == inquiryQuestionId);
                if (inquiryQuestion == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                inquiryQuestion.InquiryQuestionReplies = inquiryQuestion.InquiryQuestionReplies.Where(x => !x.IsDeleted).ToList();
                var inquiryQuestionDto = _mapper.Map<InquiryQuestionDto>(inquiryQuestion);

                _response.Data = inquiryQuestionDto;
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
        public async Task<IResponseDTO> CreateInquiryQuestion(InquiryQuestionDto inquiryQuestionDto)
        {
            try
            {
                var inquiryQuestion = _mapper.Map<Data.DbModels.CMSSchema.InquiryQuestion>(inquiryQuestionDto);

                // Add to the DB
                await _inquiryQuestionRepository.AddAsync(inquiryQuestion);

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
        public async Task<IResponseDTO> UpdateInquiryQuestion(InquiryQuestionDto inquiryQuestionDto)
        {
            try
            {
                var inquiryQuestionExist = await _inquiryQuestionRepository.GetFirstAsync(x => x.Id == inquiryQuestionDto.Id);
                if (inquiryQuestionExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var inquiryQuestion = _mapper.Map<Data.DbModels.CMSSchema.InquiryQuestion>(inquiryQuestionDto);

                _inquiryQuestionRepository.Update(inquiryQuestion);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int inquiryQuestionId, bool IsActive)
        {
            try
            {
                var inquiryQuestion = await _inquiryQuestionRepository.GetFirstAsync(x => x.Id == inquiryQuestionId);
                if (inquiryQuestion == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                inquiryQuestion.IsActive = IsActive;
                inquiryQuestion.UpdatedBy = loggedInUserId;
                inquiryQuestion.UpdatedOn = DateTime.Now;

                // Update on the Database
                _inquiryQuestionRepository.Update(inquiryQuestion);

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
        public async Task<IResponseDTO> RemoveInquiryQuestion(int inquiryQuestionId, int loggedInUserId)
        {
            try
            {
                var inquiryQuestion = await _inquiryQuestionRepository.GetFirstOrDefaultAsync(x => x.Id == inquiryQuestionId);
                if (inquiryQuestion == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                inquiryQuestion.IsDeleted = true;
                inquiryQuestion.UpdatedBy = loggedInUserId;
                inquiryQuestion.UpdatedOn = DateTime.Now;

                // Update on the Database
                _inquiryQuestionRepository.Update(inquiryQuestion);

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
        public async Task<IResponseDTO> CreateInquiryQuestionReply(InquiryQuestionReplyDto inquiryQuestionReplyDto)
        {
            try
            {
                var inquiryQuestion = await _inquiryQuestionRepository.GetFirstAsync(x => x.Id == inquiryQuestionReplyDto.InquiryQuestionId);
                var inquiryQuestionReply = _mapper.Map<Data.DbModels.CMSSchema.InquiryQuestionReply>(inquiryQuestionReplyDto);

                inquiryQuestionReply.InquiryQuestion = null;

                // Add to the DB
                inquiryQuestion.UpdatedBy = inquiryQuestionReplyDto.CreatedBy;
                inquiryQuestion.UpdatedOn = inquiryQuestionReplyDto.CreatedOn;
                inquiryQuestion.ReplyProvided = true;
                inquiryQuestion.InquiryQuestionReplies = new List<Data.DbModels.CMSSchema.InquiryQuestionReply>
                {
                    inquiryQuestionReply
                };
                _inquiryQuestionRepository.Update(inquiryQuestion);

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                // Send the email to the client (creator of the question)
                await _emailService.SendEmailReply(inquiryQuestion.Email, inquiryQuestionReplyDto.Message);

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
    }
}
