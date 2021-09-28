using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Product.Instrument;
using ForLab.Repositories.Product.Instrument;
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

namespace ForLab.Services.Product.Instrument
{
    public class InstrumentService : GService<InstrumentDto, Data.DbModels.ProductSchema.Instrument, IInstrumentRepository>, IInstrumentService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportInstrumentDto> _fileService;
        private readonly IGeneralService _generalService;
        public InstrumentService(IMapper mapper,
            IResponseDTO response,
            IInstrumentRepository instrumentRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportInstrumentDto> fileService,
            IGeneralService generalService) : base(instrumentRepository, mapper)
        {
            _instrumentRepository = instrumentRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, InstrumentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.Instrument> query = null;
            try
            {
                query = _instrumentRepository.GetAll()
                                    .Include(x => x.Vendor)
                                    .Include(x => x.ThroughPutUnit)
                                    .Include(x => x.TestingArea)
                                    .Include(x => x.ReagentSystem)
                                    .Include(x => x.ControlRequirementUnit)
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
                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (filterDto.ThroughPutUnitId > 0)
                    {
                        query = query.Where(x => x.ThroughPutUnitId == filterDto.ThroughPutUnitId);
                    }
                    if (filterDto.TestingAreaId > 0)
                    {
                        query = query.Where(x => x.TestingAreaId == filterDto.TestingAreaId);
                    }
                    if (filterDto.ReagentSystemId > 0)
                    {
                        query = query.Where(x => x.ReagentSystemId == filterDto.ReagentSystemId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.MaxThroughPut > 0)
                    {
                        query = query.Where(x => x.MaxThroughPut == filterDto.MaxThroughPut);
                    }
                    if (filterDto.ControlRequirementUnitId > 0)
                    {
                        query = query.Where(x => x.ControlRequirementUnitId == filterDto.ControlRequirementUnitId);
                    }
                    if (filterDto.ControlRequirement > 0)
                    {
                        query = query.Where(x => x.ControlRequirement == filterDto.ControlRequirement);
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "VendorName".ToLower())
                    {
                        filterDto.SortProperty = "VendorId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ThroughPutUnitName".ToLower())
                    {
                        filterDto.SortProperty = "ThroughPutUnitId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "TestingAreaName".ToLower())
                    {
                        filterDto.SortProperty = "TestingAreaId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ReagentSystemName".ToLower())
                    {
                        filterDto.SortProperty = "ReagentSystemId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ControlRequirementUnitName".ToLower())
                    {
                        filterDto.SortProperty = "ControlRequirementUnitId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<InstrumentDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(InstrumentFilterDto filterDto = null)
        {
            try
            {
                var query = _instrumentRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (filterDto.ThroughPutUnitId > 0)
                    {
                        query = query.Where(x => x.ThroughPutUnitId == filterDto.ThroughPutUnitId);
                    }
                    if (filterDto.TestingAreaId > 0)
                    {
                        query = query.Where(x => x.TestingAreaId == filterDto.TestingAreaId);
                    }
                    if (filterDto.ReagentSystemId > 0)
                    {
                        query = query.Where(x => x.ReagentSystemId == filterDto.ReagentSystemId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.MaxThroughPut > 0)
                    {
                        query = query.Where(x => x.MaxThroughPut == filterDto.MaxThroughPut);
                    }
                    if (filterDto.ControlRequirementUnitId > 0)
                    {
                        query = query.Where(x => x.ControlRequirementUnitId == filterDto.ControlRequirementUnitId);
                    }
                    if (filterDto.ControlRequirement > 0)
                    {
                        query = query.Where(x => x.ControlRequirement == filterDto.ControlRequirement);
                    }
                }

                query = query.Select(i => new Data.DbModels.ProductSchema.Instrument() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<InstrumentDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetInstrumentDetails(int instrumentId)
        {
            try
            {
                var instrument = await _instrumentRepository.GetAll()
                                        .Include(x => x.Vendor)
                                    .Include(x => x.ThroughPutUnit)
                                    .Include(x => x.TestingArea)
                                    .Include(x => x.ReagentSystem)
                                    .Include(x => x.ControlRequirementUnit)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == instrumentId);
                if (instrument == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var intrumentDto = _mapper.Map<InstrumentDto>(instrument);

                _response.Data = intrumentDto;
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
        public async Task<IResponseDTO> CreateInstrument(InstrumentDto instrumentDto)
        {
            try
            {
                var instrument = _mapper.Map<Data.DbModels.ProductSchema.Instrument>(instrumentDto);

                // Set relation variables with null to avoid unexpected EF errors
                instrument.ReagentSystem = null;
                instrument.TestingArea = null;
                instrument.ThroughPutUnit = null;
                instrument.Vendor = null;
                instrument.ControlRequirementUnit = null;

                // Add to the DB
                await _instrumentRepository.AddAsync(instrument);

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
        public async Task<IResponseDTO> UpdateInstrument(InstrumentDto instrumentDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var instrumentExist = await _instrumentRepository.GetFirstAsync(x => x.Id == instrumentDto.Id);
                if (instrumentExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && instrumentExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var instrument = _mapper.Map<Data.DbModels.ProductSchema.Instrument>(instrumentDto);

                // Set relation variables with null to avoid unexpected EF errors
                instrument.ReagentSystem = null;
                instrument.TestingArea = null;
                instrument.ThroughPutUnit = null;
                instrument.Vendor = null;
                instrument.ControlRequirementUnit = null;

                _instrumentRepository.Update(instrument);

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
        public async Task<IResponseDTO> UpdateIsActive(int instrumentId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var instrument = await _instrumentRepository.GetFirstAsync(x => x.Id == instrumentId);
                if (instrument == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && instrument.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                instrument.IsActive = IsActive;
                instrument.UpdatedBy = LoggedInUserId;
                instrument.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                instrument.LaboratoryInstruments = null;
                instrument.ProductUsages = null;
                instrument.ForecastInstruments = null;

                // Update on the Database
                _instrumentRepository.Update(instrument);

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
                var items = _instrumentRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _instrumentRepository.Update(item);
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
                var items = _instrumentRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _instrumentRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveInstrument(int instrumentId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var instrument = await _instrumentRepository.GetFirstOrDefaultAsync(x => x.Id == instrumentId);
                if (instrument == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && instrument.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                instrument.IsDeleted = true;
                instrument.UpdatedBy = LoggedInUserId;
                instrument.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                instrument.LaboratoryInstruments = null;
                instrument.ProductUsages = null;
                instrument.ForecastInstruments = null;
                // Update on the Database
                _instrumentRepository.Update(instrument);

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
        public async Task<IResponseDTO> ImportInstruments(List<InstrumentDto> instrumentDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var instruments_database = _instrumentRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var instruments = _mapper.Map<List<Data.DbModels.ProductSchema.Instrument>>(instrumentDtos);

                // vars
                var names_database = instruments_database.Select(x => x.Name.ToLower().Trim());
                var names_dto = instruments.Select(x => x.Name.ToLower().Trim());
                // Get the new ones that their names don't exist on the database
                var newInstruments = instruments.Where(x => !names_database.Contains(x.Name.ToLower().Trim()));
                // Select the objects that their names already exist in the database
                var updatedInstruments = instruments_database.Where(x => names_dto.Contains(x.Name.ToLower().Trim()));


                // Add the new object to the database
                if (newInstruments.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newInstruments.Select(x =>
                    {
                        x.LaboratoryInstruments = null;
                        x.ProductUsages = null;
                        x.ForecastInstruments = null;
                        x.Vendor = null;
                        x.ThroughPutUnit = null;
                        x.ReagentSystem = null;
                        x.ControlRequirementUnit = null;
                        x.TestingArea = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();

                    await _instrumentRepository.AddRangeAsync(newInstruments);
                }

                // Update the existing objects with the new values
                if (updatedInstruments.Count() > 0)
                {
                    foreach (var item in updatedInstruments)
                    {
                        var dto = instrumentDtos.First(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        item.UpdatedOn = DateTime.Now;
                        item.UpdatedBy = dto.CreatedBy;
                        item.Name = dto.Name;
                        item.VendorId = dto.VendorId;
                        item.MaxThroughPut = dto.MaxThroughPut;
                        item.ThroughPutUnitId = dto.ThroughPutUnitId;
                        item.ReagentSystemId = dto.ReagentSystemId;
                        item.ControlRequirement = dto.ControlRequirement;
                        item.ControlRequirementUnitId = dto.ControlRequirementUnitId;
                        item.TestingAreaId = dto.TestingAreaId;
                        // Set relation variables with null to avoid unexpected EF errors
                        item.LaboratoryInstruments = null;
                        item.ProductUsages = null;
                        item.ForecastInstruments = null;
                        item.Vendor = null;
                        item.ThroughPutUnit = null;
                        item.ReagentSystem = null;
                        item.ControlRequirementUnit = null;
                        item.TestingArea = null;
                        item.Creator = null;
                        item.Updator = null;
                        _instrumentRepository.Update(item);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newInstruments.Count();
                var numberOfUpdated = updatedInstruments.Count();

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
                    NumberOfUploded = instrumentDtos.Count,
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
        public GeneratedFile ExportInstruments(int? pageIndex = null, int? pageSize = null, InstrumentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.Instrument> query = null;
            try
            {
                query = _instrumentRepository.GetAll()
                                    .Include(x => x.Vendor)
                                    .Include(x => x.ThroughPutUnit)
                                    .Include(x => x.TestingArea)
                                    .Include(x => x.ReagentSystem)
                                    .Include(x => x.ControlRequirementUnit)
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
                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (filterDto.ThroughPutUnitId > 0)
                    {
                        query = query.Where(x => x.ThroughPutUnitId == filterDto.ThroughPutUnitId);
                    }
                    if (filterDto.TestingAreaId > 0)
                    {
                        query = query.Where(x => x.TestingAreaId == filterDto.TestingAreaId);
                    }
                    if (filterDto.ReagentSystemId > 0)
                    {
                        query = query.Where(x => x.ReagentSystemId == filterDto.ReagentSystemId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.MaxThroughPut > 0)
                    {
                        query = query.Where(x => x.MaxThroughPut == filterDto.MaxThroughPut);
                    }
                    if (filterDto.ControlRequirementUnitId > 0)
                    {
                        query = query.Where(x => x.ControlRequirementUnitId == filterDto.ControlRequirementUnitId);
                    }
                    if (filterDto.ControlRequirement > 0)
                    {
                        query = query.Where(x => x.ControlRequirement == filterDto.ControlRequirement);
                    }
                }

                query = query.OrderByDescending(x => x.Name);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "VendorName".ToLower())
                    {
                        filterDto.SortProperty = "VendorId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ThroughPutUnitName".ToLower())
                    {
                        filterDto.SortProperty = "ThroughPutUnitId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "TestingAreaName".ToLower())
                    {
                        filterDto.SortProperty = "TestingAreaId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ReagentSystemName".ToLower())
                    {
                        filterDto.SortProperty = "ReagentSystemId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ControlRequirementUnitName".ToLower())
                    {
                        filterDto.SortProperty = "ControlRequirementUnitId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportInstrumentDto>>(query.ToList());
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
        public bool IsNameUnique(InstrumentDto instrumentDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _instrumentRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != instrumentDto.Id
                                                && x.Name.ToLower().Trim() == instrumentDto.Name.ToLower().Trim());
            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => createdBy.Contains(x.CreatedBy));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int instrumentId)
        {
            try
            {
                var instrument = await _instrumentRepository.GetAll()
                                        .Include(x => x.LaboratoryInstruments)
                                        .Include(x => x.ForecastInstruments)
                                        .Include(x => x.ProductUsages)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == instrumentId);
                if (instrument == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (instrument.LaboratoryInstruments.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Instrument cannot be deleted or deactivated where it contains 'Laboratory'";
                    return _response;
                }
                if (instrument.ForecastInstruments.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Instrument cannot be deleted or deactivated where it contains 'Forecast'";
                    return _response;
                }
                if (instrument.ProductUsages.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Instrument cannot be deleted or deactivated where it contains 'Product Usages'";
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
