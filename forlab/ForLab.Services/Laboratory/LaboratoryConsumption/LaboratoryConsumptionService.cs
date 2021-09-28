using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Laboratory.LaboratoryConsumption;
using ForLab.Repositories.Laboratory.LaboratoryConsumption;
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

namespace ForLab.Services.Laboratory.LaboratoryConsumption
{
    public class LaboratoryConsumptionService : GService<LaboratoryConsumptionDto, Data.DbModels.LaboratorySchema.LaboratoryConsumption, ILaboratoryConsumptionRepository>, ILaboratoryConsumptionService
    {
        private readonly ILaboratoryConsumptionRepository _laboratoryConsumptionRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryConsumptionDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryConsumptionService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryConsumptionRepository laboratoryConsumptionRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
             IFileService<ExportLaboratoryConsumptionDto> fileService,
             IGeneralService generalService) : base(laboratoryConsumptionRepository, mapper)
        {
            _laboratoryConsumptionRepository = laboratoryConsumptionRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryConsumptionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryConsumption> query = null;
            try
            {
                query = _laboratoryConsumptionRepository.GetAll()
                                    .Include(x => x.Product)
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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.ConsumptionDuration != null)
                    {
                        query = query.Where(x => x.ConsumptionDuration.Date == filterDto.ConsumptionDuration.Value.Date);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ProductName".ToLower())
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

                var dataList = _mapper.Map<List<LaboratoryConsumptionDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryConsumptionFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryConsumptionRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ConsumptionDuration != null)
                    {
                        query = query.Where(x => x.ConsumptionDuration.Date == filterDto.ConsumptionDuration.Value.Date);
                    }
                }

