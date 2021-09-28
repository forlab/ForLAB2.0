using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Laboratory.LaboratoryTestService;
using ForLab.Repositories.Laboratory.LaboratoryTestService;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;

namespace ForLab.Services.Laboratory.LaboratoryTestService
{
    public class LaboratoryTestService : GService<LaboratoryTestServiceDto, Data.DbModels.LaboratorySchema.LaboratoryTestService, ILaboratoryTestServiceRepository>, ILaboratoryTestService
    {
        private readonly ILaboratoryTestServiceRepository _laboratoryTestServiceRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryTestServiceDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryTestService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryTestServiceRepository laboratoryTestServiceRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportLaboratoryTestServiceDto> fileService,
            IGeneralService generalService) : base(laboratoryTestServiceRepository, mapper)
        {
            _laboratoryTestServiceRepository = laboratoryTestServiceRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryTestServiceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryTestService> query = null;
            try
            {
                query = _laboratoryTestServiceRepository.GetAll()
                                    .Include(x => x.Test)
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
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ServiceDuration != null)
                    {
                        query = query.Where(x => x.ServiceDuration.Date == filterDto.ServiceDuration.Value.Date);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "LaboratoryName".ToLower())
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

                var dataList = _mapper.Map<List<LaboratoryTestServiceDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryTestServiceFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryTestServiceRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ServiceDuration != null)
                    {
                        query = query.Where(x => x.ServiceDuration.Date == filterDto.ServiceDuration.Value.Date);
                    }
                }

                query = query.Select(i => new Data.DbModels.LaboratorySchema.LaboratoryTestService() 
                {
                    Id = i.Id,
                    Laboratory = i.Laboratory,
                    Test = i.Test
                });
                query = query.OrderBy(x => x.TestId);
                var dataList = _mapper.Map<List<LaboratoryTestServiceDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryTestServiceDetails(int laboratoryTestServiceId)
        {
            try
            {
                var laboratoryTestService = await _laboratoryTestServiceRepository.GetAll()
                                        .Include(x => x.Test)
                                        .Include(x => x.Laboratory)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryTestServiceId);
                if (laboratoryTestService == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryTestServiceDto = _mapper.Map<LaboratoryTestServiceDto>(laboratoryTestService);

                _response.Data = laboratoryTestServiceDto;
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
        public async Task<IResponseDTO> CreateLaboratoryTestService(LaboratoryTestServiceDto laboratoryTestServiceDto)
        {
            try
            {
                var laboratoryTestService = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryTestService>(laboratoryTestServiceDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryTestService.Test = null;
                laboratoryTestService.Laboratory = null;

                // Add to the DB
                await _laboratoryTestServiceRepository.AddAsync(laboratoryTestService);

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
        public async Task<IResponseDTO> UpdateLaboratoryTestService(LaboratoryTestServiceDto laboratoryTestServiceDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryTestServiceExist = await _laboratoryTestServiceRepository.GetFirstAsync(x => x.Id == laboratoryTestServiceDto.Id);
                if (laboratoryTestServiceExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryTestServiceExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryTestService = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryTestService>(laboratoryTestServiceDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryTestService.Laboratory = null;
                laboratoryTestService.Test = null;

                _laboratoryTestServiceRepository.Update(laboratoryTestService);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryTestServiceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryTestService = await _laboratoryTestServiceRepository.GetFirstAsync(x => x.Id == laboratoryTestServiceId);
                if (laboratoryTestService == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryTestService.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryTestService.IsActive = IsActive;
                laboratoryTestService.UpdatedBy = LoggedInUserId;
                laboratoryTestService.UpdatedOn = DateTime.Now;


                // Update on the Database
                _laboratoryTestServiceRepository.Update(laboratoryTestService);

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
        public async Task<IResponseDTO> RemoveLaboratoryTestService(int laboratoryTestServiceId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryTestService = await _laboratoryTestServiceRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryTestServiceId);
                if (laboratoryTestService == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryTestService.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryTestService.IsDeleted = true;
                laboratoryTestService.UpdatedBy = LoggedInUserId;
                laboratoryTestService.UpdatedOn = DateTime.Now;


                // Update on the Database
                _laboratoryTestServiceRepository.Update(laboratoryTestService);

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
        public GeneratedFile ExportLaboratoryTestServices(int? pageIndex = null, int? pageSize = null, LaboratoryTestServiceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryTestService> query = null;
            try
            {
                query = _laboratoryTestServiceRepository.GetAll()
                                    .Include(x => x.Test)
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
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ServiceDuration != null)
                    {
                        query = query.Where(x => x.ServiceDuration.Date == filterDto.ServiceDuration.Value.Date);
                    }
                    if (filterDto.TestPerformed > 0)
                    {
                        query = query.Where(x => x.TestPerformed == filterDto.TestPerformed);
                    }
                }

                query = query.OrderByDescending(x => x.CreatedOn);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "LaboratoryName".ToLower())
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

                var dataList = _mapper.Map<List<ExportLaboratoryTestServiceDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportLaboratoryTestServices(List<LaboratoryTestServiceDto> laboratoryTestServiceDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryTestServices_database = _laboratoryTestServiceRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryTestServices = _mapper.Map<List<Data.DbModels.LaboratorySchema.LaboratoryTestService>>(laboratoryTestServiceDtos);

                // vars
                var newLaboratoryTestServices = new List<Data.DbModels.LaboratorySchema.LaboratoryTestService>();
                var updatedLaboratoryTestServices = new List<Data.DbModels.LaboratorySchema.LaboratoryTestService>();

                // Get new and updated laboratoryTestServices
                foreach (var item in laboratoryTestServices)
                {
                    var foundLaboratoryTestService = laboratoryTestServices_database.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId
                                                                                && x.ServiceDuration.Date == item.ServiceDuration.Date
                                                                                && x.TestId == item.TestId);
                    if (foundLaboratoryTestService == null)
                    {
                        newLaboratoryTestServices.Add(item);
                    }
                    else
                    {
                        updatedLaboratoryTestServices.Add(item);
                    }
                }

                // Add the new object to the database
                if (newLaboratoryTestServices.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newLaboratoryTestServices.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Laboratory = null;
                        x.Test = null;
                        return x;
                    }).ToList();
                    await _laboratoryTestServiceRepository.AddRangeAsync(newLaboratoryTestServices);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryTestServices.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryTestServices)
                    {
                        var fromDatabase = laboratoryTestServices_database.FirstOrDefault(x => x.ServiceDuration.Date == item.ServiceDuration.Date 
                                                                                                        && x.LaboratoryId == item.LaboratoryId
                                                                                                        && x.TestId == item.TestId );
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ServiceDuration = item.ServiceDuration;
                        fromDatabase.LaboratoryId = item.LaboratoryId;
                        fromDatabase.TestId = item.TestId;
                        fromDatabase.TestPerformed = item.TestPerformed;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        fromDatabase.Laboratory = null;
                        fromDatabase.Test = null;
                        _laboratoryTestServiceRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryTestServices.Count();
                var numberOfUpdated = updatedLaboratoryTestServices.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = laboratoryTestServiceDtos.Count,
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
        public bool IsTestServiceUnique(LaboratoryTestServiceDto laboratoryTestServiceDto)
        {
            var searchResult = _laboratoryTestServiceRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryTestServiceDto.Id
                                                && x.LaboratoryId == laboratoryTestServiceDto.LaboratoryId
                                                && x.TestId == laboratoryTestServiceDto.TestId
                                                && x.ServiceDuration.Year == laboratoryTestServiceDto.ServiceDuration.Year
                                                && x.ServiceDuration.Month == laboratoryTestServiceDto.ServiceDuration.Month);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryTestServiceId)
        {
            try
            {
                var laboratoryTestService = await _laboratoryTestServiceRepository.GetAll()
                                        .Include(x => x.Test)
                                        .Include(x => x.Laboratory)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryTestServiceId);
                if (laboratoryTestService == null)
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
