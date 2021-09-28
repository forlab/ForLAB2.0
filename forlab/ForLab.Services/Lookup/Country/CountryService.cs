using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.Country;
using ForLab.Repositories.Lookup.Country;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.Country
{
    public class CountryService : GService<CountryDto, Data.DbModels.LookupSchema.Country, ICountryRepository>, ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportCountryDto> _fileService;

        public CountryService(IMapper mapper,
            IResponseDTO response,
            ICountryRepository countryRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportCountryDto> fileService) : base(countryRepository, mapper)
        {
            _countryRepository = countryRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, CountryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.Country> query = null;
            try
            {
                query = _countryRepository.GetAll()
                                    .Include(x => x.Continent)
                                    .Include(x => x.CountryPeriod)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ContinentId > 0)
                    {
                        query = query.Where(x => x.ContinentId == filterDto.ContinentId);
                    }
                    if (filterDto.CountryPeriodId > 0)
                    {
                        query = query.Where(x => x.CountryPeriodId == filterDto.CountryPeriodId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortCode))
                    {
                        query = query.Where(x => x.ShortCode.Trim().ToLower().Contains(filterDto.ShortCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.NativeName))
                    {
                        query = query.Where(x => x.NativeName.Trim().ToLower().Contains(filterDto.NativeName.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CurrencyCode))
                    {
                        query = query.Where(x => x.CurrencyCode.Trim().ToLower().Contains(filterDto.CurrencyCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CallingCode))
                    {
                        query = query.Where(x => x.CallingCode.Trim().ToLower().Contains(filterDto.CallingCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Latitude))
                    {
                        query = query.Where(x => x.Latitude.Trim().ToLower().Contains(filterDto.Latitude.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Longitude))
                    {
                        query = query.Where(x => x.Longitude.Trim().ToLower().Contains(filterDto.Longitude.Trim().ToLower()));
                    }
                    if (filterDto.Population != null)
                    {
                        query = query.Where(x => x.Population == filterDto.Population);
                    }
                }

                query = query.OrderByDescending(x => x.Id).ThenBy(x => x.Name);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ContinentName".ToLower())
                    {
                        filterDto.SortProperty = "ContinentId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "CountryPeriodName".ToLower())
                    {
                        filterDto.SortProperty = "CountryPeriodId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<CountryDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(CountryFilterDto filterDto = null)
        {
            try
            {
                var query = _countryRepository.GetAll(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ContinentId > 0)
                    {
                        query = query.Where(x => x.ContinentId == filterDto.ContinentId);
                    }
                    if (filterDto.CountryPeriodId > 0)
                    {
                        query = query.Where(x => x.CountryPeriodId == filterDto.CountryPeriodId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortCode))
                    {
                        query = query.Where(x => x.ShortCode.Trim().ToLower().Contains(filterDto.ShortCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.NativeName))
                    {
                        query = query.Where(x => x.NativeName.Trim().ToLower().Contains(filterDto.NativeName.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CurrencyCode))
                    {
                        query = query.Where(x => x.CurrencyCode.Trim().ToLower().Contains(filterDto.CurrencyCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CallingCode))
                    {
                        query = query.Where(x => x.CallingCode.Trim().ToLower().Contains(filterDto.CallingCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Latitude))
                    {
                        query = query.Where(x => x.Latitude.Trim().ToLower().Contains(filterDto.Latitude.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Longitude))
                    {
                        query = query.Where(x => x.Longitude.Trim().ToLower().Contains(filterDto.Longitude.Trim().ToLower()));
                    }
                    if (filterDto.Population != null)
                    {
                        query = query.Where(x => x.Population == filterDto.Population);
                    }
                }

                query = query.Select(i => new Data.DbModels.LookupSchema.Country() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<CountryDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetCountryDetails(int countryId)
        {
            try
            {
                var country = await _countryRepository.GetAll()
                                        .Include(x => x.Continent)
                                        .Include(x => x.CountryPeriod)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == countryId);
                if (country == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var countryDto = _mapper.Map<CountryDto>(country);

                _response.Data = countryDto;
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
        public async Task<IResponseDTO> CreateCountry(CountryDto countryDto)
        {
            try
            {
                var country = _mapper.Map<Data.DbModels.LookupSchema.Country>(countryDto);

                // Set relation variables with null to avoid unexpected EF errors
                country.Continent = null;
                country.CountryPeriod = null;

                // Add to the DB
                await _countryRepository.AddAsync(country);

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
        public async Task<IResponseDTO> UpdateCountry(CountryDto countryDto)
        {
            try
            {
                var countryExist = await _countryRepository.GetFirstAsync(x => x.Id == countryDto.Id);
                if (countryExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var country = _mapper.Map<Data.DbModels.LookupSchema.Country>(countryDto);

                // Set relation variables with null to avoid unexpected EF errors
                country.Continent = null;
                country.CountryPeriod = null;

                _countryRepository.Update(country);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int countryId, bool IsActive)
        {
            try
            {
                var country = await _countryRepository.GetFirstAsync(x => x.Id == countryId);
                if (country == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                country.IsActive = IsActive;
                country.UpdatedBy = loggedInUserId;
                country.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                country.Regions = null;
                country.CountryDiseaseIncidents = null;
                country.CountryProductPrices = null;

                // Update on the Database
                _countryRepository.Update(country);

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
                var items = _countryRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _countryRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveCountry(int countryId, int loggedInUserId)
        {
            try
            {
                var country = await _countryRepository.GetFirstOrDefaultAsync(x => x.Id == countryId);
                if (country == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                country.IsDeleted = true;
                country.UpdatedBy = loggedInUserId;
                country.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                country.Regions = null;
                country.CountryDiseaseIncidents = null;
                country.CountryProductPrices = null;
                country.UserCountrySubscribtions = null;

                // Update on the Database
                _countryRepository.Update(country);

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
        public async Task<IResponseDTO> ImportCountries(List<CountryDto> countryDtos)
        {
            try
            {
                // Get all not deleted from the database
                var countries_database = _countryRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var countries = _mapper.Map<List<Data.DbModels.LookupSchema.Country>>(countryDtos);

                // vars
                var codes_database = countries_database.Select(x => x.ShortCode.ToLower().Trim());
                var names_database = countries_database.Select(x => x.Name.ToLower().Trim());

                // Get the new ones that their names don't exist on the database
                var newCountries = countries.Where(x => !names_database.Contains(x.Name.ToLower().Trim())
                                                     && !codes_database.Contains(x.ShortCode.ToLower().Trim()));

                // Get Skipped
                var skipped = countries.Where(x => ImportSkipped(countries_database, x)).ToList();

                // Select the objects that their names already exist in the database
                var updatedCountries = countries.Except(skipped).Except(newCountries);

                // Set relation variables with null to avoid unexpected EF errors
                newCountries.Select(x =>
                {
                    x.Regions = null;
                    x.CountryDiseaseIncidents = null;
                    x.CountryProductPrices = null;
                    x.UserCountrySubscribtions = null;
                    x.Continent = null;
                    x.CountryPeriod = null;
                    x.Creator = null;
                    x.Updator = null;
                    return x;
                }).ToList();

                // Add the new object to the database
                if (newCountries.Count() > 0)
                {
                    await _countryRepository.AddRangeAsync(newCountries);
                }

                // Update the existing objects with the new values
                if (updatedCountries.Count() > 0)
                {
                    foreach (var item in updatedCountries)
                    {
                        var fromDatabase = countries_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                              || x.ShortCode.ToLower().Trim() == item.ShortCode?.ToLower()?.Trim());
                        if (fromDatabase == null)
                        {
                            continue;
                        }
                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.ShortCode = item.ShortCode;
                        fromDatabase.ContinentId = item.ContinentId;
                        fromDatabase.CountryPeriodId = item.CountryPeriodId;
                        fromDatabase.ShortName = item.ShortName;
                        fromDatabase.NativeName = item.NativeName;
                        fromDatabase.CurrencyCode = item.CurrencyCode;
                        fromDatabase.CallingCode = item.CallingCode;
                        fromDatabase.Latitude = item.Latitude;
                        fromDatabase.Longitude = item.Longitude;
                        fromDatabase.Population = item.Population;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Regions = null;
                        fromDatabase.CountryDiseaseIncidents = null;
                        fromDatabase.CountryProductPrices = null;
                        fromDatabase.UserCountrySubscribtions = null;
                        fromDatabase.Continent = null;
                        fromDatabase.CountryPeriod = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _countryRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newCountries.Count();
                var numberOfUpdated = updatedCountries.Count();
                var numberOfSkipped = skipped.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = countryDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = numberOfSkipped
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
        public GeneratedFile ExportCountries(int? pageIndex = null, int? pageSize = null, CountryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.Country> query = null;
            try
            {
                query = _countryRepository.GetAll()
                                    .Include(x => x.Continent)
                                    .Include(x => x.CountryPeriod)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.ContinentId > 0)
                    {
                        query = query.Where(x => x.ContinentId == filterDto.ContinentId);
                    }
                    if (filterDto.CountryPeriodId > 0)
                    {
                        query = query.Where(x => x.CountryPeriodId == filterDto.CountryPeriodId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortCode))
                    {
                        query = query.Where(x => x.ShortCode.Trim().ToLower().Contains(filterDto.ShortCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.NativeName))
                    {
                        query = query.Where(x => x.NativeName.Trim().ToLower().Contains(filterDto.NativeName.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CurrencyCode))
                    {
                        query = query.Where(x => x.CurrencyCode.Trim().ToLower().Contains(filterDto.CurrencyCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CallingCode))
                    {
                        query = query.Where(x => x.CallingCode.Trim().ToLower().Contains(filterDto.CallingCode.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Latitude))
                    {
                        query = query.Where(x => x.Latitude.Trim().ToLower().Contains(filterDto.Latitude.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Longitude))
                    {
                        query = query.Where(x => x.Longitude.Trim().ToLower().Contains(filterDto.Longitude.Trim().ToLower()));
                    }
                    if (filterDto.Population != null)
                    {
                        query = query.Where(x => x.Population == filterDto.Population);
                    }
                }

                query = query.OrderByDescending(x => x.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ContinentName".ToLower())
                    {
                        filterDto.SortProperty = "ContinentId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "CountryPeriodName".ToLower())
                    {
                        filterDto.SortProperty = "CountryPeriodId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportCountryDto>>(query.ToList());

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
        public bool IsNameUnique(CountryDto countryDto)
        {
            var searchResult = _countryRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != countryDto.Id
                                                && x.ContinentId == countryDto.ContinentId
                                                && x.Name.ToLower().Trim() == countryDto.Name.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsShortCodeUnique(CountryDto countryDto)
        {
            var searchResult = _countryRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != countryDto.Id
                                                && x.ShortCode.ToLower().Trim() == countryDto.ShortCode.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsLatlngUnique(CountryDto countryDto)
        {
            var searchResult = _countryRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != countryDto.Id
                                                && x.Latitude.ToLower().Trim() == countryDto.Latitude.ToLower().Trim()
                                                && x.Longitude.ToLower().Trim() == countryDto.Longitude.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int countryId)
        {
            try
            {
                var country = await _countryRepository.GetAll()
                                        .Include(x => x.Regions)
                                        .Include(x => x.CountryDiseaseIncidents)
                                        .Include(x => x.CountryProductPrices)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == countryId);
                if (country == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (country.Regions.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Country cannot be deleted or deactivated where it contains 'Regions'";
                    return _response;
                }
                if (country.CountryDiseaseIncidents.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Country cannot be deleted or deactivated where it contains 'Disease Incidents'";
                    return _response;
                }
                if (country.CountryProductPrices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Country cannot be deleted or deactivated where it contains 'Product Prices'";
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
        private bool ImportSkipped(List<Data.DbModels.LookupSchema.Country> countries, Data.DbModels.LookupSchema.Country country)
        {
            var foundByName = countries.FirstOrDefault(x => x.Name.Trim().ToLower() == country.Name.Trim().ToLower());
            var foundByCode = countries.FirstOrDefault(x => x.ShortCode.Trim().ToLower() == country.ShortCode.Trim().ToLower());

            if (foundByName != null && foundByCode != null && foundByName.Id != foundByCode.Id)
            {
                return true;
            }

            return false;
        }
    }
}
