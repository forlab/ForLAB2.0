using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Common;
using ForLab.DTO.Security.User;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ForLab.Services.Security.User
{
    public interface IUserService
    {
        Task<IResponseDTO> GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, UserFilterDto filterDto = null);
        Task<IResponseDTO> GetAllUsersAsDrp(string rootPath, UserFilterDto filterDto = null);
        Task<IResponseDTO> GetUserDetails(string rootPath, int userId);
        Task<IResponseDTO> CreateUser(UserDto userDto, IFormFile file);
        Task<IResponseDTO> Register(UserDto userDto, IFormFile file);
        Task<IResponseDTO> UpdateUser(string rootPath, UserDto userDto, IFormFile file);
        Task<IResponseDTO> GetAllRoles();
        Task<IResponseDTO> UpdateUserStatus(int loggedInUserId, int userId, string status, LocationDto locationDto);
        Task<IResponseDTO> MandatoryChangePassword(int loggedInUserId, string email);
        Task<GeneratedFile> ExportUsers(int? pageIndex = null, int? pageSize = null, UserFilterDto filterDto = null);
    }
}
