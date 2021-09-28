using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.ProductAssumptionParameter;
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

namespace ForLab.Services.DiseaseProgram.ProductAssumptionParameter
{
    public class ProductAssumptionParameterService : GService<ProductAssumptionParameterDto, Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter, IProductAssumptionParameterRepository>, IProductAssumptionParameterService
    {
        private readonly IProductAssumptionParameterRepository _productAssumptionParameterRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportProductAssumptionParameterDto> _fileService;
        private readonly IGeneralService _generalService;
        public ProductAssumptionParameterService(IMapper mapper,
            IResponseDTO response,
            IProductAssumptionParameterRepository productAssumptionParameterRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportProductAssumptionParameterDto> fileService,
            IGeneralService generalService) : base(productAssumptionParameterRepository, mapper)
        {
            _productAssumptionParameterRepository = productAssumptionParameterRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProductAssumptionParameterFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter> query = null;
            try
            {
                query = _productAssumptionParameterRepository.GetAll()
                                    .Include(x => x.Program)
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
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.IsPercentage != null)
                    {
                        query = query.Where(x => x.IsPercentage == filterDto.IsPercentage);
                    }
                    if (filterDto.IsPositive != null)
                    {
                        query = query.Where(x => x.IsPositive == filterDto.IsPositive);
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "ProgramName".ToLower())
                    {
                        filterDto.SortProperty = "ProgramId";
                    }
                    query = query.OrderBy(
                    string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ProductAssumptionParameterDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(ProductAssumptionParameterFilterDto filterDto = null)
        {
            try
            {
                var query = _productAssumptionParameterRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.IsPercentage != null)
                    {
                        query = query.Where(x => x.IsPercentage == filterDto.IsPercentage);
                    }
                    if (filterDto.IsPositive != null)
                    {
                        query = query.Where(x => x.IsPositive == filterDto.IsPositive);
                    }
                }

                query = query.Select(i => new Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<ProductAssumptionParameterDrp>>(query.ToList());

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
        public IResponseDTO GetAllProductAssumptionsForForcast(string programIds)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter> query = null;
            try
            {
                // Filter by program ids
                var progIds = programIds?.Split(',')?.Select(int.Parse)?.ToList();
                if (progIds == null || progIds.Count == 0 || !progIds.Any(x => x != 0))
                {
                    _response.Data = new List<GroupProductAssumptionParameterDto>();
                    _response.IsPassed = true;
                    return _response;
                }

                query = _productAssumptionParameterRepository.GetAll()
                                    .Include(x => x.Program)
                                    .Where(x => !x.IsDeleted);

                query = query.Where(x => progIds.Contains(x.ProgramId));
                query = query.OrderByDescending(x => x.Id);

                var dataList = _mapper.Map<List<ProductAssumptionParameterDto>>(query.ToList());
                var grouped = new List<GroupProductAssumptionParameterDto>();
                var groupdByName = dataList.GroupBy(x => x.ProgramName);
                foreach (var item in groupdByName)
                {
                    grouped.Add(new GroupProductAssumptionParameterDto
                    {
                        ProgramName = item.Key,
                        ProductAssumptionParameterDtos = item.ToList()
                    });
                }

                _response.Data = grouped;
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
        public async Task<IResponseDTO> GetProductAssumptionParameterDetails(int productAssumptionParameterId)
        {
            try
            {
                var productAssumptionParameter = await _productAssumptionParameterRepository.GetAll()
                                        .Include(x => x.ProgramId)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == productAssumptionParameterId);
                if (productAssumptionParameter == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var productAssumptionParameterDto = _mapper.Map<ProductAssumptionParameterDto>(productAssumptionParameter);

                _response.Data = productAssumptionParameterDto;
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
        public async Task<IResponseDTO> CreateProductAssumptionParameter(ProductAssumptionParameterDto productAssumptionParameterDto)
        {
            try
            {
                var productAssumptionParameter = _mapper.Map<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter>(productAssumptionParameterDto);

                // Set relation variables with null to avoid unexpected EF errors
                productAssumptionParameter.Program = null;

                // Add to the DB
                await _productAssumptionParameterRepository.AddAsync(productAssumptionParameter);

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
        public async Task<IResponseDTO> UpdateProductAssumptionParameter(ProductAssumptionParameterDto productAssumptionParameterDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productAssumptionParameterExist = await _productAssumptionParameterRepository.GetFirstAsync(x => x.Id == productAssumptionParameterDto.Id);
                if (productAssumptionParameterExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && productAssumptionParameterExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var productAssumptionParameter = _mapper.Map<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter>(productAssumptionParameterDto);

                // Set relation variables with null to avoid unexpected EF errors

                productAssumptionParameter.Program = null;


                _productAssumptionParameterRepository.Update(productAssumptionParameter);

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
        public async Task<IResponseDTO> UpdateIsActive(int productAssumptionParameterId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productAssumptionParameter = await _productAssumptionParameterRepository.GetFirstAsync(x => x.Id == productAssumptionParameterId);
                if (productAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && productAssumptionParameter.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                productAssumptionParameter.IsActive = IsActive;
                productAssumptionParameter.UpdatedBy = LoggedInUserId;
                productAssumptionParameter.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                productAssumptionParameter.ForecastProductAssumptionValues = null;

                // Update on the Database
                _productAssumptionParameterRepository.Update(productAssumptionParameter);

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
        public async Task<IResponseDTO> RemoveProductAssumptionParameter(int productAssumptionParameterId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var productAssumptionParameter = await _productAssumptionParameterRepository.GetFirstOrDefaultAsync(x => x.Id == productAssumptionParameterId);
                if (productAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && productAssumptionParameter.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                productAssumptionParameter.IsDeleted = true;
                productAssumptionParameter.UpdatedBy = LoggedInUserId;
                productAssumptionParameter.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                productAssumptionParameter.ForecastProductAssumptionValues = null;

                // Update on the Database
                _productAssumptionParameterRepository.Update(productAssumptionParameter);

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
        public GeneratedFile ExportProductAssumptionParameters(int? pageIndex = null, int? pageSize = null, ProductAssumptionParameterFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter> query = null;
            try
            {
                query = _productAssumptionParameterRepository.GetAll()
                                    .Include(x => x.Program)
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
                    if (filterDto.ProgramId > 0)
                    {
                        query = query.Where(x => x.ProgramId == filterDto.ProgramId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (filterDto.IsPercentage != null)
                    {
                        query = query.Where(x => x.IsPercentage == filterDto.IsPercentage);
                    }
                    if (filterDto.IsPositive != null)
                    {
                        query = query.Where(x => x.IsPositive == filterDto.IsPositive);
                    }
                }

                query = query.OrderBy(x => x.Program.Name);

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    if (filterDto.SortProperty.ToLower() == "CountryName".ToLower())
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

                var dataList = _mapper.Map<List<ExportProductAssumptionParameterDto>>(query.ToList());
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
        public async Task<IResponseDTO> ImportProductAssumptionParameters(List<ProductAssumptionParameterDto> productAssumptionParameterDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var productAssumptionParameters_database = _productAssumptionParameterRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var productAssumptionParameters = _mapper.Map<List<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter>>(productAssumptionParameterDtos);

                // vars
                var newProductAssumptionParameters = new List<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter>();
                var updatedProductAssumptionParameters = new List<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter>();

                // Get new and updated productAssumptionParameters
                foreach (var item in productAssumptionParameters)
                {
                    var foundProductAssumptionParameter = productAssumptionParameters_database.FirstOrDefault(x => x.ProgramId == item.ProgramId 
                                                                                                            && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if (foundProductAssumptionParameter == null)
                    {
                        newProductAssumptionParameters.Add(item);
                    }
                    else
                    {
                        updatedProductAssumptionParameters.Add(item);
                    }
                }

                if (!IsSuperAdmin)
                {
                    updatedProductAssumptionParameters = updatedProductAssumptionParameters.Where(x => x.CreatedBy == LoggedInUserId).ToList();
                }

                // Add the new object to the database
                if (newProductAssumptionParameters.Count() > 0)
                {
                    // Set relation variables with null to avoid unexpected EF errors
                    newProductAssumptionParameters.Select(x =>
                    {
                        x.Program = null;
                        x.ForecastProductAssumptionValues = null;
                        x.Creator = null;
                        x.Updator = null;
                        return x;
                    }).ToList();
                    await _productAssumptionParameterRepository.AddRangeAsync(newProductAssumptionParameters);
                }

                // Update the existing objects with the new values
                if (updatedProductAssumptionParameters.Count() > 0)
                {
                    foreach (var item in updatedProductAssumptionParameters)
                    {
                        var fromDatabase = productAssumptionParameters_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                                                    && x.ProgramId == item.ProgramId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }
                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.ProgramId = item.ProgramId;
                         // boolen variables
                        fromDatabase.IsNumeric = item.IsNumeric;
                        fromDatabase.IsPercentage = item.IsPercentage;
                        fromDatabase.IsPositive = item.IsPositive;
                        fromDatabase.IsNegative = item.IsNegative;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Program = null;
                        fromDatabase.ForecastProductAssumptionValues = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _productAssumptionParameterRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newProductAssumptionParameters.Count();
                var numberOfUpdated = updatedProductAssumptionParameters.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = productAssumptionParameterDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = productAssumptionParameterDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public bool IsNameUnique(ProductAssumptionParameterDto productAssumptionParameterDto)
        {
            var searchResult = _productAssumptionParameterRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != productAssumptionParameterDto.Id
                                                && x.Name.ToLower().Trim() == productAssumptionParameterDto.Name.ToLower().Trim()
                                                && x.ProgramId == productAssumptionParameterDto.ProgramId);

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int productAssumptionParameterId)
        {
            try
            {
                var productAssumptionParameter = await _productAssumptionParameterRepository.GetAll()
                                        .Include(x => x.Program)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == productAssumptionParameterId);
                if (productAssumptionParameter == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (productAssumptionParameter.ForecastProductAssumptionValues.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Product Assumption Parameter cannot be deleted or deactivated where it contains 'Forecast Product Assumption Values'";
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