                query = query.Select(i => new Data.DbModels.LaboratorySchema.LaboratoryConsumption()
                {
                    Id = i.Id,
                    Laboratory = i.Laboratory,
                    Product = i.Product
                });
                query = query.OrderBy(x => x.ProductId);
                var dataList = _mapper.Map<List<LaboratoryConsumptionDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryConsumptionDetails(int laboratoryConsumptionId)
        {
            try
            {
                var laboratoryConsumption = await _laboratoryConsumptionRepository.GetAll()
                                        .Include(x => x.Product)
                                        .Include(x => x.Laboratory)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryConsumptionId);
                if (laboratoryConsumption == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryConsumptionDto = _mapper.Map<LaboratoryConsumptionDto>(laboratoryConsumption);

                _response.Data = laboratoryConsumptionDto;
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
        public async Task<IResponseDTO> CreateLaboratoryConsumption(LaboratoryConsumptionDto laboratoryConsumptionDto)
        {
            try
            {
                var laboratoryConsumption = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryConsumption>(laboratoryConsumptionDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryConsumption.Product = null;
                laboratoryConsumption.Laboratory = null;

                // Add to the DB
                await _laboratoryConsumptionRepository.AddAsync(laboratoryConsumption);

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
        public async Task<IResponseDTO> UpdateLaboratoryConsumption(LaboratoryConsumptionDto laboratoryConsumptionDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryConsumptionExist = await _laboratoryConsumptionRepository.GetFirstAsync(x => x.Id == laboratoryConsumptionDto.Id);
                if (laboratoryConsumptionExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryConsumptionExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryConsumption = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryConsumption>(laboratoryConsumptionDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryConsumption.Laboratory = null;
                laboratoryConsumption.Product = null;

                _laboratoryConsumptionRepository.Update(laboratoryConsumption);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryConsumptionId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryConsumption = await _laboratoryConsumptionRepository.GetFirstAsync(x => x.Id == laboratoryConsumptionId);
                if (laboratoryConsumption == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryConsumption.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryConsumption.IsActive = IsActive;
                laboratoryConsumption.UpdatedBy = LoggedInUserId;
                laboratoryConsumption.UpdatedOn = DateTime.Now;


                // Update on the Database
                _laboratoryConsumptionRepository.Update(laboratoryConsumption);

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
        public async Task<IResponseDTO> RemoveLaboratoryConsumption(int laboratoryConsumptionId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryConsumption = await _laboratoryConsumptionRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryConsumptionId);
                if (laboratoryConsumption == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryConsumption.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryConsumption.IsDeleted = true;
                laboratoryConsumption.UpdatedBy = LoggedInUserId;
                laboratoryConsumption.UpdatedOn = DateTime.Now;


                // Update on the Database
                _laboratoryConsumptionRepository.Update(laboratoryConsumption);

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
        public GeneratedFile ExportLaboratoryConsumptions(int? pageIndex = null, int? pageSize = null, LaboratoryConsumptionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryConsumption> query = null;
            try
            {
                query = _laboratoryConsumptionRepository.GetAll()
                                    .Include(x => x.Product)
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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.ConsumptionDuration != null)
                    {
                        query = query.Where(x => x.ConsumptionDuration.Date == filterDto.ConsumptionDuration.Value.Date);
                    }
                }
                query = query.OrderByDescending(x => x.ProductId);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ProductName".ToLower())
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

                var dataList = _mapper.Map<List<ExportLaboratoryConsumptionDto>>(query.ToList());
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
        public async Task<IResponseDTO> ImportLaboratoryConsumptions(List<LaboratoryConsumptionDto> laboratoryConsumptionDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryConsumptions_database = _laboratoryConsumptionRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryConsumptions = _mapper.Map<List<Data.DbModels.LaboratorySchema.LaboratoryConsumption>>(laboratoryConsumptionDtos);

                // vars
                var newLaboratoryConsumptions = new List<Data.DbModels.LaboratorySchema.LaboratoryConsumption>();
                var updatedLaboratoryConsumptions = new List<Data.DbModels.LaboratorySchema.LaboratoryConsumption>();

                // Get new and updated laboratoryConsumptions
                foreach (var item in laboratoryConsumptions)
                {
                    var foundLaboratoryConsumption = laboratoryConsumptions_database.FirstOrDefault(x => x.ConsumptionDuration.Date == item.ConsumptionDuration.Date
                                                                                                      && x.LaboratoryId == item.LaboratoryId
                                                                                                      && x.ProductId == item.ProductId);

                    if (foundLaboratoryConsumption == null)
                    {
                        newLaboratoryConsumptions.Add(item);
                    }
                    else
                    {
                        updatedLaboratoryConsumptions.Add(item);
                    }
                }

                // Add the new object to the database
                if (newLaboratoryConsumptions.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newLaboratoryConsumptions.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Laboratory = null;
                        x.Product = null;
                        return x;
                    }).ToList();
                    await _laboratoryConsumptionRepository.AddRangeAsync(newLaboratoryConsumptions);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryConsumptions.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryConsumptions)
                    {
                        var fromDatabase = laboratoryConsumptions_database.FirstOrDefault(x => x.ConsumptionDuration.Date == item.ConsumptionDuration.Date
                                                                                            && x.LaboratoryId == item.LaboratoryId
                                                                                            && x.ProductId == item.ProductId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ConsumptionDuration = item.ConsumptionDuration;
                        fromDatabase.LaboratoryId = item.LaboratoryId;
                        fromDatabase.ProductId = item.ProductId;
                        fromDatabase.AmountUsed = item.AmountUsed;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        fromDatabase.Laboratory = null;
                        fromDatabase.Product = null;
                        _laboratoryConsumptionRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryConsumptions.Count();
                var numberOfUpdated = updatedLaboratoryConsumptions.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = laboratoryConsumptionDtos.Count,
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
        public bool IsConsumptionUnique(LaboratoryConsumptionDto laboratoryConsumptionDto)
        {
            var searchResult = _laboratoryConsumptionRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryConsumptionDto.Id
                                                && x.LaboratoryId == laboratoryConsumptionDto.LaboratoryId
                                                && x.ProductId == laboratoryConsumptionDto.ProductId
                                                && x.ConsumptionDuration.Year == laboratoryConsumptionDto.ConsumptionDuration.Year
                                                && x.ConsumptionDuration.Month == laboratoryConsumptionDto.ConsumptionDuration.Month);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryConsumptionId)
        {
            try
            {
                var laboratoryConsumption = await _laboratoryConsumptionRepository.GetAll()
                                        .Include(x => x.Product)
                                        .Include(x => x.Laboratory)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryConsumptionId);
                if (laboratoryConsumption == null)
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
