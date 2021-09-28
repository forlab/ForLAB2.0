using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Laboratory.LaboratoryPatientStatistic;
using ForLab.Repositories.Laboratory.LaboratoryPatientStatistic;
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

namespace ForLab.Services.Laboratory.LaboratoryPatientStatistic
{
    public class LaboratoryPatientStatisticService : GService<LaboratoryPatientStatisticDto, Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic, ILaboratoryPatientStatisticRepository>, ILaboratoryPatientStatisticService
    {
        private readonly ILaboratoryPatientStatisticRepository _laboratoryPatientStatisticRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryPatientStatisticDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryPatientStatisticService(IMapper mapper,
            IResponseDTO response,
            IFileService<ExportLaboratoryPatientStatisticDto> fileService,
            ILaboratoryPatientStatisticRepository laboratoryPatientStatisticRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IGeneralService generalService) : base(laboratoryPatientStatisticRepository, mapper)
        {
            _laboratoryPatientStatisticRepository = laboratoryPatientStatisticRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryPatientStatisticFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic> query = null;
            try
            {
                query = _laboratoryPatientStatisticRepository.GetAll()
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
                    if (filterDto.Count > 0)
                    {
                        query = query.Where(x => x.Count == filterDto.Count);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.Period != null)
                    {
                        query = query.Where(x => x.Period.Date == filterDto.Period.Value.Date);
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
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<LaboratoryPatientStatisticDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryPatientStatisticFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryPatientStatisticRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                    if (filterDto.Count > 0)
                    {
                        query = query.Where(x => x.Count == filterDto.Count);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.Period != null)
                    {
                        query = query.Where(x => x.Period.Date == filterDto.Period.Value.Date);
                    }
                }

                query = query.Select(i => new Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic() 
                { 
                    Id = i.Id,
                    Laboratory = i.Laboratory
                });
                query = query.OrderBy(x => x.Count);
                var dataList = _mapper.Map<List<LaboratoryPatientStatisticDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryPatientStatisticDetails(int laboratoryPatientStatisticId)
        {
            try
            {
                var laboratoryPatientStatistic = await _laboratoryPatientStatisticRepository.GetAll()
                                        .Include(x => x.Laboratory)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryPatientStatisticId);
                if (laboratoryPatientStatistic == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryPatientStatisticDto = _mapper.Map<LaboratoryPatientStatisticDto>(laboratoryPatientStatistic);

                _response.Data = laboratoryPatientStatisticDto;
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
        public async Task<IResponseDTO> CreateLaboratoryPatientStatistic(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto)
        {
            try
            {
                var laboratoryPatientStatistic = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic>(laboratoryPatientStatisticDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryPatientStatistic.Laboratory = null;

                // Add to the DB
                await _laboratoryPatientStatisticRepository.AddAsync(laboratoryPatientStatistic);

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
        public async Task<IResponseDTO> UpdateLaboratoryPatientStatistic(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryPatientStatisticExist = await _laboratoryPatientStatisticRepository.GetFirstAsync(x => x.Id == laboratoryPatientStatisticDto.Id);
                if (laboratoryPatientStatisticExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryPatientStatisticExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryPatientStatistic = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic>(laboratoryPatientStatisticDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryPatientStatistic.Laboratory = null;

                _laboratoryPatientStatisticRepository.Update(laboratoryPatientStatistic);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryPatientStatisticId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryPatientStatistic = await _laboratoryPatientStatisticRepository.GetFirstAsync(x => x.Id == laboratoryPatientStatisticId);
                if (laboratoryPatientStatistic == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryPatientStatistic.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryPatientStatistic.IsActive = IsActive;
                laboratoryPatientStatistic.UpdatedBy = LoggedInUserId;
                laboratoryPatientStatistic.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryPatientStatisticRepository.Update(laboratoryPatientStatistic);

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
        public async Task<IResponseDTO> RemoveLaboratoryPatientStatistic(int laboratoryPatientStatisticId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryPatientStatistic = await _laboratoryPatientStatisticRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryPatientStatisticId);
                if (laboratoryPatientStatistic == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryPatientStatistic.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryPatientStatistic.IsDeleted = true;
                laboratoryPatientStatistic.UpdatedBy = LoggedInUserId;
                laboratoryPatientStatistic.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryPatientStatisticRepository.Update(laboratoryPatientStatistic);

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
        public GeneratedFile ExportLaboratoryPatientStatistics(int? pageIndex = null, int? pageSize = null, LaboratoryPatientStatisticFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic> query = null;
            try
            {
                query = _laboratoryPatientStatisticRepository.GetAll()
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
                    if (filterDto.Count > 0)
                    {
                        query = query.Where(x => x.Count == filterDto.Count);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.Period != null)
                    {
                        query = query.Where(x => x.Period.Date == filterDto.Period.Value.Date);
                    }
                }
                query = query.OrderByDescending(x => x.Count);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "LaboratoryName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportLaboratoryPatientStatisticDto>>(query.ToList());
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
        public async Task<IResponseDTO> ImportLaboratoryPatientStatistics(List<LaboratoryPatientStatisticDto> laboratoryPatientStatisticDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryPatientStatistics_database = _laboratoryPatientStatisticRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryPatientStatistics = _mapper.Map<List<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic>>(laboratoryPatientStatisticDtos);

                // vars
                var newLaboratoryPatientStatistics = new List<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic>();
                var updatedLaboratoryPatientStatistics = new List<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic>();

                // Get new and updated laboratoryPatientStatistics
                foreach (var item in laboratoryPatientStatistics)
                {
                    var foundLaboratoryPatientStatistic = laboratoryPatientStatistics_database.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId
                                                                                            && x.Period.Date == item.Period.Date);
                    if (foundLaboratoryPatientStatistic == null)
                    {
                        newLaboratoryPatientStatistics.Add(item);
                    }
                    else
                    {
                        updatedLaboratoryPatientStatistics.Add(item);
                    }
                }

                // Add the new object to the database
                if (newLaboratoryPatientStatistics.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newLaboratoryPatientStatistics.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Laboratory = null;
                        return x;
                    }).ToList();
                    await _laboratoryPatientStatisticRepository.AddRangeAsync(newLaboratoryPatientStatistics);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryPatientStatistics.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryPatientStatistics)
                    {
                        var fromDatabase = laboratoryPatientStatistics_database.FirstOrDefault(x => x.Period.Date == item.Period.Date 
                                                                                                && x.LaboratoryId == item.LaboratoryId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Period = item.Period;
                        fromDatabase.LaboratoryId = item.LaboratoryId;
                        fromDatabase.Count = item.Count;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        fromDatabase.Laboratory = null;
                        _laboratoryPatientStatisticRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryPatientStatistics.Count();
                var numberOfUpdated = updatedLaboratoryPatientStatistics.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = laboratoryPatientStatisticDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated
                };
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
        public bool IsPatientStatisticUnique(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto)
        {
            var searchResult = _laboratoryPatientStatisticRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryPatientStatisticDto.Id
                                                && x.LaboratoryId == laboratoryPatientStatisticDto.LaboratoryId
                                                && x.Period.Date == laboratoryPatientStatisticDto.Period.Date);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryPatientStatisticId)
        {
            try
            {
                var laboratoryPatientStatistic = await _laboratoryPatientStatisticRepository.GetAll()
                                        .Include(x => x.Laboratory)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryPatientStatisticId);
                if (laboratoryPatientStatistic == null)
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
