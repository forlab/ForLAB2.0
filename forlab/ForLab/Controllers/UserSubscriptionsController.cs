using ForLab.Core.Interfaces;
using ForLab.DTO.Security.UserSubscription;
using ForLab.Services.Security.UserSubscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class UserSubscriptionsController : BaseController
    {
        private readonly IUserSubscriptionService _userSubscriptionService;

        public UserSubscriptionsController(
            IUserSubscriptionService userSubscriptionService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _userSubscriptionService = userSubscriptionService;
        }


        [HttpGet]
        public IResponseDTO GetAllUserCountrySubscriptions(int? pageIndex, int? pageSize, [FromQuery] UserSubscriptionFilterDto filterDto)
        {
            _response = _userSubscriptionService.GetAllUserCountrySubscriptions(pageIndex, pageSize, filterDto);
            return _response;
        }
         
        [HttpGet]
        public IResponseDTO GetAllUserRegionSubscriptions(int? pageIndex, int? pageSize, [FromQuery] UserSubscriptionFilterDto filterDto)
        {
            _response = _userSubscriptionService.GetAllUserRegionSubscriptions(pageIndex, pageSize, filterDto);
            return _response;
        }

        [HttpGet]
        public IResponseDTO GetAllUserLaboratorySubscriptions(int? pageIndex, int? pageSize, [FromQuery] UserSubscriptionFilterDto filterDto)
        {
            _response = _userSubscriptionService.GetAllUserLaboratorySubscriptions(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetUserCountriesAsDrp(int applicationUserId)
        {
            _response = await _userSubscriptionService.GetUserCountriesAsDrp(applicationUserId);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetUserRegionsAsDrp(int applicationUserId, int countryId)
        {
            _response = await _userSubscriptionService.GetUserRegionsAsDrp(applicationUserId, countryId);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetUserLaboratoriesAsDrp(int applicationUserId, int countryId)
        {
            _response = await _userSubscriptionService.GetUserLaboratoriesAsDrp(applicationUserId, countryId);
            return _response;
        }

    }
}
