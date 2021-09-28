using AutoMapper;
using ForLab.Repositories.Security.UserTransactionHistory;
using ForLab.Core.Interfaces;
using ForLab.Data.BaseModeling;
using ForLab.Data.DataContext;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.Data.Enums;
using ForLab.DTO.Common;
using ForLab.DTO.Security.User;
using ForLab.Repositories.UOW;
using ForLab.Services.Global.UploadFiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ForLab.Repositories.Security.UserRole;
using ForLab.Repositories.Configuration.Configuration;
using ForLab.Services.Global.SendEmail;
using Microsoft.EntityFrameworkCore;

namespace ForLab.Services.Security.Account
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IResponseDTO _response;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;
        private readonly IEmailService _emailService;
        private readonly IUserTransactionHistoryRepository _userTransactionHistoryRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountService(
          IUnitOfWork<AppDbContext> unitOfWork,
          IConfiguration configuration,
          IMapper mapper,
          UserManager<ApplicationUser> userManager,
          IPasswordHasher<ApplicationUser> passwordHasher,
          IResponseDTO responseDTO,
          IUploadFilesService uploadFilesService,
          IUserTransactionHistoryRepository userTransactionHistoryRepository,
          IEmailService emailService,
          IConfigurationRepository configurationRepository,
          IUserRoleRepository userRoleRepository,
          RoleManager<ApplicationRole> roleManager)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _response = responseDTO;
            _unitOfWork = unitOfWork;
            _uploadFilesService = uploadFilesService;
            _emailService = emailService;
            _configurationRepository = configurationRepository;
            _userTransactionHistoryRepository = userTransactionHistoryRepository;
            _userRoleRepository = userRoleRepository;
            _roleManager = roleManager;
        }
        

        public async Task<IResponseDTO> Login(string rootPath, LoginParamsDto loginParams)
        {
            try
            {
                var config = await _configurationRepository.GetFirstAsync();
                var appUser = await _userManager.FindByEmailAsync(loginParams.Email);

                if (appUser == null)
                {
                    _response.Message = "Email is not found";
                    _response.IsPassed = false;
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

                if (appUser != null &&
                    _passwordHasher.VerifyHashedPassword(appUser, appUser.PasswordHash, loginParams.Password) !=
                    PasswordVerificationResult.Success)
                {
                    appUser.AccessFailedCount += 1;
                    await _userManager.UpdateAsync(appUser);

                    if (appUser.AccessFailedCount == config.AccountLoginAttempts)
                    {
                        // lock the account
                        appUser.Status = UserStatusEnum.Locked.ToString();
                        await _userManager.UpdateAsync(appUser);

                        // send email here
                        await _emailService.AfterAccountIsDisabled(appUser.Email, DateTime.Now, loginParams.LocationDto.IP, loginParams.LocationDto.CountryName);

                        _response.Message = $"You have unsuccessfully logged into your account {appUser.AccessFailedCount} times and your account has been locked.  Please contact your administration in order to have your account unlocked.";
                        _response.IsPassed = false;
                        return _response;
                    }

                    // send email here
                    //await _emailService.WrongPasswordAttempt(appUser.Email, DateTime.Now, loginParams.LocationDto.IP, loginParams.LocationDto.CountryName);

                    _response.Message = $"Invalid password, you have {config.AccountLoginAttempts - appUser.AccessFailedCount} Attempts then your account will be locked.";
                    _response.IsPassed = false;
                    return _response;
                }

                if (!appUser.EmailConfirmed)
                {
                    _response.Message = "Please check your email inbox for an email from the system administrator in order to confirm your email. If you cannot find the email, please contact the application administrator.";
                    _response.IsPassed = false;
                    return _response;
                }

                if (DateTime.Now.Date >= appUser.NextPasswordExpiryDate.Date)
                {
                    _response.Message = "Your password is expired";
                    _response.IsPassed = false;
                    return _response;
                }

                var authorizedUserDto = _mapper.Map<AuthorizedUserDto>(appUser);

                if (appUser.ChangePassword)
                {
                    var resetPassToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                    // encode the token
                    authorizedUserDto.Token = WebUtility.UrlEncode(resetPassToken);
                }
                else
                {
                    authorizedUserDto.Token = GenerateJSONWebToken(appUser.Id, appUser.UserName);
                }

                authorizedUserDto.UserRoles = new List<string>();

                // get user roles
                authorizedUserDto.UserRoles = GetRoles(appUser.Id);

                if (!string.IsNullOrEmpty(authorizedUserDto.PersonalImagePath))
                {
                    authorizedUserDto.PersonalImagePath = rootPath + authorizedUserDto.PersonalImagePath;
                }


                // Add Transaction here
                await _userTransactionHistoryRepository.AddAsync(new UserTransactionHistory
                {
                    Location = _mapper.Map<Location>(loginParams.LocationDto),
                    CreatedBy = appUser.Id,
                    CreatedOn = DateTime.Now,
                    UserTransactionTypeId = (int)UserTransactionTypesEnum.Login,
                    Description = "User logged in successfully",
                    UserId = appUser.Id,
                    From = "Unauthenticated status",
                    To = "authenticated status",
                    IsDeleted = false,
                });

                var finalResult = await _unitOfWork.CommitAsync();
                if (finalResult == 0)
                {
                    _response.IsPassed = false;
                    _response.Message = "Faild to login";
                    return _response;
                }

                // in case user logged in successfully, reset AccessFailedCount
                if (appUser.AccessFailedCount > 0)
                {
                    appUser.AccessFailedCount = 0;
                    await _userManager.UpdateAsync(appUser);
                }

                _response.IsPassed = true;
                _response.Message = "You are logged in successfully.";
                _response.Data = authorizedUserDto;
            }
            catch (Exception ex)
            {
                _response.IsPassed = false;
                _response.Message = $"Error: {ex.Message} Details: {ex.InnerException?.Message}";
                return _response;
            }

            return _response;
        }
        public async Task<IResponseDTO> ResetPassword(string rootPath, ResetPasswordParamsDto resetPasswordParams)
        {
            try
            {
                var appUser = await _userManager.FindByEmailAsync(resetPasswordParams.Email.Trim());
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

                // appUser.IsPasswordSet = true;
                var result = await _userManager.ResetPasswordAsync(appUser, resetPasswordParams.Token, resetPasswordParams.NewPassword);
                if (!result.Succeeded)
                {
                    _response.IsPassed = false;
                    _response.Message = $"{result.Errors.FirstOrDefault().Description}";
                    return _response;
                }

                var daysToChnagePass = _configurationRepository.GetFirst().NumOfDaysToChangePassword;
                appUser.NextPasswordExpiryDate = DateTime.Now.AddDays(daysToChnagePass);
                appUser.ChangePassword = false;
                appUser.EmailConfirmed = true;
                appUser.EmailVerifiedDate = DateTime.Now;

                await _userManager.UpdateAsync(appUser);

                // send email
                await _emailService.AfterResetPassword(resetPasswordParams.Email.Trim());

                _response.IsPassed = true;
                _response.Message = "Your password is resetted successfully";
            }
            catch (Exception ex)
            {
                _response.IsPassed = false;
                _response.Message = $"Error: {ex.Message} Details: {ex.InnerException?.Message}";
                return _response;
            }

            return _response;
        }
        public async Task<IResponseDTO> ForgetPassword(string email, LocationDto locationDto)
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
        public async Task<IResponseDTO> ChangePassword(ChangePasswordParamsDto userParams)
        {
            var appUser = await _userManager.FindByEmailAsync(userParams.Email.Trim());
            if (appUser == null)
            {
                _response.IsPassed = false;
                _response.Message = "Invalid email or password";
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

            var result = await _userManager.ChangePasswordAsync(appUser, userParams.CurrentPassword, userParams.NewPassword);
            if (!result.Succeeded)
            {
                _response.IsPassed = false;
                _response.Message = $"Code: {result.Errors.FirstOrDefault().Code} {result.Errors.FirstOrDefault().Description}";
                return _response;
            }


            var daysToChnagePass = _configurationRepository.GetFirst().NumOfDaysToChangePassword;
            appUser.NextPasswordExpiryDate = DateTime.Now.AddDays(daysToChnagePass);

            // Update the user and commit
            await _userManager.UpdateAsync(appUser);

            // Send email
            //await _emailService.AfterPasswordChanges(userParams.Email.Trim(), DateTime.Now, userParams.LocationDto.IP, userParams.LocationDto.CountryName);

            _response.IsPassed = true;
            _response.Message = "Done";
            return _response;
        }
        public string GenerateJSONWebToken(int userId, string userName)
        {
            var config = _configurationRepository.GetFirst();
            var signingKey = Convert.FromBase64String(_configuration["Jwt:Key"]);
            //var expiryDuration = int.Parse(_configuration["Jwt:ExpiryDuration"]);
            var expiryDuration = config.PasswordExpiryTime;

            var claims = new List<Claim>
            {
                new Claim("userid", userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userName)
            };

            var user = _userManager.FindByIdAsync(userId.ToString()).Result;
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var item in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, item);
                claims.Add(roleClaim);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,              // Not required as no third-party is involved
                Audience = null,            // Not required as no third-party is involved
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expiryDuration),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(jwtToken);
            return token;
        }
        public async Task<IResponseDTO> GetLoggedInUserProfile(string rootPath, int userId)
        {
            var appUser = await _userManager.Users.Include(x => x.UserSubscriptionLevel).FirstOrDefaultAsync(x => x.Id == userId);
            var userDetailsDto = _mapper.Map<UserDetailsDto>(appUser);
            var userRoles = await _userManager.GetRolesAsync(appUser);
            userDetailsDto.UserRoles = userRoles.ToList();

            if (!string.IsNullOrEmpty(userDetailsDto.PersonalImagePath))
            {
                userDetailsDto.PersonalImagePath = rootPath + userDetailsDto.PersonalImagePath;
            }

            _response.IsPassed = true;
            _response.Data = userDetailsDto;
            return _response;
        }
        public async Task<IResponseDTO> UpdateUserImagePath(int userId, string imagePath)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            appUser.PersonalImagePath = imagePath;

            var result = await _userManager.UpdateAsync(appUser);

            if (!result.Succeeded)
            {
                _response.IsPassed = false;
                _response.Message = $"Code: {result.Errors.FirstOrDefault().Code} {result.Errors.FirstOrDefault().Description}";
                return _response;
            }

            _response.IsPassed = true;
            _response.Message = "Done";
            _response.Data = null;

            return _response;
        }
        public async Task<IResponseDTO> CheckSessionExpiryDate(string Email,DateTime VisitSetPasswordPageDate)
        {
            var appUser = await _userManager.FindByEmailAsync(Email);

            if (appUser == null)
            {
                _response.Message = "There is no user with the specified email";
                _response.Data = null;
                _response.IsPassed = false;
                return _response;

            }

            //session time out !
            var dateOfMailSent = appUser.CreatedOn;
            //var result =  await _configService.TimeToSessionTimeOut();
            //if(!result.IsPassed)
            //{
            //    _response.Data = null;
            //    _response.IsPassed = false;
            //    _response.Message = "Invalid configration settings in configuration table";
            //    return _response;
            //}
            if(VisitSetPasswordPageDate==null)
            {
                _response.Data = null;
                _response.IsPassed = false;
                _response.Message = "Inavild Date";
                return _response;
            }

          

            //var dateBetweenSentMailAndVisitPage = VisitSetPasswordPageDate - dateOfMailSent;
            
            //if ((double)dateBetweenSentMailAndVisitPage.Hours>result.Data)
            //{
            //    _response.Data = null;
            //    _response.IsPassed = false;
            //    _response.Message = "Session time out";
            //    return _response;
            //}

            //_response.Data = "The period betwwen mail has been sent and visting this page is  "+ dateBetweenSentMailAndVisitPage + "Hours";
            _response.IsPassed = true;
            _response.Message = "Session time is allowed";
            return _response;
        }
        public string GenerateJSONWebTokenForResetPass(int userId, string userName)
        {
            //var signingKey = Convert.FromBase64String(_configuration[]);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.
                  GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var resultFromConfigurationTable = _configurationRepository.GetFirst().TimeToSessionTimeOut;
            double expiryDuration = (Convert.ToDouble(resultFromConfigurationTable));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,              // Not required as no third-party is involved
                Audience = null,            // Not required as no third-party is involved
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(expiryDuration),
                Subject = new ClaimsIdentity(new List<Claim>() {
                new Claim("userid", userId.ToString()),
                //new Claim("role", role),
                new Claim(ClaimTypes.NameIdentifier, userName)
            }),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(jwtToken);
            return token;
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
        public async Task<IResponseDTO> ConfirmEmailAddress(string email, string token)
        {
            try
            {
                var appUser = await _userManager.FindByEmailAsync(email.Trim().ToLower());

                if (appUser == null)
                {
                    _response.Message = "Invalid user email";
                    _response.IsPassed = false;
                    return _response;
                }

                var result = await _userManager.ConfirmEmailAsync(appUser, token);
                if (!result.Succeeded)
                {
                    _response.Message = $"{result.Errors.FirstOrDefault().Description}";
                    _response.IsPassed = false;
                    return _response;
                }

                _response.Message = "Ok";
                _response.IsPassed = true;
            }
            catch (Exception ex)
            {
                _response.IsPassed = false;
                _response.Message = $"Error: {ex.Message} Details: {ex.InnerException.Message}";
                return _response;
            }

            return _response;
        }
    }
}
