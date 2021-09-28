using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.LaboratoryCategory;
using ForLab.Repositories.Lookup.LaboratoryCategory;
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

namespace ForLab.Services.Lookup.LaboratoryCategory
{
    public class LaboratoryCategoryService : GService<LaboratoryCategoryDto, Data.DbModels.LookupSchema.LaboratoryCategory, ILaboratoryCategoryRepository>, ILaboratoryCategoryService
    {
        private readonly ILaboratoryCategoryRepository _laboratoryCategoryRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportLaboratoryCategoryDto> _fileService;
        private readonly IGeneralService _generalService;
        
        public LaboratoryCategoryService(IMapper mapper,
            IResponseDTO response,
            ILaboratoryCategoryRepository laboratoryCategoryRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
            IFileService<ExportLaboratoryCategoryDto> fileService,
            IGeneralService generalService) : base(laboratoryCategoryRepository, mapper)
        {
            _laboratoryCategoryRepository = laboratoryCategoryRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _generalService = generalService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryCategoryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.LaboratoryCategory> query = null;
            try
            {
                query = _laboratoryCategoryRepository.GetAll()
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    // Security Filter
                    if(!filterDto.IsSuperAdmin)
                    {
                        //var createdBy = _generalService.SuperAdminIds();
                        //createdBy.Add(filterDto.LoggedInUserId);
                        query = query.Where(x => x.CreatedBy == filterDto.LoggedInUserId || x.Shared);
                    }
                    

                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Id);
                var total = query.Count();

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    query = query.OrderBy(
                        string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<LaboratoryCategoryDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(LaboratoryCategoryFilterDto filterDto = null)
        {
            try
            {
                var query = _laboratoryCategoryRepository.GetAll(x => !x.IsDeleted && x.IsActive);


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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.Select(i => new Data.DbModels.LookupSchema.LaboratoryCategory() { Id = i.Id, Name = i.Name });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<LaboratoryCategoryDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetLaboratoryCategoryDetails(int laboratoryCategoryId)
        {
            try
            {
                var laboratoryCategory = await _laboratoryCategoryRepository.GetAll()
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryCategoryId);
                if (laboratoryCategory == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var laboratoryCategoryDto = _mapper.Map<LaboratoryCategoryDto>(laboratoryCategory);

                _response.Data = laboratoryCategoryDto;
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
        public async Task<IResponseDTO> CreateLaboratoryCategory(LaboratoryCategoryDto laboratoryCategoryDto)
        {
            try
            {
                var laboratoryCategory = _mapper.Map<Data.DbModels.LookupSchema.LaboratoryCategory>(laboratoryCategoryDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryCategory.Laboratories = null;

                // Add to the DB
                await _laboratoryCategoryRepository.AddAsync(laboratoryCategory);

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
        public async Task<IResponseDTO> UpdateLaboratoryCategory(LaboratoryCategoryDto laboratoryCategoryDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryCategoryExist = await _laboratoryCategoryRepository.GetFirstAsync(x => x.Id == laboratoryCategoryDto.Id);
                if (laboratoryCategoryExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryCategoryExist.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                var laboratoryCategory = _mapper.Map<Data.DbModels.LookupSchema.LaboratoryCategory>(laboratoryCategoryDto);

                // Set relation variables with null to avoid unexpected EF errors
                laboratoryCategory.Laboratories = null;

                _laboratoryCategoryRepository.Update(laboratoryCategory);

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
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryCategoryId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryCategory = await _laboratoryCategoryRepository.GetFirstAsync(x => x.Id == laboratoryCategoryId);
                if (laboratoryCategory == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryCategory.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                laboratoryCategory.IsActive = IsActive;
                laboratoryCategory.UpdatedBy = LoggedInUserId;
                laboratoryCategory.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                laboratoryCategory.Laboratories = null;

                // Update on the Database
                _laboratoryCategoryRepository.Update(laboratoryCategory);

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
                var items = _laboratoryCategoryRepository.GetAll(x => ids.Contains(x.Id)).ToList();

                if (!IsSuperAdmin && items.Select(x => x.CreatedBy).Any(x => x != LoggedInUserId))
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to update";
                    return _response;
                }

                // Update IsActive value
                var objectsToUpdate = items?.Where(x => x.IsActive != isActive).Select(x =>
                {
                    x.IsActive = isActive;
                    x.UpdatedBy = LoggedInUserId;
                    x.UpdatedOn = DateTime.Now;
                    return x;
                }).ToList();


                // Update on the Database
                foreach(var item in objectsToUpdate)
                {
                    _laboratoryCategoryRepository.Update(item);
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
                var items = _laboratoryCategoryRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _laboratoryCategoryRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveLaboratoryCategory(int laboratoryCategoryId, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                var laboratoryCategory = await _laboratoryCategoryRepository.GetFirstOrDefaultAsync(x => x.Id == laboratoryCategoryId);
                if (laboratoryCategory == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (!IsSuperAdmin && laboratoryCategory.CreatedBy != LoggedInUserId)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "You don't have permission to remove";
                    return _response;
                }

                // Update IsDeleted value
                laboratoryCategory.IsDeleted = true;
                laboratoryCategory.UpdatedBy = LoggedInUserId;
                laboratoryCategory.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                laboratoryCategory.Laboratories = null;

                // Update on the Database
                _laboratoryCategoryRepository.Update(laboratoryCategory);

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
        public async Task<IResponseDTO> ImportLaboratoryCategories(List<LaboratoryCategoryDto> laboratoryCategoryDtos, int LoggedInUserId, bool IsSuperAdmin)
        {
            try
            {
                // Get all not deleted from the database
                var laboratoryCategorys_database = _laboratoryCategoryRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var laboratoryCategorys = _mapper.Map<List<Data.DbModels.LookupSchema.LaboratoryCategory>>(laboratoryCategoryDtos);

                // vars
                var names_database = laboratoryCategorys_database.Select(x => x.Name.ToLower().Trim());
                var names_dto = laboratoryCategorys.Select(x => x.Name.ToLower().Trim());
                // Get the new ones that their names don't exist on the database
                var newLaboratoryCategories = laboratoryCategorys.Where(x => !names_database.Contains(x.Name.ToLower().Trim()));
                // Select the objects that their names already exist in the database
                var updatedLaboratoryCategories = laboratoryCategorys_database.Where(x => names_dto.Contains(x.Name.ToLower().Trim()));
                if(!IsSuperAdmin)
                {
                    updatedLaboratoryCategories = updatedLaboratoryCategories.Where(x => x.CreatedBy == LoggedInUserId);
                }

                // Set relation variables with null to avoid unexpected EF errors
                newLaboratoryCategories.Select(x =>
                {
                    x.Laboratories = null;
                    x.Creator = null;
                    x.Updator = null;
                    return x;
                }).ToList();

                // Add the new object to the database
                if (newLaboratoryCategories.Count() > 0)
                {
                    await _laboratoryCategoryRepository.AddRangeAsync(newLaboratoryCategories);
                }

                // Update the existing objects with the new values
                if (updatedLaboratoryCategories.Count() > 0)
                {
                    foreach (var item in updatedLaboratoryCategories)
                    {
                        var dto = laboratoryCategoryDtos.First(x => x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                        item.UpdatedOn = DateTime.Now;
                        item.UpdatedBy = dto.CreatedBy;
                        item.Name = dto.Name;
                        // Set relation variables with null to avoid unexpected EF errors
                        item.Laboratories = null;
                        item.Creator = null;
                        item.Updator = null;
                        _laboratoryCategoryRepository.Update(item);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newLaboratoryCategories.Count();
                var numberOfUpdated = updatedLaboratoryCategories.Count();

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
                    NumberOfUploded = laboratoryCategoryDtos.Count,
                    NumberOfAdded = numberOfAdded,
                    NumberOfUpdated = numberOfUpdated,
                    NumberOfSkipped = laboratoryCategoryDtos.Count - (numberOfAdded + numberOfUpdated)
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
        public GeneratedFile ExportLaboratoryCategories(int? pageIndex = null, int? pageSize = null, LaboratoryCategoryFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.LaboratoryCategory> query = null;
            try
            {
                query = _laboratoryCategoryRepository.GetAll()
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
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }
                query = query.OrderByDescending(x => x.Name);
                var total = query.Count();


                //Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportLaboratoryCategoryDto>>(query.ToList());
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
        public bool IsNameUnique(LaboratoryCategoryDto laboratoryCategoryDto, int LoggedInUserId, bool IsSuperAdmin)
        {
            var searchResult = _laboratoryCategoryRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != laboratoryCategoryDto.Id
                                                && x.Name.ToLower().Trim() == laboratoryCategoryDto.Name.ToLower().Trim());

            // Security Filter
            if (!IsSuperAdmin)
            {
                var createdBy = _generalService.SuperAdminIds();
                createdBy.Add(LoggedInUserId);
                searchResult = searchResult.Where(x => x.CreatedBy == null || createdBy.Contains(x.CreatedBy.Value));
            }

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int laboratoryCategoryId)
        {
            try
            {
                var laboratoryCategory = await _laboratoryCategoryRepository.GetAll()
                                        .Include(x => x.Laboratories)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == laboratoryCategoryId);
                if (laboratoryCategory == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (laboratoryCategory.Laboratories.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Laboratory Category cannot be deleted or deactivated where it contains 'Laboratories'";
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