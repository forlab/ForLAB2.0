using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Product.ProductUsage;
using ForLab.Repositories.Product.ProductUsage;
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

namespace ForLab.Services.Product.ProductUsage
{
    public class ProductUsageService : GService<ProductUsageDto, Data.DbModels.ProductSchema.ProductUsage, IProductUsageRepository>, IProductUsageService
    {
        private readonly IProductUsageRepository _productUsageRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportProductUsageDto> _fileService;
        private readonly IFileService<ExportTestUsageDto> _fileServiceTest;
        private readonly IGeneralService _generalService;

        public ProductUsageService(IMapper mapper,
            IResponseDTO response,
            IProductUsageRepository productUsageRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportProductUsageDto> fileService,
            IFileService<ExportTestUsageDto> fileServiceTest,
            IGeneralService generalService) : base(productUsageRepository, mapper)
        {
            _productUsageRepository = productUsageRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _fileServiceTest = fileServiceTest;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProductUsageFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.ProductUsage> query = null;
            try
            {
                query = _productUsageRepository.GetAll()
                                    .Include(x => x.Instrument)
                                    .Include(x => x.CountryPeriod)
                                    .Include(x => x.Product)
                                    .Include(x => x.Test)
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
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.IsForControl != null)
                    {
                        query = query.Where(x => x.IsForControl == filterDto.IsForControl);
                    }
                    if (filterDto.PerPeriod != null)
                    {
                        query = query.Where(x => x.PerPeriod == filterDto.PerPeriod);
                    }
                    if (filterDto.PerPeriodPerInstrument != null)
                    {
                        query = query.Where(x => x.PerPeriodPerInstrument == filterDto.PerPeriodPerInstrument);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.Amount != null)
                    {
                        query = query.Where(x => x.Amount == filterDto.Amount);
                    }
                    if (filterDto.CountryPeriodId != null)
                    {
                        query = query.Where(x => x.CountryPeriodId == filterDto.CountryPeriodId);
                    }
                    if(!string.IsNullOrEmpty(filterDto.ProductTypeIds))
                    {
                        var productTypeIds = filterDto.ProductTypeIds.Split(",").ToList().ConvertAll(x => int.Parse(x));
                        query = query.Where(x => productTypeIds.Contains(x.Product.ProductTypeId));
                    }
                }

                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "CountryPeriodName".ToLower())
                    {
                        filterDto.SortProperty = "CountryPeriodId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "InstrumentName".ToLower())
                    {
                        filterDto.SortProperty = "InstrumentId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ProductName".ToLower())
                    {
                        filterDto.SortProperty = "ProductId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ProductUsageDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ProductUsageFilterDto filterDto = null)
        {
            try
            {
                var query = _productUsageRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.IsForControl != null)
                    {
                        query = query.Where(x => x.IsForControl == filterDto.IsForControl);
                    }
                    if (filterDto.PerPeriod != null)
                    {
                        query = query.Where(x => x.PerPeriod == filterDto.PerPeriod);
                    }
                    if (filterDto.PerPeriodPerInstrument != null)
                    {
                        query = query.Where(x => x.PerPeriodPerInstrument == filterDto.PerPeriodPerInstrument);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.Amount != null)
                    {
                        query = query.Where(x => x.Amount == filterDto.Amount);
                    }
                    if (filterDto.CountryPeriodId != null)
                    {
                        query = query.Where(x => x.CountryPeriodId == filterDto.CountryPeriodId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProductTypeIds))
                    {
                        var productTypeIds = filterDto.ProductTypeIds.Split(",").ToList().ConvertAll(x => int.Parse(x));
                        query = query.Where(x => productTypeIds.Contains(x.Product.ProductTypeId));
                    }
                }

                query = query.Select(i => new Data.DbModels.ProductSchema.ProductUsage()
                {
                    Id = i.Id,
                    Instrument = i.Instrument,
                    Product = i.Product
                });
                var dataList = _mapper.Map<List<ProductUsageDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetProductUsageDetails(int productUsageId)
        {
            try
            {
                var productUsage = await _productUsageRepository.GetAll()
                                            .Include(x => x.Instrument)
                                            .Include(x => x.CountryPeriod)
                                            .Include(x => x.Product)
                                            .Include(x => x.Creator)
                                            .FirstOrDefaultAsync(x => x.Id == productUsageId);
                if (productUsage == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var productUsageDto = _mapper.Map<ProductUsageDto>(productUsage);

                _response.Data = productUsageDto;
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
        public async Task<IResponseDTO> CreateProductUsage(ProductUsageDto productUsageDto)
        {
            try
            {
                var productUsage = _mapper.Map<Data.DbModels.ProductSchema.ProductUsage>(productUsageDto);

                // Set relation variables with null to avoid unexpected EF errors
                productUsage.Instrument = null;
                productUsage.CountryPeriod = null;
                productUsage.Test = null;
                productUsage.Product = null;

                // Add to the DB
                await _productUsageRepository.AddAsync(productUsage);

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
        public async Task<IResponseDTO> UpdateProductUsage(ProductUsageDto productUsageDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productUsageExist = await _productUsageRepository.GetFirstAsync(x => x.Id == productUsageDto.Id);
                if (productUsageExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && productUsageExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var productUsage = _mapper.Map<Data.DbModels.ProductSchema.ProductUsage>(productUsageDto);

                // Set relation variables with null to avoid unexpected EF errors
                productUsage.Instrument = null;
                productUsage.CountryPeriod = null;
                productUsage.Test = null;
                productUsage.Product = null;

                _productUsageRepository.Update(productUsage);

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
        public async Task<IResponseDTO> UpdateIsActive(int productUsageId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productUsage = await _productUsageRepository.GetFirstAsync(x => x.Id == productUsageId);
                if (productUsage == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && productUsage.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                productUsage.IsActive = IsActive;
                productUsage.UpdatedBy = LoggedInUserId;
                productUsage.UpdatedOn = DateTime.Now;

                // Update on the Database
                _productUsageRepository.Update(productUsage);

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
        public async Task<IResponseDTO> RemoveProductUsage(int productUsageId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productUsage = await _productUsageRepository.GetFirstOrDefaultAsync(x => x.Id == productUsageId);
                if (productUsage == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && productUsage.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                productUsage.IsDeleted = true;
                productUsage.UpdatedBy = LoggedInUserId;
                productUsage.UpdatedOn = DateTime.Now;

                // Update on the Database
                _productUsageRepository.Update(productUsage);

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
        public GeneratedFile ExportProductUsages(int? pageIndex = null, int? pageSize = null, ProductUsageFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.ProductUsage> query = null;
            try
            {
                query = _productUsageRepository.GetAll()
                                    .Include(x => x.Instrument)
                                    .Include(x => x.CountryPeriod)
                                    .Include(x => x.Product)
                                    .Include(x => x.Test)
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
                    if (filterDto.TestId > 0)
                    {
                        query = query.Where(x => x.TestId == filterDto.TestId);
                    }
                    if (filterDto.IsForControl != null)
                    {
                        query = query.Where(x => x.IsForControl == filterDto.IsForControl);
                    }
                    if (filterDto.PerPeriod != null)
                    {
                        query = query.Where(x => x.PerPeriod == filterDto.PerPeriod);
                    }
                    if (filterDto.PerPeriodPerInstrument != null)
                    {
                        query = query.Where(x => x.PerPeriodPerInstrument == filterDto.PerPeriodPerInstrument);
                    }
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.Amount != null)
                    {
                        query = query.Where(x => x.Amount == filterDto.Amount);
                    }
                    if (filterDto.CountryPeriodId != null)
                    {
                        query = query.Where(x => x.CountryPeriodId == filterDto.CountryPeriodId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProductTypeIds))
                    {
                        var productTypeIds = filterDto.ProductTypeIds.Split(",").ToList().ConvertAll(x => int.Parse(x));
                        query = query.Where(x => productTypeIds.Contains(x.Product.ProductTypeId));
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
                    else if (filterDto.SortProperty.ToLower() == "TestName".ToLower())
                    {
                        filterDto.SortProperty = "TestId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "InstrumentName".ToLower())
                    {
                        filterDto.SortProperty = "InstrumentId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                if(filterDto.ExportProductUsage)
                {
                    var productDataList = _mapper.Map<List<ExportProductUsageDto>>(query.ToList());
                    return _fileService.ExportToExcel(productDataList);
                }


                var dataList = _mapper.Map<List<ExportTestUsageDto>>(query.ToList());
                return _fileServiceTest.ExportToExcel(dataList);
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Error " + ex.Message;
            }
            return null;
        }
        public async Task<IResponseDTO> ImportProductUsages(List<ProductUsageDto> productUsageDtos, bool isProduct, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var productUsages_database = _productUsageRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var productUsages = _mapper.Map<List<Data.DbModels.ProductSchema.ProductUsage>>(productUsageDtos);

                // vars
                var newProductUsages = new List<Data.DbModels.ProductSchema.ProductUsage>();
                var updatedProductUsages = new List<Data.DbModels.ProductSchema.ProductUsage>();

                // Get new and updated productUsages
                foreach (var item in productUsages)
                {
                    Data.DbModels.ProductSchema.ProductUsage foundProductUsage = null;
                    if(isProduct)
                    {
                        if(item.PerPeriod || item.PerPeriodPerInstrument)
                        {
                            if(item.PerPeriod)
                            {
                                foundProductUsage = productUsages_database.FirstOrDefault(x =>
                                                            x.ProductId == item.ProductId
                                                            && x.PerPeriod == item.PerPeriod
                                                            && x.CountryPeriodId == item.CountryPeriodId);
                            } 
                            else if(item.PerPeriodPerInstrument)
                            {
                                foundProductUsage = productUsages_database.FirstOrDefault(x =>
                                                            x.ProductId == item.ProductId
                                                            && x.PerPeriodPerInstrument == item.PerPeriodPerInstrument
                                                            && x.InstrumentId == item.InstrumentId);
                            }
                        } 
                    } 
                    else
                    {
                        foundProductUsage = productUsages_database.FirstOrDefault(x =>
                                                                     x.ProductId == item.ProductId
                                                                     && x.TestId == item.TestId
                                                                     && x.InstrumentId == item.InstrumentId);
                    }

                    if (foundProductUsage == null)
                    {
                        newProductUsages.Add(item);
                    }
                    else
                    {
                        updatedProductUsages.Add(item);
                    }
                }

                // Add the new object to the database
                if (newProductUsages.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newProductUsages.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Product = null;
                        x.Test = null;
                        x.Instrument = null;
                        x.CountryPeriod = null;
                        return x;
                    }).ToList();
                    await _productUsageRepository.AddRangeAsync(newProductUsages);
                }

                // Update the existing objects with the new values
                if (updatedProductUsages.Count() > 0)
                {
                    foreach (var item in updatedProductUsages)
                    {
                        Data.DbModels.ProductSchema.ProductUsage fromDatabase = null;
                        if (isProduct)
                        {
                            if (item.PerPeriod || item.PerPeriodPerInstrument)
                            {
                                if (item.PerPeriod)
                                {
                                    fromDatabase = productUsages_database.FirstOrDefault(x =>
                                                                x.ProductId == item.ProductId
                                                                && x.PerPeriod == item.PerPeriod
                                                                && x.CountryPeriodId == item.CountryPeriodId);
                                }
                                else if (item.PerPeriodPerInstrument)
                                {
                                    fromDatabase = productUsages_database.FirstOrDefault(x =>
                                                                x.ProductId == item.ProductId
                                                                && x.PerPeriodPerInstrument == item.PerPeriodPerInstrument
                                                                && x.InstrumentId == item.InstrumentId);
                                }
                            }
                        }
                        else
                        {
                            fromDatabase = productUsages_database.FirstOrDefault(x =>
                                                                         x.ProductId == item.ProductId
                                                                         && x.TestId == item.TestId
                                                                         && x.InstrumentId == item.InstrumentId);
                        }

                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ProductId = item.ProductId;
                        fromDatabase.TestId = item.TestId;
                        fromDatabase.InstrumentId = item.InstrumentId;
                        fromDatabase.IsForControl = item.IsForControl;
                        fromDatabase.Amount = item.Amount;
                        fromDatabase.PerPeriod = item.PerPeriod;
                        fromDatabase.PerPeriodPerInstrument = item.PerPeriodPerInstrument;
                        fromDatabase.CountryPeriodId = item.CountryPeriodId;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Instrument = null;
                        fromDatabase.Product = null;
                        fromDatabase.Test = null;
                        fromDatabase.CountryPeriod = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _productUsageRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newProductUsages.Count();
                var numberOfUpdated = updatedProductUsages.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = productUsageDtos.Count,
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
        public bool IsProductUsagePeriodUnique(ProductUsageDto productUsageDto)
        {
            var searchResult = _productUsageRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != productUsageDto.Id
                                                && x.ProductId == productUsageDto.ProductId
                                                && x.PerPeriod == productUsageDto.PerPeriod
                                                && x.CountryPeriodId == productUsageDto.CountryPeriodId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsProductUsageInstrumentUnique(ProductUsageDto productUsageDto)
        {
            var searchResult = _productUsageRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != productUsageDto.Id
                                                && x.ProductId == productUsageDto.ProductId
                                                && x.PerPeriodPerInstrument == productUsageDto.PerPeriodPerInstrument
                                                && x.InstrumentId == productUsageDto.InstrumentId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public bool IsTestUsageUnique(ProductUsageDto productUsageDto)
        {
            var searchResult = _productUsageRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != productUsageDto.Id
                                                && x.ProductId == productUsageDto.ProductId
                                                && x.TestId == productUsageDto.TestId
                                                && x.InstrumentId == productUsageDto.InstrumentId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
