using ForLab.Core.Interfaces;
using ForLab.Services.Lookup.ThroughPutUnit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ThroughPutUnitsController : BaseController
    {
        private readonly IThroughPutUnitService _throughPutUnitService;

        public ThroughPutUnitsController(
            IThroughPutUnitService throughPutUnitService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _throughPutUnitService = throughPutUnitService;
        }


        [HttpGet]
        public IResponseDTO GetThroughPutUnits()
        {
            _response = _throughPutUnitService.GetThroughPutUnits();
            return _response;
        }
    }
}
