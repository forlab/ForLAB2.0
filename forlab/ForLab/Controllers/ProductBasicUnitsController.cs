using ForLab.Core.Interfaces;
using ForLab.Services.Lookup.ProductBasicUnit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ProductBasicUnitsController : BaseController
    {
        private readonly IProductBasicUnitService _productBasicUnitService;

        public ProductBasicUnitsController(
            IProductBasicUnitService productBasicUnitService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _productBasicUnitService = productBasicUnitService;
        }


        [HttpGet]
        public IResponseDTO GetProductBasicUnits()
        {
            _response = _productBasicUnitService.GetProductBasicUnits();
            return _response;
        }
    }
}
