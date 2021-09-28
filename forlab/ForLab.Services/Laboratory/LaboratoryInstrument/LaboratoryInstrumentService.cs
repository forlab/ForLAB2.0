using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Laboratory.LaboratoryInstrument;
using ForLab.Repositories.Laboratory.LaboratoryInstrument;
using ForLab.Repositories.Lookup.Laboratory;
using ForLab.Repositories.Product.Instrument;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.DataFilter;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryInstrument
{
    public class LaboratoryInstrumentService : GService<LaboratoryInstrumentDto, Data.DbModels.LaboratorySchema.LaboratoryInstrument, ILaboratoryInstrumentRepository>, ILaboratoryInstrumentService
    {
        private readonly ILaboratoryInstrumentRepository _laboratoryInstrumentRepository;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ILaboratoryRepository _laboratoryRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IDataFilterService<Data.DbModels.LaboratorySchema.LaboratoryInstrument> _dataFilterService;
        private readonly IFileService<ExportLaboratoryInstrumentDto> _fileService;
        private readonly IGeneralService _generalService;
        public LaboratoryInstrumentService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryInstrumentRepository laboratoryInstrumentRepository,
            IInstrumentRepository instrumentRepository,
            ILaboratoryRepository laboratoryRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IDataFilterService<Data.DbModels.LaboratorySchema.LaboratoryInstrument> dataFilterService,
            IFileService<ExportLaboratoryInstrumentDto> fileService,
            IGeneralService generalService) : base(laboratoryInstrumentRepository, mapper)
        {
            _laboratoryInstrumentRepository = laboratoryInstrumentRepository;
            _instrumentRepository = instrumentRepository;
            _laboratoryRepository = laboratoryRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _dataFilterService = dataFilterService;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryInstrumentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryInstrument> query = null;
            try
            {
                query = _laboratoryInstrumentRepository.GetAll()
                                    .Include(x => x.Instrument)
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
                    if (filterDto.InstrumentId > 0)
                    {
                        query = query.Where(x => x.InstrumentId == filterDto.InstrumentId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.Quantity>0)
                    {
                        query = query.Where(x => x.Quantity == filterDto.Quantity);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "InstrumentName".ToLower())
                    {
                        filterDto.SortProperty = "InstrumentId";
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

                var dataList = _mapper.Map<List<LaboratoryInstrumentDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryInstrumentFilterDto filterDto = null)
        {
            try 
            { 
                var query = _laboratoryInstrumentRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                if (filterDto.InstrumentId > 0)
                {
                    query = query.Where(x => x.InstrumentId == filterDto.InstrumentId);
                }
                if (filterDto.LaboratoryId > 0)
                {
                    query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                }
                if (filterDto.Quantity>0)
                {
                    query = query.Where(x => x.Quantity == filterDto.Quantity);
                }
            }

            query = query.Select(i => new Data.DbModels.LaboratorySchema.LaboratoryInstrument()
            {
                Id = i.Id ,
                Instrument = i.Instrument ,
                Laboratory = i.Laboratory
            });
            query = query.OrderBy(x => x.InstrumentId);
            var dataList = _mapper.Map<List<LaboratoryInstrumentDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryInstrumentDetails(int laboratoryInstrumentId)
        {
            try
            {
                var laboratoryInstrument = await _laboratoryInstrumentRepository.GetAll()
                                        .Include(x => x.Instrument)
                                        .Include(x => x.Laboratory)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryInstrumentId);
                if (laboratoryInstrument == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryInstrumentDto = _mapper.Map<LaboratoryInstrumentDto>(laboratoryInstrument);

                _response.Data = laboratoryInstrumentDto;
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
        public async Task<IResponseDTO> CreateLaboratoryInstrument(LaboratoryInstrumentDto laboratoryInstrumentDto)
        {
            try
            {
                var laboratoryInstrument = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryInstrument>(laboratoryInstrumentDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryInstrument.Instrument = null;
                laboratoryInstrument.Laboratory = null;

                // Add to the DB
                await _laboratoryInstrumentRepository.AddAsync(laboratoryInstrument);

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
        public async Task<IResponseDTO> UpdateLaboratoryInstrument(LaboratoryInstrumentDto laboratoryInstrumentDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryInstrumentExist = await _laboratoryInstrumentRepository.GetFirstAsync(x => x.Id == laboratoryInstrumentDto.Id);
                if (laboratoryInstrumentExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryInstrumentExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryInstrument = _mapper.Map<Data.DbModels.LaboratorySchema.LaboratoryInstrument>(laboratoryInstrumentDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryInstrument.Laboratory = null;
                laboratoryInstrument.Instrument = null;

                _laboratoryInstrumentRepository.Update(laboratoryInstrument);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryInstrumentId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryInstrument = await _laboratoryInstrumentRepository.GetFirstAsync(x => x.Id == laboratoryInstrumentId);
                if (laboratoryInstrument == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryInstrument.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryInstrument.IsActive = IsActive;
                laboratoryInstrument.UpdatedBy = LoggedInUserId;
                laboratoryInstrument.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors

                // Update on the Database
                _laboratoryInstrumentRepository.Update(laboratoryInstrument);

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
        public async Task<IResponseDTO> RemoveLaboratoryInstrument(int laboratoryInstrumentId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryInstrument = await _laboratoryInstrumentRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryInstrumentId);
                if (laboratoryInstrument == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryInstrument.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryInstrument.IsDeleted = true;
                laboratoryInstrument.UpdatedBy = LoggedInUserId;
                laboratoryInstrument.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryInstrumentRepository.Update(laboratoryInstrument);

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
        public GeneratedFile ExportLaboratoryInstruments(int? pageIndex = null, int? pageSize = null, LaboratoryInstrumentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LaboratorySchema.LaboratoryInstrument> query = null;
            try
            {
                query = _laboratoryInstrumentRepository.GetAll()
                                    .Include(x => x.Instrument)
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
                    if (filterDto.InstrumentId > 0)
                    {
                        query = query.Where(x => x.InstrumentId == filterDto.InstrumentId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.Quantity > 0)
                    {
                        query = query.Where(x => x.Quantity == filterDto.Quantity);
                    }
                }
                query = query.OrderByDescending(x => x.InstrumentId);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "InstrumentName".ToLower())
                    {
                        filterDto.SortProperty = "InstrumentId";
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

                var dataList = _mapper.Map<List<ExportLaboratoryInstrumentDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportLaboratoryInstruments(List<LaboratoryInstrumentDto> laboratoryInstrumentDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryInstruments_database = _laboratoryInstrumentRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryInstruments = _mapper.Map<List<Data.DbModels.LaboratorySchema.LaboratoryInstrument>>(laboratoryInstrumentDtos);

                // vars
                var newLaboratoryInstruments = new List<Data.DbModels.LaboratorySchema.LaboratoryInstrument>();
                var updatedLaboratoryInstruments = new List<Data.DbModels.LaboratorySchema.LaboratoryInstrument>();

                // Get new and updated laboratoryInstruments
                foreach (var item in laboratoryInstruments)
                {
                    var foundLaboratoryInstrument = laboratoryInstruments_database.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId
                                                                                                && x.InstrumentId == item.InstrumentId);
                    if (foundLaboratoryInstrument == null)
                    {
                        newLaboratoryInstruments.Add(item);
                    }
                    else
                    {
                        item.Id = foundLaboratoryInstrument.Id;
                        updatedLaboratoryInstruments.Add(item);
                    }
                }

                // Validate sum of percentage
                var updatedIds = updatedLaboratoryInstruments.Select(x => x.Id);
                var res = CheckImportValidPercentage(laboratoryInstrumentDtos, laboratoryInstruments_database.Where(x => !updatedIds.Contains(x.Id)).ToList());
                if(!res.IsPassed)
                {
                    return res;
                }

                // Add the new object to the database
                if (newLaboratoryInstruments.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newLaboratoryInstruments.Select(x =>
                    {
                        x.Instrument = null;
                        x.Laboratory = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _laboratoryInstrumentRepository.AddRangeAsync(newLaboratoryInstruments);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryInstruments.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryInstruments)
                    {
                        var fromDatabase = laboratoryInstruments_database.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId
                                                                                        && x.InstrumentId == item.InstrumentId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.InstrumentId = item.InstrumentId;
                        fromDatabase.LaboratoryId = item.LaboratoryId;
                        fromDatabase.Quantity = item.Quantity;
                        fromDatabase.TestRunPercentage = item.TestRunPercentage;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Instrument = null;
                        fromDatabase.Laboratory = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _laboratoryInstrumentRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryInstruments.Count();
                var numberOfUpdated = updatedLaboratoryInstruments.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = laboratoryInstrumentDtos.Count,
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
        public bool IsLaboratoryInstrumentUnique(LaboratoryInstrumentDto laboratoryInstrumentDto)
        {
            var searchResult = _laboratoryInstrumentRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryInstrumentDto.Id
                                                && x.LaboratoryId == laboratoryInstrumentDto.LaboratoryId
                                                && x.InstrumentId == laboratoryInstrumentDto.InstrumentId);
                                                
            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsValidPercentage(LaboratoryInstrumentDto laboratoryInstrumentDto)
        {
            var testingAreaId = _instrumentRepository.GetFirstOrDefault(x => x.Id == laboratoryInstrumentDto.InstrumentId)?.TestingAreaId;
            var sumOfTestRunPercentage = _laboratoryInstrumentRepository.GetAll().Include(x => x.Instrument)
                                            .Where(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryInstrumentDto.Id
                                                && x.LaboratoryId == laboratoryInstrumentDto.LaboratoryId
                                                && x.Instrument.TestingAreaId == testingAreaId).Sum(x => x.TestRunPercentage);

            if ((sumOfTestRunPercentage + laboratoryInstrumentDto.TestRunPercentage) > 100)
            {
                return false;
            }

            return true;
        }
        private IResponseDTO CheckImportValidPercentage(List<LaboratoryInstrumentDto> laboratoryInstrumentDtos, List<Data.DbModels.LaboratorySchema.LaboratoryInstrument> laboratoryInstruments)
        {
            // Collect all in One place
            var allData = laboratoryInstrumentDtos;
            allData = allData.Concat(_mapper.Map<List<LaboratoryInstrumentDto>>(laboratoryInstruments)).ToList();
            // Ids
            var labIds = allData.Select(x => x.LaboratoryId).Distinct();
            var instrumentIds = allData.Select(x => x.InstrumentId).Distinct();
            // Load data from DB
            var instruments = _instrumentRepository.GetAll(x => !x.IsDeleted && instrumentIds.Contains(x.Id)).ToList();
            var laboratories = _laboratoryRepository.GetAll(x => !x.IsDeleted && labIds.Contains(x.Id)).ToList();

            foreach (var labId in labIds)
            {
                var labInstrumentDtos = allData.Where(x => x.LaboratoryId == labId);
                var insIds = labInstrumentDtos.Select(x => x.InstrumentId).Distinct();
                var testingAreaIds = instruments.Where(x => insIds.Contains(x.Id)).Select(x => x.TestingAreaId).Distinct();
                foreach (var testingAreaId in testingAreaIds)
                {
                    var insIds1 = instruments.Where(x => x.TestingAreaId == testingAreaId).Select(x => x.Id).Distinct();
                    var sum = labInstrumentDtos.Where(x => insIds1.Contains(x.InstrumentId)).Sum(x => x.TestRunPercentage);
                    if(sum > 100)
                    {
                        _response.IsPassed = false;
                        _response.Message = $"Sum of all instrument test percentage must be 100% per testing area for laboratory '{laboratories?.First(x => x.Id == labId)?.Name}'";
                        return _response;
                    }
                }
            }

            _response.IsPassed = true;
            return _response;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryInstrumentId)
        {
            try
            {
                var laboratoryInstrument = await _laboratoryInstrumentRepository.GetAll()
                                        .Include(x => x.Instrument)
                                        .Include(x => x.Laboratory)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryInstrumentId);
                if (laboratoryInstrument == null)
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
