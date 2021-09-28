using ForLab.Core.Interfaces;
using ForLab.DTO.Security.UserSubscription;
using System.Threading.Tasks;

namespace ForLab.Services.Security.UserSubscription
{
    public interface IUserSubscriptionService
    {
        Task<IResponseDTO> GetUserCountriesAsDrp(int userId);
        Task<IResponseDTO> GetUserRegionsAsDrp(int userId, int countryId);
        Task<IResponseDTO> GetUserLaboratoriesAsDrp(int userId, int countryId);
        IResponseDTO GetAllUserCountrySubscriptions(int? pageIndex = null, int? pageSize = null, UserSubscriptionFilterDto filterDto = null);
        IResponseDTO GetAllUserRegionSubscriptions(int? pageIndex = null, int? pageSize = null, UserSubscriptionFilterDto filterDto = null);
        IResponseDTO GetAllUserLaboratorySubscriptions(int? pageIndex = null, int? pageSize = null, UserSubscriptionFilterDto filterDto = null);
    }
}
