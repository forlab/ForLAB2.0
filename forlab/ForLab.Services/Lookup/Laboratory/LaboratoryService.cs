using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.Laboratory;
using ForLab.Repositories.Lookup.Laboratory;
using ForLab.Repositories.Security.UserLaboratorySubscription;
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

namespace ForLab.Services.Lookup.Laboratory
{
    public class LaboratoryService : GService<LaboratoryDto, Data.DbModels.LookupSchema.Laboratory, ILaboratoryRepository>, ILaboratoryService
    {
        private readonly ILaboratoryRepository _laboratoryRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryRepository laboratoryRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportLaboratoryDto> fileService,
            IGeneralService generalService) : base(laboratoryRepository, mapper)
        {
            _laboratoryRepository = laboratoryRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.Laboratory> query = null;
            try
            {
                query = _laboratoryRepository.GetAll()
                                    .Include(x => x.Region)
                                    .Include(x => x.LaboratoryCategory)
                                    .Include(x => x.LaboratoryLevel)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
                    }
                    if (filterDto.LaboratoryCategoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryCategoryId == filterDto.LaboratoryCategoryId);
                    }
                    if (filterDto.LaboratoryLevelId > 0)
                    {
                        query = query.Where(x => x.LaboratoryLevelId == filterDto.LaboratoryLevelId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Latitude))
                    {
                        query = query.Where(x => x.Latitude.Trim().ToLower().Contains(filterDto.Latitude.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Longitude))
                    {
                        query = query.Where(x => x.Longitude.Trim().ToLower().Contains(filterDto.Longitude.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "RegionName".ToLower())
                    {
                        filterDto.SortProperty = "RegionId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "LaboratoryCategoryName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryCategoryId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "LaboratoryLevelName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryLevelId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<LaboratoryDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryRepository.GetAll(x => !x.IsDeleted && x.IsActive);


                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
                    }
                    if (filterDto.LaboratoryCategoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryCategoryId == filterDto.LaboratoryCategoryId);
                    }
                    if (filterDto.LaboratoryLevelId > 0)
                    {
                        query = query.Where(x => x.LaboratoryLevelId == filterDto.LaboratoryLevelId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Latitude))
                    {
                        query = query.Where(x => x.Latitude.Trim().ToLower().Contains(filterDto.Latitude.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Longitude))
                    {
                        query = query.Where(x => x.Longitude.Trim().ToLower().Contains(filterDto.Longitude.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.LookupSchema.Laboratory() { Id = i.Id, Name = i.Name, RegionId = i.RegionId });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<LaboratoryDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryDetails(int laboratoryId)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetAll()
                                        .Include(x => x.Region)
                                        .Include(x => x.LaboratoryCategory)
                                        .Include(x => x.LaboratoryLevel)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryId);
                if (laboratory == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryDto = _mapper.Map<LaboratoryDto>(laboratory);

                _response.Data = laboratoryDto;
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
        public async Task<IResponseDTO> CreateLaboratory(LaboratoryDto laboratoryDto)
        {
            try
            {
                var laboratory = _mapper.Map<Data.DbModels.LookupSchema.Laboratory>(laboratoryDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratory.LaboratoryCategory = null;
                laboratory.LaboratoryLevel = null;
                laboratory.Region = null;

                // Add to the DB
                laboratory.LaboratoryWorkingDays = GetDafaultWorkingDays(laboratoryDto.CreatedBy.Value);
                laboratory.UserLaboratorySubscriptions = new List<Data.DbModels.SecuritySchema.UserLaboratorySubscription>
                {
                    new Data.DbModels.SecuritySchema.UserLaboratorySubscription
                    {
                        CreatedBy = laboratoryDto.CreatedBy.Value,
                        CreatedOn = laboratoryDto.CreatedOn,
                        ApplicationUserId = laboratoryDto.CreatedBy.Value,
                        IsActive = true,
                        Laboratory = null,
                        ApplicationUser = null
                    }
                };
                await _laboratoryRepository.AddAsync(laboratory);

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
        public async Task<IResponseDTO> UpdateLaboratory(LaboratoryDto laboratoryDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryExist = await _laboratoryRepository.GetFirstAsync(x => x.Id == laboratoryDto.Id);
                if (laboratoryExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratory = _mapper.Map<Data.DbModels.LookupSchema.Laboratory>(laboratoryDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratory.LaboratoryCategory = null;
                laboratory.LaboratoryLevel = null;
                laboratory.Region = null;

                _laboratoryRepository.Update(laboratory);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetFirstAsync(x => x.Id == laboratoryId);
                if (laboratory == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratory.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratory.IsActive = IsActive;
                laboratory.UpdatedBy = LoggedInUserId;
                laboratory.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                laboratory.LaboratoryWorkingDays = null;
                laboratory.LaboratoryInstruments = null;
                laboratory.ForecastInfos = null;
                laboratory.LaboratoryPatientStatistics = null;
                laboratory.LaboratoryTestServices = null;
                laboratory.LaboratoryConsumptions = null;
                laboratory.LaboratoryProductPrices = null;

                // Update on the Database
                _laboratoryRepository.Update(laboratory);

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
        public async Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var items = _laboratoryRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                if (!IsSuperAdmin && items.Select(x => x.CreatedBy).Any(x => x != LoggedInUserId))
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.IsActive != isActive).Select(x =>
                {
                    x.IsActive = isActive;
                    x.UpdatedBy = LoggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _laboratoryRepository.Update(item);
                }

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no changes to save";
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
        public async Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var items = _laboratoryRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.Shared != shared).Select(x =>
                {
                    x.Shared = shared;
                    x.UpdatedBy = LoggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _laboratoryRepository.Update(item);
                }

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "There is no changes to save";
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
        public async Task<IResponseDTO> RemoveLaboratory(int laboratoryId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryId);
                if (laboratory == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratory.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratory.IsDeleted = true;
                laboratory.UpdatedBy = LoggedInUserId;
                laboratory.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                laboratory.LaboratoryWorkingDays = null;
                laboratory.LaboratoryInstruments = null;
                laboratory.ForecastInfos = null;
                laboratory.LaboratoryPatientStatistics = null;
                laboratory.LaboratoryTestServices = null;
                laboratory.LaboratoryConsumptions = null;
                laboratory.LaboratoryProductPrices = null;

                // Update on the Database
                _laboratoryRepository.Update(laboratory);

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
        public async Task<IResponseDTO> ImportLaboratories(List<LaboratoryDto> laboratoryDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratories_database = _laboratoryRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratories = _mapper.Map<List<Data.DbModels.LookupSchema.Laboratory>>(laboratoryDtos);

                // vars
                var newLaboratories = new List<Data.DbModels.LookupSchema.Laboratory>();
                var updatedLaboratories = new List<Data.DbModels.LookupSchema.Laboratory>();

                // Get new and updated laboratories
                foreach (var item in laboratories)
                {
                    var foundLaboratory = laboratories_database.FirstOrDefault(x => x.RegionId == item.RegionId
                                                                         && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (foundLaboratory == null)
                    {
                        newLaboratories.Add(item);
                    }
                    else
                    {
                        updatedLaboratories.Add(item);
                    }
                }

                // Add the new object to the database
                if (newLaboratories.Count() > 0)
                {  
                    // Set relation variables with null to avoid unexpected EF errors
                    newLaboratories.Select(x =>
                    {
                        x.LaboratoryWorkingDays = null;
                        x.LaboratoryInstruments = null;
                        x.ForecastInfos = null;
                        x.LaboratoryPatientStatistics = null;
                        x.LaboratoryTestServices = null;
                        x.LaboratoryConsumptions = null;
                        x.LaboratoryProductPrices = null;
                        x.ForecastLaboratories = null;
                        x.UserLaboratorySubscriptions = null;
                        x.LaboratoryLevel = null;
                        x.LaboratoryCategory = null;
                        x.Region = null;
                        x.Creator = null;
                        x.Updator = null;
                        x.LaboratoryWorkingDays = GetDafaultWorkingDays(x.CreatedBy.Value);
                        return x;
                    }).ToList();
                    await _laboratoryRepository.AddRangeAsync(newLaboratories);
                }

                // Update the existing objects with the new values
                if (updatedLaboratories.Count() > 0)
                {
                    foreach (var item in updatedLaboratories)
                    {
                        var fromDatabase = laboratories_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                        && x.RegionId == item.RegionId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.RegionId = item.RegionId;
                        fromDatabase.LaboratoryCategoryId = item.LaboratoryCategoryId;
                        fromDatabase.LaboratoryLevelId = item.LaboratoryLevelId;
                        fromDatabase.Latitude = item.Latitude;
                        fromDatabase.Longitude = item.Longitude;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.LaboratoryWorkingDays = null;
                        fromDatabase.LaboratoryInstruments = null;
                        fromDatabase.ForecastInfos = null;
                        fromDatabase.LaboratoryPatientStatistics = null;
                        fromDatabase.LaboratoryTestServices = null;
                        fromDatabase.LaboratoryConsumptions = null;
                        fromDatabase.LaboratoryProductPrices = null;
                        fromDatabase.ForecastLaboratories = null;
                        fromDatabase.UserLaboratorySubscriptions = null;
                        fromDatabase.LaboratoryLevel = null;
                        fromDatabase.LaboratoryCategory = null;
                        fromDatabase.Region = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _laboratoryRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratories.Count();
                var numberOfUpdated = updatedLaboratories.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = laboratoryDtos.Count,
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
        public GeneratedFile ExportLaboratories(int? pageIndex = null, int? pageSize = null, LaboratoryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.Laboratory> query = null;
            try
            {
                query = _laboratoryRepository.GetAll()
                                    .Include(x => x.Region)
                                    .Include(x => x.LaboratoryCategory)
                                    .Include(x => x.LaboratoryLevel)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }


                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
                    }
                    if (filterDto.LaboratoryCategoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryCategoryId == filterDto.LaboratoryCategoryId);
                    }
                    if (filterDto.LaboratoryLevelId > 0)
                    {
                        query = query.Where(x => x.LaboratoryLevelId == filterDto.LaboratoryLevelId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Latitude))
                    {
                        query = query.Where(x => x.Latitude.Trim().ToLower().Contains(filterDto.Latitude.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Longitude))
                    {
                        query = query.Where(x => x.Longitude.Trim().ToLower().Contains(filterDto.Longitude.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Name);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "RegionName".ToLower())
                    {
                        filterDto.SortProperty = "RegionId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "LaboratoryCategoryName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryCategoryId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "LaboratoryLevelName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryLevelId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }
                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }
                var dataList = _mapper.Map<List<ExportLaboratoryDto>>(query.ToList());

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
        // Helper methods
        private List<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay> GetDafaultWorkingDays(int loggedInUserId)
        {
            var days = Enum.GetNames(typeof(DayOfWeek)).Where(x => x != DayOfWeek.Saturday.ToString() && x != DayOfWeek.Monday.ToString()).ToList();
            var result = days.ConvertAll(day =>
            {
                return new Data.DbModels.LaboratorySchema.LaboratoryWorkingDay
                {
                    Day = day,
                    FromTime = new TimeSpan(8, 0, 0),
                    ToTime = new TimeSpan(14, 0, 0),
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                };
            });
            return result;
        }
        // Validators methods
        public bool IsNameUnique(LaboratoryDto laboratoryDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _laboratoryRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryDto.Id
                                                && x.RegionId == laboratoryDto.RegionId
                                                && x.Name.ToLower().Trim() == laboratoryDto.Name.ToLower().Trim());
            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => x.CreatedBy == null || createdBy.Contains(x.CreatedBy.Value));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsLatlngUnique(LaboratoryDto laboratoryDto)
        {
            var searchResult = _laboratoryRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryDto.Id
                                                && x.Latitude.ToLower().Trim() == laboratoryDto.Latitude.ToLower().Trim()
                                                && x.Longitude.ToLower().Trim() == laboratoryDto.Longitude.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryId)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetAll()
                                        .Include(x => x.Region)
                                        .Include(x => x.LaboratoryCategory)
                                        .Include(x => x.LaboratoryLevel)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryId);
                if (laboratory == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (laboratory.LaboratoryWorkingDays.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'Days of laboratory working'";
                    return _response;
                }
                if (laboratory.LaboratoryInstruments.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'Instruments'";
                    return _response;
                }
                if (laboratory.ForecastInfos.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'ForecastInformations'";
                    return _response;
                }
                if (laboratory.LaboratoryPatientStatistics.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'Patient Statistics'";
                    return _response;
                }
                if (laboratory.LaboratoryTestServices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'Test Services'";
                    return _response;
                }
                if (laboratory.LaboratoryConsumptions.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'Consumptions'";
                    return _response;
                }
                if (laboratory.LaboratoryProductPrices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory cannot be deleted or deactivated where it contains 'Product Prices'";
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