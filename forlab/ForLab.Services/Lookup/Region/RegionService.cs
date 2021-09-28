using AutoMapper;
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.DTO.Lookup.Region;
using ForLab.Repositories.Lookup.Region;
using ForLab.Repositories.UOW;
using ForLab.Services.Generics;
using ForLab.Services.Global.FileService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.Region
{
    public class RegionService : GService<RegionDto, Data.DbModels.LookupSchema.Region, IRegionRepository>, IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IFileService<ExportRegionDto> _fileService;

        public RegionService(IMapper mapper,
            IResponseDTO response,
            IRegionRepository regionRepository,
            IUnitOfWork<AppDbContext> unitOfWork,
             IFileService<ExportRegionDto> fileService) : base(regionRepository, mapper)
        {
            _regionRepository = regionRepository;
            _response = response;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, RegionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.Region> query = null;
            try
            {
                query = _regionRepository.GetAll()
                                    .Include(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortName))
                    {
                        query = query.Where(x => x.ShortName.Trim().ToLower().Contains(filterDto.ShortName.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Id).ThenBy(x => x.Name);
                var total = query.Count();

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

                var dataList = _mapper.Map<List<RegionDto>>(query.ToList());

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
        public IResponseDTO GetAllAsDrp(RegionFilterDto filterDto = null)
        {
            try
            {
                var query = _regionRepository.GetAll(x => !x.IsDeleted && x.IsActive);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }

                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortName))
                    {
                        query = query.Where(x => x.ShortName.Trim().ToLower().Contains(filterDto.ShortName.Trim().ToLower()));
                    }

                }

                query = query.Select(i => new Data.DbModels.LookupSchema.Region() { Id = i.Id, Name = i.Name, CountryId = i.CountryId });
                query = query.OrderBy(x => x.Name);
                var dataList = _mapper.Map<List<RegionDrp>>(query.ToList());

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
        public async Task<IResponseDTO> GetRegionDetails(int countryId)
        {
            try
            {
                var region = await _regionRepository.GetAll()
                                        .Include(x => x.Country)
                                        .Include(x => x.Creator)
                                        .FirstOrDefaultAsync(x => x.Id == countryId);
                if (region == null)
                {
                    _response.Message = "Invalid object id";
                    _response.IsPassed = false;
                    return _response;
                }

                var regionDto = _mapper.Map<RegionDto>(region);

                _response.Data = regionDto;
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
        public async Task<IResponseDTO> CreateRegion(RegionDto regionDto)
        {
            try
            {
                var region = _mapper.Map<Data.DbModels.LookupSchema.Region>(regionDto);

                // Set relation variables with null to avoid unexpected EF errors
                region.Country = null;

                // Add to the DB
                await _regionRepository.AddAsync(region);

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
        public async Task<IResponseDTO> UpdateRegion(RegionDto regionDto)
        {
            try
            {
                var RegionExist = await _regionRepository.GetFirstAsync(x => x.Id == regionDto.Id);
                if (RegionExist == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid id";
                    return _response;
                }

                var region = _mapper.Map<Data.DbModels.LookupSchema.Region>(regionDto);

                // Set relation variables with null to avoid unexpected EF errors
                region.Country = null;

                _regionRepository.Update(region);

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
        public async Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int regionId, bool IsActive)
        {
            try
            {
                var region = await _regionRepository.GetFirstAsync(x => x.Id == regionId);
                if (region == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsActive value
                region.IsActive = IsActive;
                region.UpdatedBy = loggedInUserId;
                region.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                region.Country = null;
                region.Laboratories = null;
                region.RegionProductPrices = null;

                // Update on the Database
                _regionRepository.Update(region);

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
                var items = _regionRepository.GetAll(x => ids.Contains(x.Id)).ToList();

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
                    _regionRepository.Update(item);
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
        public async Task<IResponseDTO> RemoveRegion(int regionId, int loggedInUserId)
        {
            try
            {
                var region = await _regionRepository.GetFirstOrDefaultAsync(x => x.Id == regionId);
                if (region == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }

                // Update IsDeleted value
                region.IsDeleted = true;
                region.UpdatedBy = loggedInUserId;
                region.UpdatedOn = DateTime.Now;
                // Set children objects with null to avoid unexpected EF errors
                region.Country = null;
                region.Laboratories = null;
                region.RegionProductPrices = null;

                // Update on the Database
                _regionRepository.Update(region);

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
        public async Task<IResponseDTO> ImportRegions(List<RegionDto> regionDtos)
        {
            try
            {
                // Get all not deleted from the database
                var regions_database = _regionRepository.GetAll(x => !x.IsDeleted).ToList();
                // Map DTO to data object
                var regions = _mapper.Map<List<Data.DbModels.LookupSchema.Region>>(regionDtos);

                // vars
                var newRegions = new List<Data.DbModels.LookupSchema.Region>();
                var updatedRegions = new List<Data.DbModels.LookupSchema.Region>();
               
                // Get new and updated regions
                foreach(var item in regions)
                {
                    var foundRegion = regions_database.FirstOrDefault(x => x.CountryId == item.CountryId 
                                                                        && x.Name.ToLower().Trim() == item.Name.ToLower().Trim());
                    if(foundRegion == null)
                    {
                        newRegions.Add(item);
                    }
                    else
                    {
                        updatedRegions.Add(item);
                    }
                }

                // Set relation variables with null to avoid unexpected EF errors
                newRegions.Select(x =>
                {
                    x.Country = null;
                    x.Laboratories = null;
                    x.RegionProductPrices = null;
                    x.UserRegionSubscriptions = null;
                    x.Creator = null;
                    x.Updator = null;
                    return x;
                }).ToList();

                // Add the new object to the database
                if (newRegions.Count() > 0)
                {
                    await _regionRepository.AddRangeAsync(newRegions);
                }

                // Update the existing objects with the new values
                if (updatedRegions.Count() > 0)
                {
                    foreach (var item in updatedRegions)
                    {
                        var fromDatabase = regions_database.FirstOrDefault(x => x.Name.ToLower().Trim() == item.Name?.ToLower()?.Trim()
                                                                             && x.CountryId == item.CountryId);
                        if (fromDatabase == null)
                        {
                            continue;
                        }

                        fromDatabase.UpdatedOn = DateTime.Now;
                        fromDatabase.UpdatedBy = item.CreatedBy;
                        fromDatabase.Name = item.Name;
                        fromDatabase.CountryId = item.CountryId;
                        fromDatabase.ShortName = item.ShortName;
                        // Set relation variables with null to avoid unexpected EF errors
                        fromDatabase.Country = null;
                        fromDatabase.Laboratories = null;
                        fromDatabase.RegionProductPrices = null;
                        fromDatabase.UserRegionSubscriptions = null;
                        fromDatabase.Creator = null;
                        fromDatabase.Updator = null;
                        _regionRepository.Update(fromDatabase);
                    }
                }

                // Calculate the number before commit to the database to get the right numbers
                var numberOfAdded = newRegions.Count();
                var numberOfUpdated = updatedRegions.Count();

                // Commit
                int save = await _unitOfWork.CommitAsync();

                _response.Data = new
                {
                    NumberOfUploded = regionDtos.Count,
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
        public GeneratedFile ExportRegions(int? pageIndex = null, int? pageSize = null, RegionFilterDto filterDto = null)
        {
            IQueryable<Data.DbModels.LookupSchema.Region> query = null;
            try
            {
                query = _regionRepository.GetAll()
                                    .Include(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.CountryId > 0)
                    {
                        query = query.Where(x => x.CountryId == filterDto.CountryId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.ShortName))
                    {
                        query = query.Where(x => x.ShortName.Trim().ToLower().Contains(filterDto.ShortName.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Name);

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

                var dataList = _mapper.Map<List<ExportRegionDto>>(query.ToList());
                var result = _fileService.ExportToExcel(dataList);
                return result;
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
        public bool IsNameUnique(RegionDto regionDto)
        {
            var searchResult = _regionRepository.GetAll(x =>
                                                !x.IsDeleted
                                                && x.Id != regionDto.Id
                                                && x.CountryId == regionDto.CountryId
                                                && x.Name.ToLower().Trim() == regionDto.Name.ToLower().Trim());

            if (searchResult.Count() > 0)
            {
                return false;
            }

            return true;
        }
        public async Task<IResponseDTO> IsUsed(int regionId)
        {
            try
            {
                var region = await _regionRepository.GetAll()
                                        .Include(x => x.Laboratories)
                                        .Include(x => x.RegionProductPrices)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == regionId);
                if (region == null)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Invalid object id";
                    return _response;
                }
                if (region.Laboratories.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Region cannot be deleted or deactivated where it contains 'Laboratories'";
                    return _response;
                }
                if (region.RegionProductPrices.Where(x => !x.IsDeleted).Count() > 0)
                {
                    _response.Data = null;
                    _response.IsPassed = false;
                    _response.Message = "Region cannot be deleted or deactivated where it contains 'Product Prices'";
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
