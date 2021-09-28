using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.RegionProductPrice;
using ForLab.Services.Product.RegionProductPrice;
using ForLab.Validators.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class RegionProductPricesController : BaseController
    {
        private readonly IRegionProductPriceService _regionProductPriceService;

        public RegionProductPricesController(
            IRegionProductPriceService regionProductPriceService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _regionProductPriceService = regionProductPriceService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] RegionProductPriceFilterDto filterDto)
        {
            _response = _regionProductPriceService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] RegionProductPriceFilterDto filterDto)
        {
            _response = _regionProductPriceService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetRegionProductPriceDetails(int regionProductPriceId)
        {
            _response = await _regionProductPriceService.GetRegionProductPriceDetails(regionProductPriceId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateRegionProductPrice([FromBody]RegionProductPriceDto regionProductPriceDto)
        {
            //Validation
            var validationResult = await (new RegionProductPriceValidator(_regionProductPriceService)).ValidateAsync(regionProductPriceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            regionProductPriceDto.Creator = null;
            regionProductPriceDto.Updator = null;
            regionProductPriceDto.CreatedBy = LoggedInUserId;
            regionProductPriceDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            regionProductPriceDto.ProductName = null;
            regionProductPriceDto.RegionName = null;

            _response = await _regionProductPriceService.CreateRegionProductPrice(regionProductPriceDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateRegionProductPrice([FromBody]RegionProductPriceDto regionProductPriceDto)
        {
            // Validation
            var validationResult = await (new RegionProductPriceValidator(_regionProductPriceService)).ValidateAsync(regionProductPriceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            regionProductPriceDto.Creator = null;
            regionProductPriceDto.Updator = null;
            regionProductPriceDto.UpdatedBy = LoggedInUserId;
            regionProductPriceDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            regionProductPriceDto.ProductName = null;
            regionProductPriceDto.RegionName = null;

            _response = await _regionProductPriceService.UpdateRegionProductPrice(regionProductPriceDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int regionProductPriceId, bool isActive)
        {
            //if (!isActive)
            //{
            //    // Validation
            //    var validationResult = await _regionProductPriceService.IsUsed(regionProductPriceId);
            //    if (!validationResult.IsPassed)
            //    {
            //        return validationResult;
            //    }
            //}

            _response = await _regionProductPriceService.UpdateIsActive(regionProductPriceId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveRegionProductPrice(int regionProductPriceId)
        {
            //// Validation
            //var validationResult = await _regionProductPriceService.IsUsed(regionProductPriceId);
            //if (!validationResult.IsPassed)
            //{
            //    return validationResult;
            //}

            _response = await _regionProductPriceService.RemoveRegionProductPrice(regionProductPriceId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportRegionProductPrices(int? pageIndex = null, int? pageSize = null, [FromQuery] RegionProductPriceFilterDto filterDto = null)
        {
            var file = _regionProductPriceService.ExportRegionProductPrices(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

     
        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportRegionProductPrices([FromBody]List<RegionProductPriceDto> regionProductPriceDtos)
        {
            // Validation
            for (var i = 0; i < regionProductPriceDtos.Count; i++)
            {
                var validationResult = await (new ImportRegionProductPriceValidator()).ValidateAsync(regionProductPriceDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (RegionProductPriceName with CountryId)
            var duplicates = regionProductPriceDtos.GroupBy(x => new
            {
                RegionId = x.RegionId,
                ProductId = x.ProductId
            }).Any(g => g.Count() > 1);
            if (duplicates)
            {
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Region with the Product";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            regionProductPriceDtos.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.ProductName = null;
                x.RegionName = null;
                return x;
            }).ToList();

            _response = await _regionProductPriceService.ImportRegionProductPrices(regionProductPriceDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


    }
}
