using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.PatientGroup;
using ForLab.Repositories.Lookup.PatientGroup;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.PatientGroup
{
    public class PatientGroupService : GService<PatientGroupDto, Data.DbModels.LookupSchema.PatientGroup, IPatientGroupRepository>, IPatientGroupService
    {
        private readonly IPatientGroupRepository _patientGroupRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportPatientGroupDto> _fileService;
        private readonly IGeneralService _generalService;

        public PatientGroupService(IMapper mapper,
            IResponseDTO response,
            IPatientGroupRepository patientGroupRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportPatientGroupDto> fileService,
            IGeneralService generalService) : base(patientGroupRepository, mapper)
        {
            _patientGroupRepository = patientGroupRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, PatientGroupFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.PatientGroup> query = null;
            try
            {
                query = _patientGroupRepository.GetAll()
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

                var dataList = _mapper.Map<List<PatientGroupDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(PatientGroupFilterDto filterDto = null)
        {
            try
            {
                var query = _patientGroupRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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

                query = query.Select(i => new Data.DbModels.LookupSchema.PatientGroup() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<PatientGroupDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetPatientGroupDetails(int patientGroupId)
        {
            try
            {
                var patientGroup = await _patientGroupRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == patientGroupId);
                if (patientGroup == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var patientGroupDto = _mapper.Map<PatientGroupDto>(patientGroup);

                _response.Data = patientGroupDto;
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
        public async Task<IResponseDTO> CreatePatientGroup(PatientGroupDto patientGroupDto)
        {
            try
            {
                var patientGroup = _mapper.Map<Data.DbModels.LookupSchema.PatientGroup>(patientGroupDto);

                // Set relation variables with null to avoid unexpected EF errors
                // patientGroup.TestingProtocols = null;
                patientGroup.ForecastPatientGroups = null;
               
                // Add to the DB
                await _patientGroupRepository.AddAsync(patientGroup);

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
        public async Task<IResponseDTO> UpdatePatientGroup(PatientGroupDto patientGroupDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var patientGroupExist = await _patientGroupRepository.GetFirstAsync(x => x.Id == patientGroupDto.Id);
                if (patientGroupExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && patientGroupExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var patientGroup = _mapper.Map<Data.DbModels.LookupSchema.PatientGroup>(patientGroupDto);

                // Set relation variables with null to avoid unexpected EF errors
                patientGroup.ForecastPatientGroups = null;
               
                _patientGroupRepository.Update(patientGroup);

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
        public async Task<IResponseDTO> UpdateIsActive(int patientGroupId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var patientGroup = await _patientGroupRepository.GetFirstAsync(x => x.Id == patientGroupId);
                if (patientGroup == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && patientGroup.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                patientGroup.IsActive = IsActive;
                patientGroup.UpdatedBy = LoggedInUserId;
                patientGroup.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                patientGroup.ForecastPatientGroups = null; 
              
                //patientGroup.TestingProtocols = null;

                // Update on the Database
                _patientGroupRepository.Update(patientGroup);

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
                var items = _patientGroupRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _patientGroupRepository.Update(item);
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
                var items = _patientGroupRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _patientGroupRepository.Update(item);
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
        public async Task<IResponseDTO> RemovePatientGroup(int patientGroupId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var patientGroup = await _patientGroupRepository.GetFirstOrDefaultAsync(x => x.Id == patientGroupId);
                if (patientGroup == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && patientGroup.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                patientGroup.IsDeleted = true;
                patientGroup.UpdatedBy = LoggedInUserId;
                patientGroup.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                //patientGroup.TestingProtocols = null;
                patientGroup.ForecastPatientGroups = null;
                
                // Update on the Database
                _patientGroupRepository.Update(patientGroup);

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
        public async Task<IResponseDTO> ImportPatientGroups(List<PatientGroupDto> patientGroupDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var patientGroups_database = _patientGroupRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var patientGroups = _mapper.Map<List<Data.DbModels.LookupSchema.PatientGroup>>(patientGroupDtos);

                // vars
                var names_database = patientGroups_database.Select(x => x.Name.ToLower().Trim());
                var names_dto = patientGroups.Select(x => x.Name.ToLower().Trim());
                // Get the new ones that their names don't exist on the database
                var newPatientGroups = patientGroups.Where(x => !names_database.Contains(x.Name.ToLower().Trim()));
                // Select the objects that their names already exist in the database
                var updatedPatientGroups = patientGroups_database.Where(x => names_dto.Contains(x.Name.ToLower().Trim()));
                if (!IsSuperAdmin)
                {
                    updatedPatientGroups = updatedPatientGroups.Where(x => x.CreatedBy == LoggedInUserId);
                }

                // Set relation variables with null to avoid unexpected EF errors
                newPatientGroups.Select(x =>
                {
                    x.ForecastPatientGroups = null;
                    x.ForecastMorbidityTestingProtocolMonths = null;
                    x.Creator = null;
                    x.Updator = null;
                    return x;
                }).ToList();

                // Add the new object to the database
                if (newPatientGroups.Count() > 0)
                {
                    await _patientGroupRepository.AddRangeAsync(newPatientGroups);
                }

                // Update the existing objects with the new values
                if (updatedPatientGroups.Count() > 0)
                {
                    foreach (var item in updatedPatientGroups)
                    {
                        var dto = patientGroupDtos.First(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        item.UpdatedOn = DateTime.Now;
                        item.UpdatedBy = dto.CreatedBy;
                        item.Name = dto.Name;
                        // Set relation variables with null to avoid unexpected EF errors
                        item.ForecastPatientGroups = null;
                        item.ForecastMorbidityTestingProtocolMonths = null;
                        item.Creator = null;
                        item.Updator = null;
                        _patientGroupRepository.Update(item);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newPatientGroups.Count();
                var numberOfUpdated = updatedPatientGroups.Count();

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
                    NumberOfUploded = patientGroupDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = patientGroupDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public GeneratedFile ExportPatientGroups(int? pageIndex = null, int? pageSize = null, PatientGroupFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.PatientGroup> query = null;
            try
            {
                query = _patientGroupRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if (!filterDto.IsSuperAdmin)
                    {
                        var createdBy = _generalService.SuperAdminIds();
                        createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == null || createdBy.Contains(x.CreatedBy.Value));
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

                var dataList = _mapper.Map<List<ExportPatientGroupDto>>(query.ToList());
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
        public bool IsNameUnique(PatientGroupDto patientGroupDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _patientGroupRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != patientGroupDto.Id
                                                && x.Name.ToLower().Trim() == patientGroupDto.Name.ToLower().Trim());
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
        public async Task<IResponseDTO> IsUsed(int patientGroupId)
        {
            try
            {
                var patientGroup = await _patientGroupRepository.GetAll()
                                        .Include(x => x.ForecastPatientGroups)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == patientGroupId);
                if (patientGroup == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (patientGroup.ForecastPatientGroups.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "patient Group cannot be deleted or deactivated where it contains 'Forecasts'";
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
