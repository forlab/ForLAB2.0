using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using ForLab.Core.Interfaces;
using ForLab.Data.DataContext;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.Data.Enums;
using ForLab.DTO.Security.User;
using ForLab.Repositories.Security.UserRole;
using ForLab.Repositories.UOW;
using ForLab.Services.Global.SendEmail;
using ForLab.Services.Global.UploadFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using ForLab.DTO.Common;
using System.Net;
using System.Web;
using ForLab.Repositories.Configuration.Configuration;
using ForLab.Repositories.Security.UserCountrySubscription;
using ForLab.Repositories.Security.UserRegionSubscription;
using ForLab.Repositories.Security.UserLaboratorySubscription;
using ForLab.Core.Common;
using ForLab.Services.Global.FileService;

namespace ForLab.Services.Security.User
{
   public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;
        private readonly IEmailService _emailService;
        private readonly IUserCountrySubscriptionRepository _userCountrySubscriptionRepository;
        private readonly IUserRegionSubscriptionRepository _userRegionSubscriptionRepository;
        private readonly IUserLaboratorySubscriptionRepository _userLaboratorySubscriptionRepository;
        private readonly IFileService<ExportUserDto> _fileService;

        public UserService(
            IUnitOfWork<AppDbContext> unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IResponseDTO responseDTO,
            IUserRoleRepository userRoleRepository,
            RoleManager<ApplicationRole> roleManager,
            IUploadFilesService uploadFilesService,
            IEmailService emailService,
            IConfigurationRepository configurationRepository,
            IUserCountrySubscriptionRepository userCountrySubscriptionRepository,
            IUserRegionSubscriptionRepository userRegionSubscriptionRepository,
            IUserLaboratorySubscriptionRepository userLaboratorySubscriptionRepository,
            IFileService<ExportUserDto> fileService
            )
        {
            _mapper = mapper;
            _userRoleRepository = userRoleRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _response = responseDTO;
            _unitOfWork = unitOfWork;
            _uploadFilesService = uploadFilesService;
            _emailService = emailService;
            _configurationRepository = configurationRepository;
            _userCountrySubscriptionRepository = userCountrySubscriptionRepository;
            _userRegionSubscriptionRepository = userRegionSubscriptionRepository;
            _userLaboratorySubscriptionRepository = userLaboratorySubscriptionRepository;
            _fileService = fileService;
        }


