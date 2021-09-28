using ForLab.Core.Interfaces;
using ForLab.DTO.Common;
using ForLab.DTO.Security.User;
using System;
using System.Threading.Tasks;

namespace ForLab.Services.Security.Account
{
   public  interface IAccountService
    {
        string GenerateJSONWebToken(int userId, string userName);
        Task<IResponseDTO> Login(string rootPath, LoginParamsDto loginParams);
        Task<IResponseDTO> ResetPassword(string rootPath,ResetPasswordParamsDto resetPasswordParams);
        Task<IResponseDTO> ForgetPassword(string email, LocationDto locationDto);
        Task<IResponseDTO> ChangePassword(ChangePasswordParamsDto userParams);
        Task<IResponseDTO> GetLoggedInUserProfile(string rootPath, int userId);
        Task<IResponseDTO> UpdateUserImagePath(int userId, string imagePath);
        Task<IResponseDTO> CheckSessionExpiryDate(string Email, DateTime VisitSetPageDate);
        string GenerateJSONWebTokenForResetPass(int userId, string userName);
        Task<IResponseDTO> ConfirmEmailAddress(string email, string token);
    }
}
