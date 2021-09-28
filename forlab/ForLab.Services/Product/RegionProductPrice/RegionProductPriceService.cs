using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Product.RegionProductPrice;
using ForLab.Repositories.Product.RegionProductPrice;
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

namespace ForLab.Services.Product.RegionProductPrice
{
    public class RegionProductPriceService : GService<RegionProductPriceDto, Data.DbModels.ProductSchema.RegionProductPrice, IRegionProductPriceRepository>, IRegionProductPriceService
    {
        private readonly IRegionProductPriceRepository _regionProductPriceRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportRegionProductPriceDto> _fileService;
        private readonly IGeneralService _generalService;
        public RegionProductPriceService(IMapper mapper,
            IResponseDTO response,
            IRegionProductPriceRepository regionProductPriceRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportRegionProductPriceDto> fileService,
            IGeneralService generalService) : base(regionProductPriceRepository, mapper)
        {
            _regionProductPriceRepository = regionProductPriceRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, RegionProductPriceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.RegionProductPrice> query = null;
            try
            {
                query = _regionProductPriceRepository.GetAll()
                                    .Include(x => x.Region).ThenInclude(x => x.Country)
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
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
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
                    if (filterDto.SortProperty.ToLower() == "RegionName".ToLower())
                    {
                        filterDto.SortProperty = "RegionId";
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

                var dataList = _mapper.Map<List<RegionProductPriceDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(RegionProductPriceFilterDto filterDto = null)
        {
            try
            {
                var query = _regionProductPriceRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
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

                query = query.Select(i => new Data.DbModels.ProductSchema.RegionProductPrice()
                { 
                    Id = i.Id,
                    Product = i.Product,
                    Region = i.Region
                });
                var dataList = _mapper.Map<List<RegionProductPriceDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetRegionProductPriceDetails(int regionProductPriceId)
        {
            try
            {
                var regionProductPrice = await _regionProductPriceRepository.GetAll()
                                        .Include(x => x.Region).ThenInclude(x => x.Country)
                                        .Include(x => x.Product)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == regionProductPriceId);
                if (regionProductPrice == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var regionProductPriceDto = _mapper.Map<RegionProductPriceDto>(regionProductPrice);

                _response.Data = regionProductPriceDto;
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
        public async Task<IResponseDTO> CreateRegionProductPrice(RegionProductPriceDto regionProductPriceDto)
        {
            try
            {
                var regionProductPrice = _mapper.Map<Data.DbModels.ProductSchema.RegionProductPrice>(regionProductPriceDto);

                // Set relation variables with null to avoid unexpected EF errors
                regionProductPrice.Region = null;
                regionProductPrice.Product = null;

                // Add to the DB
                await _regionProductPriceRepository.AddAsync(regionProductPrice);

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
        public async Task<IResponseDTO> UpdateRegionProductPrice(RegionProductPriceDto regionProductPriceDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var regionProductPriceExist = await _regionProductPriceRepository.GetFirstAsync(x => x.Id == regionProductPriceDto.Id);
                if (regionProductPriceExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && regionProductPriceExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var regionProductPrice = _mapper.Map<Data.DbModels.ProductSchema.RegionProductPrice>(regionProductPriceDto);

                // Set relation variables with null to avoid unexpected EF errors
                regionProductPrice.Region = null;
                regionProductPrice.Product = null;

                _regionProductPriceRepository.Update(regionProductPrice);

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
        public async Task<IResponseDTO> UpdateIsActive(int regionProductPriceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var regionProductPrice = await _regionProductPriceRepository.GetFirstAsync(x => x.Id == regionProductPriceId);
                if (regionProductPrice == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && regionProductPrice.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                regionProductPrice.IsActive = IsActive;
                regionProductPrice.UpdatedBy = LoggedInUserId;
                regionProductPrice.UpdatedOn = DateTime.Now;

                // Update on the Database
                _regionProductPriceRepository.Update(regionProductPrice);

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
        public async Task<IResponseDTO> RemoveRegionProductPrice(int regionProductPriceId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var regionProductPrice = await _regionProductPriceRepository.GetFirstOrDefaultAsync(x => x.Id == regionProductPriceId);
                if (regionProductPrice == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && regionProductPrice.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                regionProductPrice.IsDeleted = true;
                regionProductPrice.UpdatedBy = LoggedInUserId;
                regionProductPrice.UpdatedOn = DateTime.Now;

                // Update on the Database
                _regionProductPriceRepository.Update(regionProductPrice);

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
        public GeneratedFile ExportRegionProductPrices(int? pageIndex = null, int? pageSize = null, RegionProductPriceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.RegionProductPrice> query = null;
            try
            {
                query = _regionProductPriceRepository.GetAll()
                                    .Include(x => x.Product)
                                    .Include(x => x.Region).ThenInclude(x => x.Country)
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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
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
                    else if (filterDto.SortProperty.ToLower() == "RegionName".ToLower())
                    {
                        filterDto.SortProperty = "RegionId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportRegionProductPriceDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportRegionProductPrices(List<RegionProductPriceDto> regionProductPriceDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var regionProductPrices_database = _regionProductPriceRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var regionProductPrices = _mapper.Map<List<Data.DbModels.ProductSchema.RegionProductPrice>>(regionProductPriceDtos);

                // vars
                var newRegionProductPrices = new List<Data.DbModels.ProductSchema.RegionProductPrice>();
                var updatedRegionProductPrices = new List<Data.DbModels.ProductSchema.RegionProductPrice>();

                // Get new and updated regionProductPrices
                foreach (var item in regionProductPrices)
                {
                    var foundRegionProductPrice = regionProductPrices_database.FirstOrDefault(x => x.RegionId == item.RegionId
                                                                                                && x.ProductId == item.ProductId);
                    if (foundRegionProductPrice == null)
                    {
                        newRegionProductPrices.Add(item);
                    }
                    else
                    {
                        updatedRegionProductPrices.Add(item);
                    }
                }

                // Add the new object to the database
                if (newRegionProductPrices.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newRegionProductPrices.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Product = null;
                        x.Region = null;
                        return x;
                    }).ToList();
                    await _regionProductPriceRepository.AddRangeAsync(newRegionProductPrices);
                }

                // Update the existing objects with the new values
                if (updatedRegionProductPrices.Count() > 0)
                {
                    foreach (var item in updatedRegionProductPrices)
                    {
                        var fromDatabase = regionProductPrices_database.FirstOrDefault(x => x.RegionId == item.RegionId
                                                                                         && x.ProductId == item.ProductId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ProductId = item.ProductId;
                        fromDatabase.RegionId = item.RegionId;
                        fromDatabase.Price = item.Price;
                        fromDatabase.PackSize = item.PackSize;
                        fromDatabase.FromDate = item.FromDate;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Region = null;
                        fromDatabase.Product = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _regionProductPriceRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newRegionProductPrices.Count();
                var numberOfUpdated = updatedRegionProductPrices.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = regionProductPriceDtos.Count,
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
        public bool IsRegionProductUnique(RegionProductPriceDto regionProductPriceDto)
        {
            var searchResult = _regionProductPriceRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != regionProductPriceDto.Id
                                                && x.ProductId == regionProductPriceDto.ProductId
                                                && x.RegionId == regionProductPriceDto.RegionId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
