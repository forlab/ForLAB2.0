using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Product.LaboratoryProductPrice;
using ForLab.Repositories.Product.LaboratoryProductPrice;
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

namespace ForLab.Services.Product.LaboratoryProductPrice
{
    public class LaboratoryProductPriceService : GService<LaboratoryProductPriceDto, Data.DbModels.ProductSchema.LaboratoryProductPrice, ILaboratoryProductPriceRepository>, ILaboratoryProductPriceService
    {
        private readonly ILaboratoryProductPriceRepository _laboratoryProductPriceRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryProductPriceDto> _fileService;
        private readonly IGeneralService _generalService;

        public LaboratoryProductPriceService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryProductPriceRepository laboratoryProductPriceRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportLaboratoryProductPriceDto> fileService,
            IGeneralService generalService) : base(laboratoryProductPriceRepository, mapper)
        {
            _laboratoryProductPriceRepository = laboratoryProductPriceRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryProductPriceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.LaboratoryProductPrice> query = null;
            try
            {
                query = _laboratoryProductPriceRepository.GetAll()
                                    .Include(x => x.Laboratory).ThenInclude(x => x.Region).ThenInclude(x => x.Country)
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
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
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
                    if (filterDto.SortProperty.ToLower() == "LaboratoryName".ToLower())
                    {
                        filterDto.SortProperty = "LaboratoryId";
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

                var dataList = _mapper.Map<List<LaboratoryProductPriceDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryProductPriceFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryProductPriceRepository.GetAll(x => !x.IsDeleted && x.IsActive);

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
                    if (filterDto.PackSize > 0)
                    {
                        query = query.Where(x => x.PackSize == filterDto.PackSize);
                    }
                    if (filterDto.Price > 0)
                    {
                        query = query.Where(x => x.Price == filterDto.Price);
                    }
                }

                query = query.Select(i => new Data.DbModels.ProductSchema.LaboratoryProductPrice()
                {
                    Id = i.Id ,
                    Laboratory = i.Laboratory ,
                    Product = i.Product
                });
                var dataList = _mapper.Map<List<LaboratoryProductPriceDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryProductPriceDetails(int laboratoryProductPriceId)
        {
            try
            {
                var laboratoryProductPrice = await _laboratoryProductPriceRepository.GetAll()
                                        .Include(x => x.Laboratory).ThenInclude(x => x.Region).ThenInclude(x => x.Country)
                                        .Include(x => x.Product)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryProductPriceId);
                if (laboratoryProductPrice == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryProductPriceDto = _mapper.Map<LaboratoryProductPriceDto>(laboratoryProductPrice);

                _response.Data = laboratoryProductPriceDto;
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
        public async Task<IResponseDTO> CreateLaboratoryProductPrice(LaboratoryProductPriceDto laboratoryProductPriceDto)
        {
            try
            {
                var laboratoryProductPrice = _mapper.Map<Data.DbModels.ProductSchema.LaboratoryProductPrice>(laboratoryProductPriceDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryProductPrice.Laboratory = null;
                laboratoryProductPrice.Product = null;

                // Add to the DB
                await _laboratoryProductPriceRepository.AddAsync(laboratoryProductPrice);

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
        public async Task<IResponseDTO> UpdateLaboratoryProductPrice(LaboratoryProductPriceDto laboratoryProductPriceDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryProductPriceExist = await _laboratoryProductPriceRepository.GetFirstAsync(x => x.Id == laboratoryProductPriceDto.Id);
                if (laboratoryProductPriceExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryProductPriceExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryProductPrice = _mapper.Map<Data.DbModels.ProductSchema.LaboratoryProductPrice>(laboratoryProductPriceDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryProductPrice.Laboratory = null;
                laboratoryProductPrice.Product = null;

                _laboratoryProductPriceRepository.Update(laboratoryProductPrice);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryProductPriceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryProductPrice = await _laboratoryProductPriceRepository.GetFirstAsync(x => x.Id == laboratoryProductPriceId);
                if (laboratoryProductPrice == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryProductPrice.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryProductPrice.IsActive = IsActive;
                laboratoryProductPrice.UpdatedBy = LoggedInUserId;
                laboratoryProductPrice.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryProductPriceRepository.Update(laboratoryProductPrice);

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
        public async Task<IResponseDTO> RemoveLaboratoryProductPrice(int laboratoryProductPriceId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryProductPrice = await _laboratoryProductPriceRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryProductPriceId);
                if (laboratoryProductPrice == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryProductPrice.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryProductPrice.IsDeleted = true;
                laboratoryProductPrice.UpdatedBy = LoggedInUserId;
                laboratoryProductPrice.UpdatedOn = DateTime.Now;

                // Update on the Database
                _laboratoryProductPriceRepository.Update(laboratoryProductPrice);

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
        public GeneratedFile ExportLaboratoryProductPrices(int? pageIndex = null, int? pageSize = null, LaboratoryProductPriceFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.ProductSchema.LaboratoryProductPrice> query = null;
            try
            {
                query = _laboratoryProductPriceRepository.GetAll()
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
                    if (filterDto.ProductId > 0)
                    {
                        query = query.Where(x => x.ProductId == filterDto.ProductId);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
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

                var dataList = _mapper.Map<List<ExportLaboratoryProductPriceDto>>(query.ToList());

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
        public async Task<IResponseDTO> ImportLaboratoryProductPrices(List<LaboratoryProductPriceDto> laboratoryProductPriceDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryProductPrices_database = _laboratoryProductPriceRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryProductPrices = _mapper.Map<List<Data.DbModels.ProductSchema.LaboratoryProductPrice>>(laboratoryProductPriceDtos);

                // vars
                var newLaboratoryProductPrices = new List<Data.DbModels.ProductSchema.LaboratoryProductPrice>();
                var updatedLaboratoryProductPrices = new List<Data.DbModels.ProductSchema.LaboratoryProductPrice>();

                // Get new and updated laboratoryProductPrices
                foreach (var item in laboratoryProductPrices)
                {
                    var foundLaboratoryProductPrice = laboratoryProductPrices_database.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId
                                                                                                        && x.ProductId == item.ProductId);
                    if (foundLaboratoryProductPrice == null)
                    {
                        newLaboratoryProductPrices.Add(item);
                    }
                    else
                    {
                        updatedLaboratoryProductPrices.Add(item);
                    }
                }

                // Add the new object to the database
                if (newLaboratoryProductPrices.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newLaboratoryProductPrices.Select(x =>
                    {
                        x.Creator = null;
                        x.Updator = null;
                        x.Product = null;
                        x.Laboratory = null;
                        return x;
                    }).ToList();
                    await _laboratoryProductPriceRepository.AddRangeAsync(newLaboratoryProductPrices);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryProductPrices.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryProductPrices)
                    {
                        var fromDatabase = laboratoryProductPrices_database.FirstOrDefault(x => x.LaboratoryId == item.LaboratoryId
                                                                                             && x.ProductId == item.ProductId);
                        
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.ProductId = item.ProductId;
                        fromDatabase.LaboratoryId = item.LaboratoryId;
                        fromDatabase.Price = item.Price;
                        fromDatabase.PackSize = item.PackSize;
                        fromDatabase.FromDate = item.FromDate;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Laboratory = null;
                        fromDatabase.Product = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _laboratoryProductPriceRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryProductPrices.Count();
                var numberOfUpdated = updatedLaboratoryProductPrices.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = laboratoryProductPriceDtos.Count,
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
        public bool IsLaboratoryProductUnique(LaboratoryProductPriceDto laboratoryProductPriceDto)
        {
            var searchResult = _laboratoryProductPriceRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryProductPriceDto.Id
                                                && x.ProductId == laboratoryProductPriceDto.ProductId
                                                && x.LaboratoryId == laboratoryProductPriceDto.LaboratoryId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
