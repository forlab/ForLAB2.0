using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.LaboratoryProductPrice;
using ForLab.Services.Product.LaboratoryProductPrice;
using ForLab.Validators.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryProductPricesController : BaseController
    {
        private readonly ILaboratoryProductPriceService _laboratoryProductPriceService;

        public LaboratoryProductPricesController(
            ILaboratoryProductPriceService laboratoryProductPriceService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryProductPriceService = laboratoryProductPriceService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryProductPriceFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryProductPriceService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryProductPriceFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryProductPriceService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryProductPriceDetails(int laboratoryProductPriceId)
        {
            _response = await _laboratoryProductPriceService.GetLaboratoryProductPriceDetails(laboratoryProductPriceId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryProductPrice([FromBody]LaboratoryProductPriceDto laboratoryProductPriceDto)
        {
            //Validation
            var validationResult = await (new LaboratoryProductPriceValidator(_laboratoryProductPriceService)).ValidateAsync(laboratoryProductPriceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryProductPriceDto.Creator = null;
            laboratoryProductPriceDto.Updator = null;
            laboratoryProductPriceDto.CreatedBy = LoggedInUserId;
            laboratoryProductPriceDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryProductPriceDto.ProductName = null;
            laboratoryProductPriceDto.LaboratoryName = null;

            _response = await _laboratoryProductPriceService.CreateLaboratoryProductPrice(laboratoryProductPriceDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryProductPrice([FromBody]LaboratoryProductPriceDto laboratoryProductPriceDto)
        {
            // Validation
            var validationResult = await (new LaboratoryProductPriceValidator(_laboratoryProductPriceService)).ValidateAsync(laboratoryProductPriceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryProductPriceDto.Creator = null;
            laboratoryProductPriceDto.Updator = null;
            laboratoryProductPriceDto.UpdatedBy = LoggedInUserId;
            laboratoryProductPriceDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryProductPriceDto.ProductName = null;
            laboratoryProductPriceDto.LaboratoryName = null;

            _response = await _laboratoryProductPriceService.UpdateLaboratoryProductPrice(laboratoryProductPriceDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryProductPriceId, bool isActive)
        {
            //if (!isActive)
            //{
            //    // Validation
            //    var validationResult = await _laboratoryProductPriceService.IsUsed(laboratoryProductPriceId);
            //    if (!validationResult.IsPassed)
            //    {
            //        return validationResult;
            //    }
            //}

            _response = await _laboratoryProductPriceService.UpdateIsActive(laboratoryProductPriceId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryProductPrice(int laboratoryProductPriceId)
        {
            //// Validation
            //var validationResult = await _laboratoryProductPriceService.IsUsed(laboratoryProductPriceId);
            //if (!validationResult.IsPassed)
            //{
            //    return validationResult;
            //}

            _response = await _laboratoryProductPriceService.RemoveLaboratoryProductPrice(laboratoryProductPriceId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }



        [HttpPost]
        public IActionResult ExportLaboratoryProductPrices(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryProductPriceFilterDto filterDto = null)
        {
            var file = _laboratoryProductPriceService.ExportLaboratoryProductPrices(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryProductPrices([FromBody]List<LaboratoryProductPriceDto> laboratoryProductPriceDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryProductPriceDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryProductPriceValidator()).ValidateAsync(laboratoryProductPriceDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (LaboratoryProductPriceName with CountryId)
            var isDuplicated = laboratoryProductPriceDtos.GroupBy(x => new
            {
                LaboratoryId = x.LaboratoryId,
                ProductId = x.ProductId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                laboratoryProductPriceDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = laboratoryProductPriceDtos.GroupBy(x => new
                {
                    LaboratoryId = x.LaboratoryId,
                    ProductId = x.ProductId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Laboratory with the product" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Laboratory with the product";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryProductPriceDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.ProductName = null;
                x.LaboratoryName = null;
                return x;
            }).ToList();

            _response = await _laboratoryProductPriceService.ImportLaboratoryProductPrices(laboratoryProductPriceDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
