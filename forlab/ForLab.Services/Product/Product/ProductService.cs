using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Product.Product;
using ForLab.Repositories.Product.Product;
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

namespace ForLab.Services.Product.Product
{
    public class ProductService : GService<ProductDto, Data.DbModels.ProductSchema.Product, IProductRepository>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportProductDto> _fileService;
        private readonly IGeneralService _generalService;

        public ProductService(IMapper mapper,
            IResponseDTO response,
            IProductRepository productRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportProductDto> fileService,
            IGeneralService generalService) : base(productRepository, mapper)
        {
            _productRepository = productRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProductFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.Product> query = null;
            try
            {
                query = _productRepository.GetAll()
                                    .Include(x => x.Vendor)
                                    .Include(x => x.ProductType)
                                    .Include(x => x.ProductBasicUnit)
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
                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (filterDto.ProductTypeId > 0)
                    {
                        query = query.Where(x => x.ProductTypeId == filterDto.ProductTypeId);
                    }
                    if (filterDto.ProductBasicUnitId > 0)
                    {
                        query = query.Where(x => x.ProductBasicUnitId == filterDto.ProductBasicUnitId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CatalogNo))
                    {
                        query = query.Where(x => x.CatalogNo.Trim().ToLower().Contains(filterDto.CatalogNo.Trim().ToLower()));
                    }
                    if (filterDto.ManufacturerPrice != null)
                    {
                        query = query.Where(x => x.ManufacturerPrice == filterDto.ManufacturerPrice);
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProductTypeIds))
                    {
                        var productTypeIds = filterDto.ProductTypeIds.Split(",").ToList().ConvertAll(x => int.Parse(x));
                        query = query.Where(x => productTypeIds.Contains(x.ProductTypeId));
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
                    else if (filterDto.SortProperty.ToLower() == "ProductTypeName".ToLower())
                    {
                        filterDto.SortProperty = "ProductTypeId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ProductBasicUnitName".ToLower())
                    {
                        filterDto.SortProperty = "ProductBasicUnitId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ProductDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ProductFilterDto filterDto = null)
        {
            try
            {
                var query = _productRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.ProductTypeId > 0)
                    {
                        query = query.Where(x => x.ProductTypeId == filterDto.ProductTypeId);
                    }
                    if (filterDto.ProductBasicUnitId > 0)
                    {
                        query = query.Where(x => x.ProductBasicUnitId == filterDto.ProductBasicUnitId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CatalogNo))
                    {
                        query = query.Where(x => x.CatalogNo.Trim().ToLower().Contains(filterDto.CatalogNo.Trim().ToLower()));
                    }
                    if (filterDto.ManufacturerPrice != null)
                    {
                        query = query.Where(x => x.ManufacturerPrice == filterDto.ManufacturerPrice);
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProductTypeIds))
                    {
                        var productTypeIds = filterDto.ProductTypeIds.Split(",").ToList().ConvertAll(x => int.Parse(x));
                        query = query.Where(x => productTypeIds.Contains(x.ProductTypeId));
                    }
                }

                query = query.Select(i => new Data.DbModels.ProductSchema.Product() { Id = i.Id, Name = i.Name, ProductTypeId = i.ProductTypeId });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<ProductDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetProductDetails(int productId)
        {
            try
            {
                var product = await _productRepository.GetAll()
                                        .Include(x => x.Vendor)
                                        .Include(x => x.ProductType)
                                        .Include(x => x.ProductBasicUnit)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == productId);
                if (product == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var productDto = _mapper.Map<ProductDto>(product);

                _response.Data = productDto;
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
        public async Task<IResponseDTO> CreateProduct(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Data.DbModels.ProductSchema.Product>(productDto);

                // Set relation variables with null to avoid unexpected EF errors
                product.ProductType = null;
                product.ProductBasicUnit = null;
                product.Vendor = null;

                // Add to the DB
                await _productRepository.AddAsync(product);

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
        public async Task<IResponseDTO> UpdateProduct(ProductDto productDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productExist = await _productRepository.GetFirstAsync(x => x.Id == productDto.Id);
                if (productExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && productExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var product = _mapper.Map<Data.DbModels.ProductSchema.Product>(productDto);

                // Set relation variables with null to avoid unexpected EF errors
                product.ProductType = null;
                product.ProductBasicUnit = null;
                product.Vendor = null;

                _productRepository.Update(product);

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
        public async Task<IResponseDTO> UpdateIsActive(int productId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var product = await _productRepository.GetFirstAsync(x => x.Id == productId);
                if (product == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && product.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                product.IsActive = IsActive;
                product.UpdatedBy = LoggedInUserId;
                product.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                product.CountryProductPrices = null;
                product.ProductUsages = null;
                product.LaboratoryConsumptions = null;
                product.RegionProductPrices = null;
                product.LaboratoryProductPrices = null;

                // Update on the Database
                _productRepository.Update(product);

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
                var items = _productRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _productRepository.Update(item);
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
                var items = _productRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _productRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveProduct(int productId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var product = await _productRepository.GetFirstOrDefaultAsync(x => x.Id == productId);
                if (product == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && product.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                product.IsDeleted = true;
                product.UpdatedBy = LoggedInUserId;
                product.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                product.CountryProductPrices = null;
                product.ProductUsages = null;
                product.LaboratoryConsumptions = null;
                product.RegionProductPrices = null;
                product.LaboratoryProductPrices = null;

                // Update on the Database
                _productRepository.Update(product);

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
        public GeneratedFile ExportProducts(int? pageIndex = null, int? pageSize = null, ProductFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.Product> query = null;
            try
            {
                query = _productRepository.GetAll()
                                    .Include(x => x.Vendor)
                                    .Include(x => x.ProductBasicUnit)
                                    .Include(x => x.ProductType)
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
                    if (filterDto.VendorId > 0)
                    {
                        query = query.Where(x => x.VendorId == filterDto.VendorId);
                    }
                    if (filterDto.ProductBasicUnitId > 0)
                    {
                        query = query.Where(x => x.ProductBasicUnitId == filterDto.ProductBasicUnitId);
                    }
                    if (filterDto.ProductTypeId > 0)
                    {
                        query = query.Where(x => x.ProductTypeId == filterDto.ProductTypeId);
                    }
                    if (filterDto.ManufacturerPrice > 0)
                    {
                        query = query.Where(x => x.ManufacturerPrice == filterDto.ManufacturerPrice);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.CatalogNo))
                    {
                        query = query.Where(x => x.CatalogNo.Trim().ToLower().Contains(filterDto.CatalogNo.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ProductTypeIds))
                    {
                        var productTypeIds = filterDto.ProductTypeIds.Split(",").ToList().ConvertAll(x => int.Parse(x));
                        query = query.Where(x => productTypeIds.Contains(x.ProductTypeId));
                    }
                }

                query = query.OrderByDescending(x => x.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "VendorName".ToLower())
                    {
                        filterDto.SortProperty = "VendorId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ProductBasicUnitName".ToLower())
                    {
                        filterDto.SortProperty = "ProductBasicUnitId";
                    }
                    else if (filterDto.SortProperty.ToLower() == "ProductTypeName".ToLower())
                    {
                        filterDto.SortProperty = "ProductTypeId";
                    }
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportProductDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportProducts(List<ProductDto> productDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var products_database = _productRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var products = _mapper.Map<List<Data.DbModels.ProductSchema.Product>>(productDtos);

                // vars
                var catalogs_database = products_database.Select(x => x.CatalogNo.ToLower().Trim());
                var names_database = products_database.Select(x => x.Name.ToLower().Trim());

                // Get the new ones that their names don't exist on the database
                var newProducts = products.Where(x => !names_database.Contains(x.Name.ToLower().Trim())
                                                     && !catalogs_database.Contains(x.CatalogNo.ToLower().Trim()));

                // Get Skipped
                var skipped = products.Where(x => ImportSkipped(products_database, x)).ToList();

                // Select the objects that their names already exist in the database
                var updatedProducts = products.Except(skipped).Except(newProducts);

                // Add the new object to the database
                if (newProducts.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newProducts.Select(x =>
                    {
                        x.CountryProductPrices = null;
                        x.RegionProductPrices = null;
                        x.ProductUsages = null;
                        x.LaboratoryProductPrices = null;
                        x.LaboratoryConsumptions = null;
                        x.Vendor = null;
                        x.ProductBasicUnit = null;
                        x.ProductType = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _productRepository.AddRangeAsync(newProducts);
                }

                // Update the existing objects with the new values
                if (updatedProducts.Count() > 0)
                {
                    foreach (var item in updatedProducts)
                    {
                        var fromDatabase = products_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                              || x.CatalogNo.ToLower().Trim() == item.CatalogNo?.ToLower()?.Trim());
                        if (fromDatabase == null)
                        {
                            continue;
                        }
                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.VendorId = item.VendorId;
                        fromDatabase.ManufacturerPrice = item.ManufacturerPrice;
                        fromDatabase.ProductTypeId = item.ProductTypeId;
                        fromDatabase.Name = item.Name;
                        fromDatabase.CatalogNo = item.CatalogNo;
                        fromDatabase.ProductBasicUnitId = item.ProductBasicUnitId;
                        fromDatabase.PackSize = item.PackSize;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.CountryProductPrices = null;
                        fromDatabase.RegionProductPrices = null;
                        fromDatabase.ProductUsages = null;
                        fromDatabase.LaboratoryProductPrices = null;
                        fromDatabase.LaboratoryConsumptions = null;
                        fromDatabase.Vendor = null;
                        fromDatabase.ProductBasicUnit = null;
                        fromDatabase.ProductType = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _productRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newProducts.Count();
                var numberOfUpdated = updatedProducts.Count();
                var numberOfSkipped = skipped.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = productDtos.Count,
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
        // Validators methods
        public bool IsNameUnique(ProductDto productDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _productRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != productDto.Id
                                                && x.Name.ToLower().Trim() == productDto.Name.ToLower().Trim());
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
        public bool IsCatalogNoUnique(ProductDto productDto)
        {
            var searchResult = _productRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != productDto.Id
                                                && x.CatalogNo.ToLower().Trim() == productDto.CatalogNo.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int productId)
        {
            try
            {
                var product = await _productRepository.GetAll()
                                        .Include(x => x.CountryProductPrices)
                                        .Include(x => x.ProductUsages)
                                        .Include(x => x.LaboratoryConsumptions)
                                        .Include(x => x.RegionProductPrices)
                                        .Include(x => x.LaboratoryProductPrices)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == productId);
                if (product == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (product.CountryProductPrices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Product cannot be deleted or deactivated where it contains 'Country Prices'";
                    return _response;
                }
                if (product.ProductUsages.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Product cannot be deleted or deactivated where it contains 'Product Usages'";
                    return _response;
                }
                if (product.LaboratoryConsumptions.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Product cannot be deleted or deactivated where it contains 'Laboratory Consumptions'";
                    return _response;
                }
                if (product.RegionProductPrices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Product cannot be deleted or deactivated where it contains 'Region Prices'";
                    return _response;
                }
                if (product.LaboratoryProductPrices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Product cannot be deleted or deactivated where it contains 'Laboratory Prices'";
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
        private bool ImportSkipped(List<Data.DbModels.ProductSchema.Product> products, Data.DbModels.ProductSchema.Product product)
        {
            var foundByName = products.FirstOrDefault(x => x.Name.Trim().ToLower() == product.Name.Trim().ToLower());
            var foundByCatalog = products.FirstOrDefault(x => x.CatalogNo.Trim().ToLower() == product.CatalogNo.Trim().ToLower());

            if (foundByName != null && foundByCatalog != null && foundByName.Id != foundByCatalog.Id)
            {
                return true;
            }

            return false;
        }
    }
}
