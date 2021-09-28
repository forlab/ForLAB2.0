using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Product.CountryProductPrice;
using ForLab.Repositories.Product.CountryProductPrice;
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

namespace ForLab.Services.Product.CountryProductPrice
{
    public class CountryProductPriceService : GService<CountryProductPriceDto, Data.DbModels.ProductSchema.CountryProductPrice, ICountryProductPriceRepository>, ICountryProductPriceService
    {
        private readonly ICountryProductPriceRepository _countryProductPriceRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportCountryProductPriceDto> _fileService;
        private readonly IGeneralService _generalService;
        public CountryProductPriceService(IMapper mapper,
            IResponseDTO response,
            ICountryProductPriceRepository countryProductPriceRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportCountryProductPriceDto> fileService,
            IGeneralService generalService) : base(countryProductPriceRepository, mapper)
        {
            _countryProductPriceRepository = countryProductPriceRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, CountryProductPriceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.CountryProductPrice> query = null;
            try
            {
                query = _countryProductPriceRepository.GetAll()
                                    .Include(x => x.Country)
                                    .Include(x => x.Product)
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
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.PackSize > 0)
                    {
                        query = query.Where(x => x.PackSize == filterDto.PackSize);
                    }
                    if (filterDto.Price > 0)
                    {
                        query = query.Where(x => x.Price == filterDto.Price);
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "CountryName".ToLower())
                    {
                        filterDto.SortProperty = "CountryId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ProductName".ToLower())
                    {
                        filterDto.SortProperty = "ProductId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<CountryProductPriceDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(CountryProductPriceFilterDto filterDto = null)
        {
            try
            {
                var query = _countryProductPriceRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.PackSize > 0)
                    {
                        query = query.Where(x => x.PackSize == filterDto.PackSize);
                    }
                    if (filterDto.Price > 0)
                    {
                        query = query.Where(x => x.Price == filterDto.Price);
                    }
                }

                query = query.Select(i => new Data.DbModels.ProductSchema.CountryProductPrice() 
                { 
                    Id = i.Id,
                    Product = i.Product,
                    Country = i.Country
                });
                var dataList = _mapper.Map<List<CountryProductPriceDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetCountryProductPriceDetails(int countryProductPriceId)
        {
            try
            {
                var countryProductPrice = await _countryProductPriceRepository.GetAll()
                                        .Include(x => x.Country)
                                        .Include(x => x.Product)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == countryProductPriceId);
                if (countryProductPrice == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var countryProductPriceDto = _mapper.Map<CountryProductPriceDto>(countryProductPrice);

                _response.Data = countryProductPriceDto;
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
        public async Task<IResponseDTO> CreateCountryProductPrice(CountryProductPriceDto countryProductPriceDto)
        {
            try
            {
                var countryProductPrice = _mapper.Map<Data.DbModels.ProductSchema.CountryProductPrice>(countryProductPriceDto);

                // Set relation variables with null to avoid unexpected EF errors
                countryProductPrice.Country = null;
                countryProductPrice.Product = null;

                // Add to the DB
                await _countryProductPriceRepository.AddAsync(countryProductPrice);

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
        public async Task<IResponseDTO> UpdateCountryProductPrice(CountryProductPriceDto countryProductPriceDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var countryProductPriceExist = await _countryProductPriceRepository.GetFirstAsync(x => x.Id == countryProductPriceDto.Id);
                if (countryProductPriceExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && countryProductPriceExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var countryProductPrice = _mapper.Map<Data.DbModels.ProductSchema.CountryProductPrice>(countryProductPriceDto);

                // Set relation variables with null to avoid unexpected EF errors
                countryProductPrice.Country = null;
                countryProductPrice.Product = null;

                _countryProductPriceRepository.Update(countryProductPrice);

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
        public async Task<IResponseDTO> UpdateIsActive(int countryProductPriceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var countryProductPrice = await _countryProductPriceRepository.GetFirstAsync(x => x.Id == countryProductPriceId);
                if (countryProductPrice == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && countryProductPrice.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                countryProductPrice.IsActive = IsActive;
                countryProductPrice.UpdatedBy = LoggedInUserId;
                countryProductPrice.UpdatedOn = DateTime.Now;

                // Update on the Database
                _countryProductPriceRepository.Update(countryProductPrice);

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
        public async Task<IResponseDTO> RemoveCountryProductPrice(int countryProductPriceId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var countryProductPrice = await _countryProductPriceRepository.GetFirstOrDefaultAsync(x => x.Id == countryProductPriceId);
                if (countryProductPrice == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && countryProductPrice.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                countryProductPrice.IsDeleted = true;
                countryProductPrice.UpdatedBy = LoggedInUserId;
                countryProductPrice.UpdatedOn = DateTime.Now;

                // Update on the Database
                _countryProductPriceRepository.Update(countryProductPrice);

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
        public GeneratedFile ExportCountryProductPrices(int? pageIndex = null, int? pageSize = null, CountryProductPriceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.CountryProductPrice> query = null;
            try
            {
                query = _countryProductPriceRepository.GetAll()
                                    .Include(x => x.Product)
                                    .Include(x => x.Country)
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
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.Price > 0)
                    {
                        query = query.Where(x => x.Price == filterDto.Price);
                    }
                    if (filterDto.PackSize > 0)
                    {
                        query = query.Where(x => x.PackSize == filterDto.PackSize);
                    }
                }

                query = query.OrderByDescending(x => x.CreatedOn);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ProductName".ToLower())
                    {
                        filterDto.SortProperty = "ProductId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "CountryName".ToLower())
                    {
                        filterDto.SortProperty = "CountryId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportCountryProductPriceDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportCountryProductPrices(List<CountryProductPriceDto> countryProductPriceDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var countryProductPrices_database = _countryProductPriceRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var countryProductPrices = _mapper.Map<List<Data.DbModels.ProductSchema.CountryProductPrice>>(countryProductPriceDtos);

                // vars
                var newCountryProductPrices = new List<Data.DbModels.ProductSchema.CountryProductPrice>();
                var updatedCountryProductPrices = new List<Data.DbModels.ProductSchema.CountryProductPrice>();

                // Get new and updated countryProductPrices
                foreach (var item in countryProductPrices)
                {
                    var foundCountryProductPrice = countryProductPrices_database.FirstOrDefault(x => x.CountryId == item.CountryId
                                                                                                && x.ProductId == item.ProductId);
                    
                    if (foundCountryProductPrice == null)
                    {
                        newCountryProductPrices.Add(item);
                    }
                    else
                    {
                        updatedCountryProductPrices.Add(item);
                    }
                }

                // Add the new object to the database
                if (newCountryProductPrices.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newCountryProductPrices.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Product = null;
                        x.Country = null;
                        return x;
                    }).ToList();
                    await _countryProductPriceRepository.AddRangeAsync(newCountryProductPrices);
                }

                // Update the existing objects with the new values
                if (updatedCountryProductPrices.Count() > 0)
                {
                    foreach (var item in updatedCountryProductPrices)
                    {
                        var fromDatabase = countryProductPrices_database.FirstOrDefault(x => x.CountryId == item.CountryId
                                                                                        && x.ProductId == item.ProductId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ProductId = item.ProductId;
                        fromDatabase.CountryId = item.CountryId;
                        fromDatabase.Price = item.Price;
                        fromDatabase.PackSize = item.PackSize;
                        fromDatabase.FromDate = item.FromDate;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        fromDatabase.Country = null;
                        fromDatabase.Product = null;
                        _countryProductPriceRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newCountryProductPrices.Count();
                var numberOfUpdated = updatedCountryProductPrices.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = countryProductPriceDtos.Count,
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
        public bool IsCountryProductUnique(CountryProductPriceDto countryProductPriceDto)
        {
            var searchResult = _countryProductPriceRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != countryProductPriceDto.Id
                                                && x.ProductId == countryProductPriceDto.ProductId
                                                && x.CountryId == countryProductPriceDto.CountryId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
