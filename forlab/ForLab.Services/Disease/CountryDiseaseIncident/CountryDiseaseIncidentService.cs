using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Disease.CountryDiseaseIncident;
using ForLab.Repositories.Disease.CountryDiseaseIncident;
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

namespace ForLab.Services.Disease.CountryDiseaseIncident
{
    public class CountryDiseaseIncidentService : GService<CountryDiseaseIncidentDto, Data.DbModels.DiseaseSchema.CountryDiseaseIncident, ICountryDiseaseIncidentRepository>, ICountryDiseaseIncidentService
    {
        private readonly ICountryDiseaseIncidentRepository _countryDiseaseIncidentRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportCountryDiseaseIncidentDto> _fileService;

        public CountryDiseaseIncidentService(IMapper mapper,
            IResponseDTO response,
            ICountryDiseaseIncidentRepository countryDiseaseIncidentRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportCountryDiseaseIncidentDto> fileService) : base(countryDiseaseIncidentRepository, mapper)
        {
            _countryDiseaseIncidentRepository = countryDiseaseIncidentRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, CountryDiseaseIncidentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseSchema.CountryDiseaseIncident> query = null;
            try
            {
                query = _countryDiseaseIncidentRepository.GetAll()
                                    .Include(x => x.Disease)
                                    .Include(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.Year > 0)
                    {
                        query = query.Where(x => x.Year == filterDto.Year);
                    }
                    if (filterDto.Incidence > 0)
                    {
                        query = query.Where(x => x.Incidence == filterDto.Incidence);
                    }
                    if (filterDto.IncidencePer1kPopulation > 0)
                    {
                        query = query.Where(x => x.IncidencePer1kPopulation == filterDto.IncidencePer1kPopulation);
                    }
                    if (filterDto.IncidencePer100kPopulation > 0)
                    {
                        query = query.Where(x => x.IncidencePer100kPopulation == filterDto.IncidencePer100kPopulation);
                    }
                    if (filterDto.PrevalenceRate > 0)
                    {
                        query = query.Where(x => x.PrevalenceRate == filterDto.PrevalenceRate);
                    }
                    if (filterDto.PrevalenceRatePer1kPopulation > 0)
                    {
                        query = query.Where(x => x.PrevalenceRatePer1kPopulation == filterDto.PrevalenceRatePer1kPopulation);
                    }
                    if (filterDto.PrevalenceRatePer100kPopulation > 0)
                    {
                        query = query.Where(x => x.PrevalenceRatePer100kPopulation == filterDto.PrevalenceRatePer100kPopulation);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<CountryDiseaseIncidentDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(CountryDiseaseIncidentFilterDto filterDto = null)
        {
            var query = _countryDiseaseIncidentRepository.GetAll(x => !x.IsDeleted && x.IsActive);


            if (filterDto != null)
            {
                if (filterDto.IsActive != null)
                {
                    query = query.Where(x => x.IsActive == filterDto.IsActive);
                }
                if (filterDto.DiseaseId > 0)
                {
                    query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                }
                if (filterDto.CountryId > 0)
                {
                    query = query.Where(x => x.CountryId == filterDto.CountryId);
                }
                if (filterDto.Year > 0)
                {
                    query = query.Where(x => x.Year == filterDto.Year);
                }
                if (filterDto.Incidence > 0)
                {
                    query = query.Where(x => x.Incidence == filterDto.Incidence);
                }
                if (filterDto.IncidencePer1kPopulation > 0)
                {
                    query = query.Where(x => x.IncidencePer1kPopulation == filterDto.IncidencePer1kPopulation);
                }
                if (filterDto.IncidencePer100kPopulation > 0)
                {
                    query = query.Where(x => x.IncidencePer100kPopulation == filterDto.IncidencePer100kPopulation);
                }
                if (filterDto.PrevalenceRate > 0)
                {
                    query = query.Where(x => x.PrevalenceRate == filterDto.PrevalenceRate);
                }
                if (filterDto.PrevalenceRatePer1kPopulation > 0)
                {
                    query = query.Where(x => x.PrevalenceRatePer1kPopulation == filterDto.PrevalenceRatePer1kPopulation);
                }
                if (filterDto.PrevalenceRatePer100kPopulation > 0)
                {
                    query = query.Where(x => x.PrevalenceRatePer100kPopulation == filterDto.PrevalenceRatePer100kPopulation);
                }
            }

            query = query.OrderBy(x => x.Id);
            query = query.Select(i => new Data.DbModels.DiseaseSchema.CountryDiseaseIncident()
            {
                Id = i.Id,
                Year = i.Year,
                Country = i.Country,
                Disease = i.Disease
            });
            var dataList = _mapper.Map<List<CountryDiseaseIncidentDrp>>(query.ToList());

            _response.Data = dataList;
            _response.IsPassed = true;
            _response.Message = "Done";
            return _response;
        }
        public async Task<IResponseDTO> GetCountryDiseaseIncidentDetails(int countryDiseaseIncidentId)
        {
            try
            {
                var countryDiseaseIncident = await _countryDiseaseIncidentRepository.GetAll()
                                        .Include(x => x.CountryId)
                                        .Include(x => x.DiseaseId)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == countryDiseaseIncidentId);
                if (countryDiseaseIncident == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var countryDiseaseIncidentDto = _mapper.Map<CountryDiseaseIncidentDto>(countryDiseaseIncident);

                _response.Data = countryDiseaseIncidentDto;
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
        public GeneratedFile ExportCountryDiseaseIncidents(int? pageIndex = null, int? pageSize = null, CountryDiseaseIncidentFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseSchema.CountryDiseaseIncident> query = null;
            try
            {
                query = _countryDiseaseIncidentRepository.GetAll()
                                    .Include(x => x.Disease)
                                    .Include(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.DiseaseId > 0)
                    {
                        query = query.Where(x => x.DiseaseId == filterDto.DiseaseId);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.Year > 0)
                    {
                        query = query.Where(x => x.Year == filterDto.Year);
                    }
                    if (filterDto.Incidence > 0)
                    {
                        query = query.Where(x => x.Incidence == filterDto.Incidence);
                    }
                    if (filterDto.IncidencePer1kPopulation > 0)
                    {
                        query = query.Where(x => x.IncidencePer1kPopulation == filterDto.IncidencePer1kPopulation);
                    }
                    if (filterDto.IncidencePer100kPopulation > 0)
                    {
                        query = query.Where(x => x.IncidencePer100kPopulation == filterDto.IncidencePer100kPopulation);
                    }
                    if (filterDto.PrevalenceRate > 0)
                    {
                        query = query.Where(x => x.PrevalenceRate == filterDto.PrevalenceRate);
                    }
                    if (filterDto.PrevalenceRatePer1kPopulation > 0)
                    {
                        query = query.Where(x => x.PrevalenceRatePer1kPopulation == filterDto.PrevalenceRatePer1kPopulation);
                    }
                    if (filterDto.PrevalenceRatePer100kPopulation > 0)
                    {
                        query = query.Where(x => x.PrevalenceRatePer100kPopulation == filterDto.PrevalenceRatePer100kPopulation);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                
                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    if (filterDto.SortProperty.ToLower() == "DiseaseName".ToLower())
                    {
                        filterDto.SortProperty = "DiseaseId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }
                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportCountryDiseaseIncidentDto>>(query.ToList());

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
        public async Task<IResponseDTO> CreateCountryDiseaseIncident(CountryDiseaseIncidentDto countryDiseaseIncidentDto)
        {
            try
            {
                var countryDiseaseIncident = _mapper.Map<Data.DbModels.DiseaseSchema.CountryDiseaseIncident>(countryDiseaseIncidentDto);

                // Set relation variables with null to avoid unexpected EF errors
                countryDiseaseIncident.Disease = null;
                countryDiseaseIncident.Country = null;

                // Add to the DB
                await _countryDiseaseIncidentRepository.AddAsync(countryDiseaseIncident);

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
        public async Task<IResponseDTO> UpdateCountryDiseaseIncident(CountryDiseaseIncidentDto countryDiseaseIncidentDto)
        {
            try
            {
                var countryDiseaseIncidentExist = await _countryDiseaseIncidentRepository.GetFirstAsync(x => x.Id == countryDiseaseIncidentDto.Id);
                if (countryDiseaseIncidentExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var countryDiseaseIncident = _mapper.Map<Data.DbModels.DiseaseSchema.CountryDiseaseIncident>(countryDiseaseIncidentDto);

                // Set relation variables with null to avoid unexpected EF errors

                countryDiseaseIncident.Disease = null;
                countryDiseaseIncident.Country = null;


                _countryDiseaseIncidentRepository.Update(countryDiseaseIncident);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int countryDiseaseIncidentId, bool IsActive)
        {
            try
            {
                var countryDiseaseIncident = await _countryDiseaseIncidentRepository.GetFirstAsync(x => x.Id == countryDiseaseIncidentId);
                if (countryDiseaseIncident == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                countryDiseaseIncident.IsActive = IsActive;
                countryDiseaseIncident.UpdatedBy = loggedInUserId;
                countryDiseaseIncident.UpdatedOn = DateTime.Now;

                // Update on the Database
                _countryDiseaseIncidentRepository.Update(countryDiseaseIncident);

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
        public async Task<IResponseDTO> UpdateIsActiveForSelected(int loggedInUserId, List<int> ids, bool isActive)
        {
            try
            {
                var items = _countryDiseaseIncidentRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.IsActive != isActive).Select(x =>
                {
                    x.IsActive = isActive;
                    x.UpdatedBy = loggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach (var item in objectsToUpdate)
                {
                    _countryDiseaseIncidentRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveCountryDiseaseIncident(int countryDiseaseIncidentId, int loggedInUserId)
        {
            try
            {
                var countryDiseaseIncident = await _countryDiseaseIncidentRepository.GetFirstOrDefaultAsync(x => x.Id == countryDiseaseIncidentId);
                if (countryDiseaseIncident == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                countryDiseaseIncident.IsDeleted = true;
                countryDiseaseIncident.UpdatedBy = loggedInUserId;
                countryDiseaseIncident.UpdatedOn = DateTime.Now;

                // Update on the Database
                _countryDiseaseIncidentRepository.Update(countryDiseaseIncident);

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
        public async Task<IResponseDTO> ImportCountryDiseaseIncidents(List<CountryDiseaseIncidentDto> countryDiseaseIncidentDtos)
        {
            try
            {
                // Get all not deleted from the database
                var countryDiseaseIncidents_database = _countryDiseaseIncidentRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var countryDiseaseIncidents = _mapper.Map<List<Data.DbModels.DiseaseSchema.CountryDiseaseIncident>>(countryDiseaseIncidentDtos);

                // vars
                var newCountryDiseases = new List<Data.DbModels.DiseaseSchema.CountryDiseaseIncident>();
                var updatedCountryDiseases = new List<Data.DbModels.DiseaseSchema.CountryDiseaseIncident>();

                // Get new and updated laboratoryInstruments
                foreach (var item in countryDiseaseIncidents)
                {
                    var foundCountryDisease = countryDiseaseIncidents_database.FirstOrDefault(x => x.CountryId == item.CountryId
                                                                                                && x.DiseaseId == item.DiseaseId
                                                                                                && x.Year == item.Year);
                    if (foundCountryDisease == null)
                    {
                        newCountryDiseases.Add(item);
                    }
                    else
                    {
                        item.Id = foundCountryDisease.Id;
                        updatedCountryDiseases.Add(item);
                    }
                }

                // Add the new object to the database
                if (newCountryDiseases.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newCountryDiseases.Select(x =>
                    {
                        x.Country = null;
                        x.Disease = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _countryDiseaseIncidentRepository.AddRangeAsync(newCountryDiseases);
                }

                // Update the existing objects with the new values
                if (updatedCountryDiseases.Count() > 0)
                {
                    foreach (var item in updatedCountryDiseases)
                    {
                        var fromDatabase = countryDiseaseIncidents_database.FirstOrDefault(x => x.CountryId == item.CountryId
                                                                                        && x.DiseaseId == item.DiseaseId
                                                                                        && x.Year == item.Year);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.CountryId = item.CountryId;
                        fromDatabase.DiseaseId = item.DiseaseId;
                        fromDatabase.Year = item.Year;
                        fromDatabase.Incidence = item.Incidence;
                        fromDatabase.IncidencePer1kPopulation = item.IncidencePer1kPopulation;
                        fromDatabase.IncidencePer100kPopulation = item.IncidencePer100kPopulation;
                        fromDatabase.PrevalenceRate = item.PrevalenceRate;
                        fromDatabase.PrevalenceRatePer1kPopulation = item.PrevalenceRatePer1kPopulation;
                        fromDatabase.PrevalenceRatePer100kPopulation = item.PrevalenceRatePer100kPopulation;
                        fromDatabase.Note = item.Note;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Country = null;
                        fromDatabase.Disease = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _countryDiseaseIncidentRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newCountryDiseases.Count();
                var numberOfUpdated = updatedCountryDiseases.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = countryDiseaseIncidentDtos.Count,
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
        public bool IsIncidentUnique(CountryDiseaseIncidentDto countryDiseaseIncidentDto)
        {
            var searchResult = _countryDiseaseIncidentRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != countryDiseaseIncidentDto.Id
                                                && x.CountryId == countryDiseaseIncidentDto.CountryId
                                                && x.DiseaseId == countryDiseaseIncidentDto.DiseaseId
                                                && x.Year == countryDiseaseIncidentDto.Year);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
