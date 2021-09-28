using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Laboratory.LaboratoryWorkingDay;
using ForLab.Repositories.Laboratory.LaboratoryWorkingDay;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryWorkingDay
{
    public class LaboratoryWorkingDayService : GService<LaboratoryWorkingDayDto, Data.DbModels.LaboratorySchema.LaboratoryWorkingDay, ILaboratoryWorkingDayRepository>, ILaboratoryWorkingDayService
    {
        private readonly ILaboratoryWorkingDayRepository _laboratoryWorkingDayRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryWorkingDayDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryWorkingDayService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryWorkingDayRepository laboratoryWorkingDayRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportLaboratoryWorkingDayDto> fileService,
            IGeneralService generalService) : base(laboratoryWorkingDayRepository, mapper)
        {
            _laboratoryWorkingDayRepository = laboratoryWorkingDayRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryWorkingDayFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay> query = null;
            try
            {
                query = _laboratoryWorkingDayRepository.GetAll()
                                    .Include(x => x.Laboratory).ThenInclude(x => x.Region).ThenInclude(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }

                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.FromTime != null)
                    {
                        query = query.Where(x => x.FromTime == filterDto.FromTime);
                    }
                    if (filterDto.ToTime != null)
                    {
                        query = query.Where(x => x.ToTime == filterDto.ToTime);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Day))
                    {
                        query = query.Where(x => x.Day.Trim().ToLower().Contains(filterDto.Day.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "LaboratoryName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "FormatedFromTime".ToLower())
                    {
                        filterDto.SortProperty = "FromTime";
                    }
                    else if (filterDto.SortProperty.ToLower() == "FormatedToTime".ToLower())
                    {
                        filterDto.SortProperty = "ToTime";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<LaboratoryWorkingDayDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryWorkingDayFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryWorkingDayRepository.GetAll(x => !x.IsDeleted && x.IsActive);


                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }

                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.FromTime != null)
                    {
                        query = query.Where(x => x.FromTime == filterDto.FromTime);
                    }
                    if (filterDto.ToTime != null)
                    {
                        query = query.Where(x => x.ToTime == filterDto.ToTime);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Day))
                    {
                        query = query.Where(x => x.Day.Trim().ToLower().Contains(filterDto.Day.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.LaboratorySchema.LaboratoryWorkingDay()
                { 
                    Id = i.Id, 
                    Laboratory = i.Laboratory
                });
                query = query.OrderBy(x => x.LaboratoryId);
                var dataList = _mapper.Map<List<LaboratoryWorkingDayDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryWorkingDayDetails(int laboratoryWorkingDayId)
        {
            try
            {
                var laboratoryWorkingDay = await _laboratoryWorkingDayRepository.GetAll()
                                        .Include(x => x.Laboratory)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryWorkingDayId);
                if (laboratoryWorkingDay == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryWorkingDayDto = _mapper.Map<LaboratoryWorkingDayDto>(laboratoryWorkingDay);

                _response.Data = laboratoryWorkingDayDto;
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
        public async Task<IResponseDTO> CreateLaboratoryWorkingDay(LaboratoryWorkingDayDto laboratoryWorkingDayDto)
        {
            try
            {
                var laboratoryWorkingDay = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay>(laboratoryWorkingDayDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryWorkingDay.Laboratory = null;

                // Add to the DB
                await _laboratoryWorkingDayRepository.AddAsync(laboratoryWorkingDay);

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
        public async Task<IResponseDTO> UpdateLaboratoryWorkingDay(LaboratoryWorkingDayDto laboratoryWorkingDayDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryWorkingDayExist = await _laboratoryWorkingDayRepository.GetFirstAsync(x => x.Id == laboratoryWorkingDayDto.Id);
                if (laboratoryWorkingDayExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryWorkingDayExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryWorkingDay = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay>(laboratoryWorkingDayDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryWorkingDay.Laboratory = null;

                _laboratoryWorkingDayRepository.Update(laboratoryWorkingDay);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryWorkingDayId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryWorkingDay = await _laboratoryWorkingDayRepository.GetFirstAsync(x => x.Id == laboratoryWorkingDayId);
                if (laboratoryWorkingDay == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryWorkingDay.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryWorkingDay.IsActive = IsActive;
                laboratoryWorkingDay.UpdatedBy = LoggedInUserId;
                laboratoryWorkingDay.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryWorkingDayRepository.Update(laboratoryWorkingDay);

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
        public async Task<IResponseDTO> RemoveLaboratoryWorkingDay(int laboratoryWorkingDayId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryWorkingDay = await _laboratoryWorkingDayRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryWorkingDayId);
                if (laboratoryWorkingDay == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryWorkingDay.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryWorkingDay.IsDeleted = true;
                laboratoryWorkingDay.UpdatedBy = LoggedInUserId;
                laboratoryWorkingDay.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryWorkingDayRepository.Update(laboratoryWorkingDay);

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
        public GeneratedFile ExportLaboratoryWorkingDays(int? pageIndex = null, int? pageSize = null, LaboratoryWorkingDayFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay> query = null;
            try
            {
                query = _laboratoryWorkingDayRepository.GetAll()
                                    .Include(x => x.Laboratory).ThenInclude(x => x.Region).ThenInclude(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => createdBy.Contains(x.CreatedBy));
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }

                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.FromTime != null)
                    {
                        query = query.Where(x => x.FromTime == filterDto.FromTime);
                    }
                    if (filterDto.ToTime != null)
                    {
                        query = query.Where(x => x.ToTime == filterDto.ToTime);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Day))
                    {
                        query = query.Where(x => x.Day.Trim().ToLower().Contains(filterDto.Day.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.LaboratoryId);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "LaboratoryName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "FormatedFromTime".ToLower())
                    {
                        filterDto.SortProperty = "FromTime";
                    }
                    else if (filterDto.SortProperty.ToLower() == "FormatedToTime".ToLower())
                    {
                        filterDto.SortProperty = "ToTime";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportLaboratoryWorkingDayDto>>(query.ToList());
                return _fileService.ExportToExcel(dataList);

            }

            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return null;
        }
        // Validators methods
        public bool IsDayUnique(LaboratoryWorkingDayDto laboratoryWorkingDayDto)
        {
            var searchResult = _laboratoryWorkingDayRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryWorkingDayDto.Id
                                                && x.LaboratoryId == laboratoryWorkingDayDto.LaboratoryId
                                                && x.FromTime == laboratoryWorkingDayDto.FromTime
                                                && x.ToTime == laboratoryWorkingDayDto.ToTime
                                                && x.Day.ToLower().Trim() == laboratoryWorkingDayDto.Day.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryWorkingDayId)
        {
            try
            {
                var laboratoryWorkingDay = await _laboratoryWorkingDayRepository.GetAll()
                                        .Include(x => x.Laboratory)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryWorkingDayId);
                if (laboratoryWorkingDay == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
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
    }
}
