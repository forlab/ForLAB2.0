using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.LaboratoryLevel;
using ForLab.Repositories.Lookup.LaboratoryLevel;
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

namespace ForLab.Services.Lookup.LaboratoryLevel
{
    public class LaboratoryLevelService : GService<LaboratoryLevelDto, Data.DbModels.LookupSchema.LaboratoryLevel, ILaboratoryLevelRepository>, ILaboratoryLevelService
    {
        private readonly ILaboratoryLevelRepository _laboratoryLevelRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryLevelDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryLevelService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryLevelRepository laboratoryLevelRepository,
            IUnitOfWork<AppDbContext> unitOfWork, 
            IFileService<ExportLaboratoryLevelDto> fileService,
            IGeneralService generalService) : base(laboratoryLevelRepository, mapper)
        {
            _laboratoryLevelRepository = laboratoryLevelRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryLevelFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.LaboratoryLevel> query = null;
            try
            {
                query = _laboratoryLevelRepository.GetAll()
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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
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

                var dataList = _mapper.Map<List<LaboratoryLevelDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryLevelFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryLevelRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.LookupSchema.LaboratoryLevel() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<LaboratoryLevelDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryLevelDetails(int laboratoryLevelId)
        {
            try
            {
                var laboratoryLevel = await _laboratoryLevelRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryLevelId);
                if (laboratoryLevel == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryLevelDto = _mapper.Map<LaboratoryLevelDto>(laboratoryLevel);

                _response.Data = laboratoryLevelDto;
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
        public async Task<IResponseDTO> CreateLaboratoryLevel(LaboratoryLevelDto laboratoryLevelDto)
        {
            try
            {
                var laboratoryLevel = _mapper.Map<Data.DbModels.LookupSchema.LaboratoryLevel>(laboratoryLevelDto);


                // Add to the DB
                await _laboratoryLevelRepository.AddAsync(laboratoryLevel);

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
        public async Task<IResponseDTO> UpdateLaboratoryLevel(LaboratoryLevelDto laboratoryLevelDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryLevelExist = await _laboratoryLevelRepository.GetFirstAsync(x => x.Id == laboratoryLevelDto.Id);
                if (laboratoryLevelExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryLevelExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryLevel = _mapper.Map<Data.DbModels.LookupSchema.LaboratoryLevel>(laboratoryLevelDto);

                _laboratoryLevelRepository.Update(laboratoryLevel);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryLevelId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryLevel = await _laboratoryLevelRepository.GetFirstAsync(x => x.Id == laboratoryLevelId);
                if (laboratoryLevel == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryLevel.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryLevel.IsActive = IsActive;
                laboratoryLevel.UpdatedBy = LoggedInUserId;
                laboratoryLevel.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                laboratoryLevel.Laboratories = null;


                // Update on the Database
                _laboratoryLevelRepository.Update(laboratoryLevel);

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
                var items = _laboratoryLevelRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _laboratoryLevelRepository.Update(item);
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
                var items = _laboratoryLevelRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _laboratoryLevelRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveLaboratoryLevel(int laboratoryLevelId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryLevel = await _laboratoryLevelRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryLevelId);
                if (laboratoryLevel == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryLevel.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryLevel.IsDeleted = true;
                laboratoryLevel.UpdatedBy = LoggedInUserId;
                laboratoryLevel.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                laboratoryLevel.Laboratories = null;

                // Update on the Database
                _laboratoryLevelRepository.Update(laboratoryLevel);

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
        public async Task<IResponseDTO> ImportLaboratoryLevels(List<LaboratoryLevelDto> laboratoryLevelDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryLevels_database = _laboratoryLevelRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryLevels = _mapper.Map<List<Data.DbModels.LookupSchema.LaboratoryLevel>>(laboratoryLevelDtos);

                // vars
                var names_database = laboratoryLevels_database.Select(x => x.Name.ToLower().Trim());
                var names_dto = laboratoryLevels.Select(x => x.Name.ToLower().Trim());
                // Get the new ones that their names don't exist on the database
                var newLaboratoryLevels = laboratoryLevels.Where(x => !names_database.Contains(x.Name.ToLower().Trim()));
                // Select the objects that their names already exist in the database
                var updatedLaboratoryLevels = laboratoryLevels_database.Where(x => names_dto.Contains(x.Name.ToLower().Trim()));
                if (!IsSuperAdmin)
                {
                    updatedLaboratoryLevels = updatedLaboratoryLevels.Where(x => x.CreatedBy == LoggedInUserId);
                }

                // Set relation variables with null to avoid unexpected EF errors
                newLaboratoryLevels.Select(x =>
                {
                    x.Laboratories = null;
                    x.Creator = null;
                    x.Updator = null;
                    return x;
                }).ToList();

                // Add the new object to the database
                if (newLaboratoryLevels.Count() > 0)
                {
                    await _laboratoryLevelRepository.AddRangeAsync(newLaboratoryLevels);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryLevels.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryLevels)
                    {
                        var dto = laboratoryLevelDtos.First(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        item.UpdatedOn = DateTime.Now;
                        item.UpdatedBy = dto.CreatedBy;
                        item.Name = dto.Name;
                        // Set relation variables with null to avoid unexpected EF errors
                        item.Laboratories = null;
                        item.Creator = null;
                        item.Updator = null;
                        _laboratoryLevelRepository.Update(item);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryLevels.Count();
                var numberOfUpdated = updatedLaboratoryLevels.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();
                if (save == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Not saved";
                    return _response;
                }

                _response.Data = new
                {
                    NumberOfUploded = laboratoryLevelDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = laboratoryLevelDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public GeneratedFile ExportLaboratoryLevels(int? pageIndex = null, int? pageSize = null, LaboratoryLevelFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.LaboratoryLevel> query = null;
            try
            {
                query = _laboratoryLevelRepository.GetAll()
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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Name);
                var total = query.Count();

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportLaboratoryLevelDto>>(query.ToList());
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
        public bool IsNameUnique(LaboratoryLevelDto laboratoryLevelDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _laboratoryLevelRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryLevelDto.Id
                                                && x.Name.ToLower().Trim() == laboratoryLevelDto.Name.ToLower().Trim());
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
        public async Task<IResponseDTO> IsUsed(int laboratoryLevelId)
        {
            try
            {
                var laboratoryLevel = await _laboratoryLevelRepository.GetAll()
                                        .Include(x => x.Laboratories)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryLevelId);
                if (laboratoryLevel == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (laboratoryLevel.Laboratories.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory Level cannot be deleted or deactivated where it contains 'Laboratories'";
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
