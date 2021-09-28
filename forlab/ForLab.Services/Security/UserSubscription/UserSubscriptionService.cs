using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.Data.Enums;
using ForLab.DTO.Lookup.Country;
using ForLab.DTO.Lookup.Region;
using ForLab.DTO.Security.UserSubscription;
using ForLab.Repositories.Lookup.Region;
using ForLab.Repositories.Security.UserCountrySubscription;
using ForLab.Repositories.Security.UserLaboratorySubscription;
using ForLab.Repositories.Security.UserRegionSubscription;
using ForLab.Repositories.UOW;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ForLab.Services.Security.UserSubscription
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserCountrySubscriptionRepository _userCountrySubscriptionRepository;
        private readonly IUserRegionSubscriptionRepository _userRegionSubscriptionRepository;
        private readonly IUserLaboratorySubscriptionRepository _userLaboratorySubscriptionRepository;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRegionRepository _regionRepository;

        public UserSubscriptionService(
            IMapper mapper,
            IResponseDTO response,
            IUnitOfWork<AppDbContext> unitOfWork,
            UserManager<ApplicationUser> userManager,
            IUserCountrySubscriptionRepository userCountrySubscriptionRepository,
            IUserRegionSubscriptionRepository userRegionSubscriptionRepository,
            IUserLaboratorySubscriptionRepository userLaboratorySubscriptionRepository,
            IRegionRepository regionRepository)
        {
            _response = response;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _userCountrySubscriptionRepository = userCountrySubscriptionRepository;
            _userRegionSubscriptionRepository = userRegionSubscriptionRepository;
            _userLaboratorySubscriptionRepository = userLaboratorySubscriptionRepository;
            _regionRepository = regionRepository;
        }

        public async Task<IResponseDTO> GetUserCountriesAsDrp(int userId)
        {
            try
            {
                // Get user info
                var user = await _userManager.FindByIdAsync(userId.ToString());

                // Check what the UI want
                if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.CountryLevel)
                {
                    var query = _userCountrySubscriptionRepository.GetAll()
                                   .Include(x => x.Country)
                                   .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                   .Select(i => new 
                                   { 
                                       Id = i.Country.Id, 
                                       Name = i.Country.Name, 
                                       CountryPeriodId = i.Country.CountryPeriodId 
                                   })
                                   .Distinct()
                                   .OrderBy(x => x.Name);
                    _response.Data = query?.ToList();
                }
                else if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.RegionLevel)
                {
                    var query = _userRegionSubscriptionRepository.GetAll()
                                       .Include(x => x.Region).ThenInclude(x => x.Country)
                                       .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                       .Select(i => new 
                                       { 
                                           Id = i.Region.Country.Id, 
                                           Name = i.Region.Country.Name, 
                                           CountryPeriodId = i.Region.Country.CountryPeriodId 
                                       })
                                       .Distinct()
                                       .OrderBy(x => x.Name);

                    _response.Data = query?.ToList();
                }
                else if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.LaboratoryLevel)
                {
                    var query = _userLaboratorySubscriptionRepository.GetAll()
                                        .Include(x => x.Laboratory).ThenInclude(x => x.Region).ThenInclude(x => x.Country)
                                        .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                        .Select(i => new 
                                        { 
                                            Id = i.Laboratory.Region.Country.Id, 
                                            Name = i.Laboratory.Region.Country.Name, 
                                            CountryPeriodId = i.Laboratory.Region.Country.CountryPeriodId 
                                        })
                                        .Distinct()
                                        .OrderBy(x => x.Name);

                    if(query.Count() == 0)
                    {
                        var region = _regionRepository.GetAll().Include(x => x.Country).FirstOrDefault(x => x.Id == user.RegionId);
                        _response.Data = new List<CountryDrp>
                        {
                            new CountryDrp
                            {
                                Id = region.Country.Id,
                                Name = region.Country.Name,
                                CountryPeriodId = region.Country.CountryPeriodId
                            }
                        };
                    } 
                    else
                    {
                        _response.Data = query?.ToList();
                    }
                }

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
        public async Task<IResponseDTO> GetUserRegionsAsDrp(int userId, int countryId)
        {
            try
            {
                // Get user info
                var user = await _userManager.FindByIdAsync(userId.ToString());

                // Check what the UI want
                if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.CountryLevel)
                {
                    var query = _userCountrySubscriptionRepository.GetAll()
                                   .Include(x => x.Country).ThenInclude(x => x.Regions)
                                   .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId && x.CountryId == countryId)
                                   .SelectMany(x => x.Country.Regions)
                                   .Where(x => !x.IsDeleted)
                                   .Select(i => new
                                   {
                                       Id = i.Id,
                                       Name = i.Name,
                                       CountryId = i.CountryId
                                   })
                                   .Distinct()
                                   .OrderBy(x => x.Name);
                    _response.Data = query?.ToList();
                }
                else if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.RegionLevel)
                {
                    var query = _userRegionSubscriptionRepository.GetAll()
                                       .Include(x => x.Region).ThenInclude(x => x.Laboratories)
                                       .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                       .Select(x => x.Region)
                                       .Where(x => !x.IsDeleted)
                                       .Select(i => new
                                       {
                                           Id = i.Id,
                                           Name = i.Name,
                                           CountryId = i.CountryId
                                       })
                                       .Distinct()
                                       .OrderBy(x => x.Name);
                    _response.Data = query?.ToList();
                }
                else if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.LaboratoryLevel)
                {
                    var query = _userLaboratorySubscriptionRepository.GetAll()
                                        .Include(x => x.Laboratory).ThenInclude(x => x.Region)
                                        .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                        .Select(i => new
                                        {
                                            Id = i.Laboratory.Region.Id,
                                            Name = i.Laboratory.Region.Name,
                                            CountryId = i.Laboratory.Region.CountryId
                                        })
                                        .Distinct()
                                        .OrderBy(x => x.Name);

                    if (query.Count() == 0)
                    {
                        var region = _regionRepository.GetFirst(x => x.Id == user.RegionId);
                        _response.Data = new List<RegionDrp>
                        {
                            new RegionDrp
                            {
                                Id = region.Id,
                                Name = region.Name,
                                CountryId = region.CountryId
                            }
                        };
                    }
                    else
                    {
                        _response.Data = query?.ToList();
                    }
                }

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
        public async Task<IResponseDTO> GetUserLaboratoriesAsDrp(int userId, int countryId)
        {
            try
            {
                // Get user info
                var user = await _userManager.FindByIdAsync(userId.ToString());

                // Check what the UI want
                if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.CountryLevel)
                {
                    var query = _userCountrySubscriptionRepository.GetAll()
                                   .Include(x => x.Country).ThenInclude(x => x.Regions).ThenInclude(x => x.Laboratories)
                                   .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId && x.CountryId == countryId)
                                   .SelectMany(x => x.Country.Regions)
                                   .SelectMany(x => x.Laboratories)
                                   .Where(x => !x.IsDeleted && x.IsActive)
                                   .Select(i => new
                                   {
                                       Id = i.Id,
                                       Name = i.Name,
                                       RegionId = i.RegionId
                                   })
                                   .Distinct()
                                   .OrderBy(x => x.Name);
                    _response.Data = query?.ToList();
                }
                else if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.RegionLevel)
                {
                    var query = _userRegionSubscriptionRepository.GetAll()
                                       .Include(x => x.Region).ThenInclude(x => x.Laboratories)
                                       .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                       .SelectMany(x => x.Region.Laboratories)
                                       .Where(x => !x.IsDeleted && x.IsActive)
                                       .Select(i => new
                                        {
                                            Id = i.Id,
                                            Name = i.Name,
                                            RegionId = i.RegionId
                                       })
                                       .Distinct()
                                       .OrderBy(x => x.Name);
                    _response.Data = query?.ToList();
                }
                else if (user.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.LaboratoryLevel)
                {
                    var query = _userLaboratorySubscriptionRepository.GetAll()
                                        .Include(x => x.Laboratory)
                                        .Where(x => !x.IsDeleted && x.IsActive && x.ApplicationUserId == userId)
                                        .Select(i => new
                                        {
                                            Id = i.Laboratory.Id,
                                            Name = i.Laboratory.Name,
                                            RegionId = i.Laboratory.RegionId
                                        })
                                        .Distinct()
                                        .OrderBy(x => x.Name);
                    _response.Data = query?.ToList();
                }

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
        public IResponseDTO GetAllUserCountrySubscriptions(int? pageIndex = null, int? pageSize = null, UserSubscriptionFilterDto filterDto = null)
        {
            IQueryable<UserCountrySubscription> query = null;
            try
            {
                query = _userCountrySubscriptionRepository.GetAll()
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
                    if (filterDto.ApplicationUserId > 0)
                    {
                        query = query.Where(x => x.ApplicationUserId == filterDto.ApplicationUserId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Country.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Country.Name);
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

                var dataList = _mapper.Map<List<UserCountrySubscriptionDto>>(query.ToList());

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
        public IResponseDTO GetAllUserRegionSubscriptions(int? pageIndex = null, int? pageSize = null, UserSubscriptionFilterDto filterDto = null)
        {
            IQueryable<UserRegionSubscription> query = null;
            try
            {
                query = _userRegionSubscriptionRepository.GetAll()
                                    .Include(x => x.Region).ThenInclude(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.RegionId > 0)
                    {
                        query = query.Where(x => x.RegionId == filterDto.RegionId);
                    }
                    if (filterDto.ApplicationUserId > 0)
                    {
                        query = query.Where(x => x.ApplicationUserId == filterDto.ApplicationUserId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Region.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Region.Name);
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

                var dataList = _mapper.Map<List<UserRegionSubscriptionDto>>(query.ToList());

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
        public IResponseDTO GetAllUserLaboratorySubscriptions(int? pageIndex = null, int? pageSize = null, UserSubscriptionFilterDto filterDto = null)
        {
            IQueryable<UserLaboratorySubscription> query = null;
            try
            {
                query = _userLaboratorySubscriptionRepository.GetAll()
                                    .Include(x => x.Laboratory).ThenInclude(x => x.Region).ThenInclude(x => x.Country)
                                    .Include(x => x.Creator)
                                    .Where(x => !x.IsDeleted);

                if (filterDto != null)
                {
                    if (filterDto.IsActive != null)
                    {
                        query = query.Where(x => x.IsActive == filterDto.IsActive);
                    }
                    if (filterDto.LaboratoryId > 0)
                    {
                        query = query.Where(x => x.LaboratoryId == filterDto.LaboratoryId);
                    }
                    if (filterDto.ApplicationUserId > 0)
                    {
                        query = query.Where(x => x.ApplicationUserId == filterDto.ApplicationUserId);
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        query = query.Where(x => x.Laboratory.Name.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()));
                    }
                }

                query = query.OrderByDescending(x => x.Laboratory.Name);
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

                var dataList = _mapper.Map<List<UserLaboratorySubscriptionDto>>(query.ToList());

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
    }
}