        public async Task<IResponseDTO> GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, UserFilterDto filterDto = null)
        {
            try
            {
                // get users with roles
                IQueryable<ApplicationUser> appUsers = _userManager.Users;

                if (filterDto != null)
                {

                    if (filterDto.RoleId != 0)
                    {
                        var role = _roleManager.Roles.FirstOrDefault(r => r.Id == filterDto.RoleId);
                        if (role != null)
                        {
                            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                            var usersInRoleIds = usersInRole.Select(x => x.Id);
                            appUsers = appUsers.Where(u => usersInRoleIds.Contains(u.Id));
                        }
                    }
                    if (!string.IsNullOrEmpty(filterDto.Email))
                    {
                        appUsers = appUsers.Where(u => u.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.PhoneNumber))
                    {
                        appUsers = appUsers.Where(u => u.PhoneNumber.Trim().ToLower().Contains(filterDto.PhoneNumber.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.JobTitle))
                    {
                        appUsers = appUsers.Where(u => u.JobTitle.Trim().ToLower().Contains(filterDto.JobTitle.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Status))
                    {
                        appUsers = appUsers.Where(u => u.Status.Trim().ToLower().Contains(filterDto.Status.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        appUsers = appUsers.Where(u => u.FirstName.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()) || u.LastName.Contains(filterDto.Name.Trim().ToLower()));
                    }


                    if (filterDto == null || (filterDto != null && string.IsNullOrEmpty(filterDto.SortProperty)))
                    {
                        appUsers = appUsers.OrderByDescending(x => x.Id);
                    }
                }

                var total = appUsers.Count();


                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    appUsers = appUsers.AsQueryable().OrderBy(
                     string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }
                // Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    appUsers = appUsers.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var usersList = _mapper.Map<List<UserDetailsDto>>(appUsers.ToList());

                foreach (var user in usersList)
                {
                    user.UserRoles = GetRoles(user.Id);

                    if (!string.IsNullOrEmpty(user.PersonalImagePath))
                    {
                        user.PersonalImagePath = rootPath + user.PersonalImagePath;
                    }

                }

                _response.Data = new
                {
                    List = usersList,
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
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> GetAllUsersAsDrp(string rootPath, UserFilterDto filterDto = null)
        {
            // get users with roles
            IQueryable<ApplicationUser> appUsers = _userManager.Users.Where(x => x.Status == UserStatusEnum.Active.ToString());

            if (filterDto != null)
            {

                if (filterDto.RoleId != 0)
                {
                    var role = _roleManager.Roles.FirstOrDefault(r => r.Id == filterDto.RoleId);
                    if (role != null)
                    {
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                        var usersInRoleIds = usersInRole.Select(x => x.Id);
                        appUsers = appUsers.Where(u => usersInRoleIds.Contains(u.Id));
                    }
                }
                if (!string.IsNullOrEmpty(filterDto.Email))
                {
                    appUsers = appUsers.Where(u => u.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                }
                if (!string.IsNullOrEmpty(filterDto.Name))
                {
                    appUsers = appUsers.Where(u => u.FirstName.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()) || u.LastName.Contains(filterDto.Name.Trim().ToLower()));
                }


                if (filterDto == null || (filterDto != null && string.IsNullOrEmpty(filterDto.SortProperty)))
                {
                    appUsers = appUsers.OrderByDescending(x => x.Id);
                }
            }

            var usersList = _mapper.Map<List<UserDrp>>(appUsers.ToList());

            foreach (var user in usersList)
            {
                if (!string.IsNullOrEmpty(user.PersonalImagePath))
                {
                    user.PersonalImagePath = rootPath + user.PersonalImagePath;
                }
            }

            _response.Data = usersList;
            _response.Message = "Ok";
            _response.IsPassed = true;

            return _response;
        }
        public async Task<IResponseDTO> GetUserDetails(string rootPath, int userId)
        {
            var appUser = await _userManager.Users.Include(x => x.UserRoles).FirstOrDefaultAsync(x => x.Id == userId);
            var userDetailsDto = _mapper.Map<UserDto>(appUser);

            if (!string.IsNullOrEmpty(userDetailsDto.PersonalImagePath))
            {
                userDetailsDto.PersonalImagePath = rootPath + userDetailsDto.PersonalImagePath;
            }

            _response.IsPassed = true;
            _response.Data = userDetailsDto;
            return _response;
        }
        public async Task<IResponseDTO> CreateUser(UserDto userDto, IFormFile file)
        {
            try
            {
                var config = _configurationRepository.GetFirst();

                // Generate user password
                userDto.Password = GeneratePassword();

                var appUser = _mapper.Map<ApplicationUser>(userDto);
                appUser.UserName = userDto.Email;
                appUser.ChangePassword = true;
                appUser.EmailVerifiedDate = null;
                appUser.NextPasswordExpiryDate = DateTime.Now.AddDays(config.NumOfDaysToChangePassword);
                appUser.UserSubscriptionLevel = null;

                IdentityResult result = await _userManager.CreateAsync(appUser, userDto.Password);
                if (!result.Succeeded)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Code: {result.Errors.FirstOrDefault().Code}, \n Description: {result.Errors.FirstOrDefault().Description}";
                    return _response;
                }

                var path = $"\\Uploads\\Users\\User_{appUser.Id}";
                if (file != null)
                {
                    await _uploadFilesService.UploadFile(path, file, true);
                    appUser.PersonalImagePath = $"\\{path}\\{file.FileName}";
                }

                // Assign roles to the user
                var subscriptionLevel = ((UserSubscriptionLevelsEnum)userDto.UserSubscriptionLevelId).ToString();
                List<ApplicationUserRole> userRoleList = new List<ApplicationUserRole>
                {
                    new ApplicationUserRole
                    {
                        UserId = appUser.Id,
                        RoleId = (int)Enum.Parse(typeof(ApplicationRolesEnum), subscriptionLevel)
                    }
                };

                await _userRoleRepository.AddRangeAsync(userRoleList);

                // Commit to database
                var finalResult = await _unitOfWork.CommitAsync();
                if (finalResult == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Faild to register the user";
                    return _response;
                }

                // Token to reset tha pass
                var resetPassToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                resetPassToken = WebUtility.UrlEncode(resetPassToken);

                // send email
                await _emailService.AfterRegistiration(userDto.Email, resetPassToken);

                _response.IsPassed = true;
                _response.Message = "You are registred successfully";
                _response.Data = _mapper.Map<UserDetailsDto>(appUser);
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> Register(UserDto userDto, IFormFile file)
        {
            try
            {
                var config = _configurationRepository.GetFirst();

                var appUser = _mapper.Map<ApplicationUser>(userDto);
                appUser.UserName = userDto.Email;
                appUser.ChangePassword = false;
                appUser.EmailVerifiedDate = null;
                appUser.NextPasswordExpiryDate = DateTime.Now.AddDays(config.NumOfDaysToChangePassword);
                // Reset
                appUser.UserSubscriptionLevel = null;
                appUser.UserCountrySubscriptions = null;
                appUser.UserRegionSubscriptions = null;
                appUser.UserLaboratorySubscriptions = null;

                IdentityResult result = await _userManager.CreateAsync(appUser, userDto.Password);
                if (!result.Succeeded)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Code: {result.Errors.FirstOrDefault().Code}, \n Description: {result.Errors.FirstOrDefault().Description}";
                    return _response;
                }

                var path = $"\\Uploads\\Users\\User_{appUser.Id}";
                if (file != null)
                {
                    await _uploadFilesService.UploadFile(path, file, true);
                    appUser.PersonalImagePath = $"\\{path}\\{file.FileName}";
                }

                // Assign roles to the user
                var subscriptionLevel = ((UserSubscriptionLevelsEnum)userDto.UserSubscriptionLevelId).ToString();
                List<ApplicationUserRole> userRoleList = new List<ApplicationUserRole>
                {
                    new ApplicationUserRole
                    {
                        UserId = appUser.Id,
                        RoleId = (int)Enum.Parse(typeof(ApplicationRolesEnum), subscriptionLevel),
                        Role = null,
                        User = null
                    }
                };
                await _userRoleRepository.AddRangeAsync(userRoleList);

                // Assign subscription to the user
                if(appUser.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.CountryLevel)
                {
                    var userCountrySubscriptions = _mapper.Map<List<UserCountrySubscription>>(userDto.UserCountrySubscriptionDtos);
                    userCountrySubscriptions.Select(x => 
                    { 
                        x.CreatedBy = appUser.Id; 
                        x.CreatedOn = DateTime.Now; 
                        x.ApplicationUserId = appUser.Id;
                        x.ApplicationUser = null;
                        x.Country = null;
                        return x; }).ToList();
                    await _userCountrySubscriptionRepository.AddRangeAsync(userCountrySubscriptions);
                }
                else if (appUser.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.RegionLevel)
                {
                    var userRegionSubscriptions = _mapper.Map<List<UserRegionSubscription>>(userDto.UserRegionSubscriptionDtos);
                    userRegionSubscriptions.Select(x => 
                    { 
                        x.CreatedBy = appUser.Id; 
                        x.CreatedOn = DateTime.Now; 
                        x.ApplicationUserId = appUser.Id;
                        x.ApplicationUser = null;
                        x.Region = null;
                        return x; }).ToList();
                    await _userRegionSubscriptionRepository.AddRangeAsync(userRegionSubscriptions);
                }
                else if (appUser.UserSubscriptionLevelId == (int)UserSubscriptionLevelsEnum.LaboratoryLevel)
                {
                    var userLaboratorySubscriptions = _mapper.Map<List<UserLaboratorySubscription>>(userDto.UserLaboratorySubscriptionDtos);
                    userLaboratorySubscriptions.Select(x => 
                    { 
                        x.CreatedBy = appUser.Id; 
                        x.CreatedOn = DateTime.Now; 
                        x.ApplicationUserId = appUser.Id;
                        x.ApplicationUser = null;
                        x.Laboratory = null;
                        return x; }).ToList();
                    await _userLaboratorySubscriptionRepository.AddRangeAsync(userLaboratorySubscriptions);
                }

                // Commit to database
                var finalResult = await _unitOfWork.CommitAsync();
                if (finalResult == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Faild to register the user";
                    return _response;
                }

                // Token to verify the email
                var verifyEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                verifyEmailToken = WebUtility.UrlEncode(verifyEmailToken);

                // send email
                await _emailService.SendEmailConfirmationRequest(userDto.Email, verifyEmailToken);

                _response.IsPassed = true;
                _response.Message = "You are registred successfully";
                _response.Data = _mapper.Map<UserDetailsDto>(appUser);
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> UpdateUser(string rootPath, UserDto userDto, IFormFile file)
        {
            try
            {
                var appUser = await _userManager.FindByIdAsync(userDto.Id.ToString());
                var path = $"\\Uploads\\Users\\User_{appUser.Id}";
                if (file != null && !userDto.ReomveProfileImage)
                {
                    await _uploadFilesService.UploadFile(path, file, true);
                    appUser.PersonalImagePath = $"\\{path}\\{file.FileName}";
                }
                else if (userDto.ReomveProfileImage)
                {
                    appUser.PersonalImagePath = null;
                }

                // Old Roles id 
                var oldRoles = _userRoleRepository.GetAll(x => x.UserId == userDto.Id);
                var oldRoleIds = oldRoles.Select(x => x.RoleId);

                // Update the user props
                appUser.FirstName = userDto.FirstName;
                appUser.LastName = userDto.LastName;
                appUser.Email = userDto.Email;
                appUser.PhoneNumber = userDto.PhoneNumber;
                appUser.JobTitle = userDto.JobTitle;
                appUser.Address = userDto.Address;

                // Check User Subscription
                if (userDto.UserSubscriptionLevelId != appUser.UserSubscriptionLevelId)
                {
                    var oldSubscriptionLevel = ((UserSubscriptionLevelsEnum)appUser.UserSubscriptionLevelId).ToString();
                    var newSubscriptionLevel = ((UserSubscriptionLevelsEnum)userDto.UserSubscriptionLevelId).ToString();
                    var oldRoleId = (int)Enum.Parse(typeof(ApplicationRolesEnum), oldSubscriptionLevel);
                    var newRoleId = (int)Enum.Parse(typeof(ApplicationRolesEnum), newSubscriptionLevel);

                    // Add new role
                    await _userRoleRepository.AddAsync(new ApplicationUserRole
                    {
                        UserId = appUser.Id,
                        RoleId = newRoleId
                    });

                    // Remove old role
                    var olduserRole = await _userRoleRepository.GetFirstAsync(x => x.UserId == appUser.Id && x.RoleId == oldRoleId);
                    if(olduserRole != null)
                    {
                        _userRoleRepository.Remove(olduserRole);
                    }
                  
                }

                var result = await _userManager.UpdateAsync(appUser);
                if (!result.Succeeded)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Code: {result.Errors.FirstOrDefault().Code}, \n Description: {result.Errors.FirstOrDefault().Description}";
                    return _response;
                }

                var finalResult = await _unitOfWork.CommitAsync();

                // Res
                var userResult = _mapper.Map<UserDetailsDto>(appUser);
                if(!string.IsNullOrEmpty(userResult.PersonalImagePath))
                {
                    userResult.PersonalImagePath = rootPath + userResult.PersonalImagePath;
                }

                _response.IsPassed = true;
                _response.Message = "Profile is updated successfully";
                _response.Data = userResult;

            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> GetAllRoles()
        {
            try
            {
                var roles = _roleManager.Roles;
                _response.Data = await roles.ToListAsync();
                _response.Message = "Done";
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
        public async Task<IResponseDTO> UpdateUserStatus(int loggedInUserId, int userId, string status, LocationDto locationDto)
        {
            try
            {
                bool isUnLocked = false;

                var appUser = await _userManager.FindByIdAsync(userId.ToString());
                if (appUser == null)
                {
                    _response.IsPassed = false;
                    _response.Message = "User not found";
                    return _response;
                }
                if (appUser.Status == status)
                {
                    _response.IsPassed = false;
                    _response.Message = $"User is already {status}";
                    return _response;
                }


                // check if the admin activate or unlock the user
                isUnLocked = appUser.Status == UserStatusEnum.Locked.ToString() ? true : false;

                appUser.Status = status;
                if (status == UserStatusEnum.Active.ToString())
                {
                    appUser.AccessFailedCount = 0;
                }

                // Update the user in database
                var result = await _userManager.UpdateAsync(appUser);
                if (!result.Succeeded)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Code: {result.Errors.FirstOrDefault().Code}, \n Description: {result.Errors.FirstOrDefault().Description}";
                    return _response;
                }

                // Send email when unlock
                if(status == UserStatusEnum.Active.ToString() && isUnLocked)
                {
                    // Token to reset tha pass
                    var resetPassToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                    resetPassToken = WebUtility.UrlEncode(resetPassToken);

                    appUser.ChangePassword = true;
                    await _userManager.UpdateAsync(appUser);

                    // send email
                    await _emailService.UnlockUserEmail(appUser.Email, resetPassToken);
                }

                _response.IsPassed = true;
                _response.Message = "Done";
                _response.Data = null;
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return _response;
        }
        public async Task<IResponseDTO> MandatoryChangePassword(int loggedInUserId, string email)
        {
            try
            {
                var appUser = await _userManager.FindByEmailAsync(email);
                if (appUser == null)
                {
                    _response.IsPassed = false;
                    _response.Message = "Invalid Email";
                    return _response;
                }

                if (appUser.Status == UserStatusEnum.Locked.ToString())
                {
                    _response.Message = "Your Account is locked. Please contact your administration";
                    _response.IsPassed = false;
                    return _response;
                }

                if (appUser.Status == UserStatusEnum.NotActive.ToString())
                {
                    _response.Message = "Your Account is disabled. Please contact your administration";
                    _response.IsPassed = false;
                    return _response;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                // encode the token
                string validToken = HttpUtility.UrlEncode(token);

                // Commit to DB
                var finalResult = await _unitOfWork.CommitAsync();
                if (finalResult == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Faild to update the user";
                    return _response;
                }

                // send email
                await _emailService.RequestToResetPassword(email, validToken);

                _response.IsPassed = true;
                _response.Message = "Done";
                _response.Data = validToken;
            }
            catch (Exception ex)
            {
                _response.IsPassed = false;
                _response.Message = $"Error: {ex.Message} Details: {ex.InnerException?.Message}";
                return _response;
            }

            return _response;
        }
        public List<string> GetRoles(int userId)
        {
            List<string> result = new List<string>();
            try
            {
                var roleIds = _userRoleRepository.GetAll(x => x.UserId == userId)
                                                              .Select(x => x.RoleId);

                var roleNames = _roleManager.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name);
                result = roleNames.ToList();
            }
            catch (Exception ex)
            {
                var message = $"Error: {ex.Message} Details: {ex.InnerException?.Message}";
            }

            return result;
        }
        public async Task<GeneratedFile> ExportUsers(int? pageIndex = null, int? pageSize = null, UserFilterDto filterDto = null)
        {
            try
            {
                // get users with roles
                IQueryable<ApplicationUser> appUsers = _userManager.Users.Include(x => x.UserSubscriptionLevel);

                if (filterDto != null)
                {

                    if (filterDto.RoleId != 0)
                    {
                        var role = _roleManager.Roles.FirstOrDefault(r => r.Id == filterDto.RoleId);
                        if (role != null)
                        {
                            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                            var usersInRoleIds = usersInRole.Select(x => x.Id);
                            appUsers = appUsers.Where(u => usersInRoleIds.Contains(u.Id));
                        }
                    }
                    if (!string.IsNullOrEmpty(filterDto.Email))
                    {
                        appUsers = appUsers.Where(u => u.Email.Trim().ToLower().Contains(filterDto.Email.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.PhoneNumber))
                    {
                        appUsers = appUsers.Where(u => u.PhoneNumber.Trim().ToLower().Contains(filterDto.PhoneNumber.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.JobTitle))
                    {
                        appUsers = appUsers.Where(u => u.JobTitle.Trim().ToLower().Contains(filterDto.JobTitle.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Status))
                    {
                        appUsers = appUsers.Where(u => u.Status.Trim().ToLower().Contains(filterDto.Status.Trim().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterDto.Name))
                    {
                        appUsers = appUsers.Where(u => u.FirstName.Trim().ToLower().Contains(filterDto.Name.Trim().ToLower()) || u.LastName.Contains(filterDto.Name.Trim().ToLower()));
                    }


                    if (filterDto == null || (filterDto != null && string.IsNullOrEmpty(filterDto.SortProperty)))
                    {
                        appUsers = appUsers.OrderByDescending(x => x.Id);
                    }
                }

                //Check Sort Property
                if (filterDto != null && !string.IsNullOrEmpty(filterDto.SortProperty))
                {
                    appUsers = appUsers.AsQueryable().OrderBy(
                     string.Format("{0} {1}", filterDto.SortProperty, filterDto.IsAscending ? "ASC" : "DESC"));
                }

                // Apply Pagination
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    appUsers = appUsers.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var dataList = _mapper.Map<List<ExportUserDto>>(appUsers.ToList());

                return _fileService.ExportToExcel(dataList);
            }
            catch (Exception ex)
            {
                _response.Data = null;
                _response.Message = "Error " + ex.Message;
                _response.IsPassed = false;
            }

            return null;
        }

        // Helper Methods
        private string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            var result = Regex.Replace(password.ToString(), @"[^0-9a-zA-Z]+", "$");
            result += RandomString(6);

            return result;
        }
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdrfghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }


}


