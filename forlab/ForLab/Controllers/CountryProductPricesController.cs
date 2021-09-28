using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.CountryProductPrice;
using ForLab.Services.Product.CountryProductPrice;
using ForLab.Validators.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class CountryProductPricesController : BaseController
    {
        private readonly ICountryProductPriceService _countryProductPriceService;

        public CountryProductPricesController(
            ICountryProductPriceService countryProductPriceService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _countryProductPriceService = countryProductPriceService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] CountryProductPriceFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _countryProductPriceService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] CountryProductPriceFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _countryProductPriceService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetCountryProductPriceDetails(int countryProductPriceId)
        {
            _response = await _countryProductPriceService.GetCountryProductPriceDetails(countryProductPriceId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateCountryProductPrice([FromBody]CountryProductPriceDto countryProductPriceDto)
        {
            //Validation
            var validationResult = await (new CountryProductPriceValidator(_countryProductPriceService)).ValidateAsync(countryProductPriceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            countryProductPriceDto.Creator = null;
            countryProductPriceDto.Updator = null;
            countryProductPriceDto.CreatedBy = LoggedInUserId;
            countryProductPriceDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            countryProductPriceDto.ProductName = null;
            countryProductPriceDto.CountryName = null;

            _response = await _countryProductPriceService.CreateCountryProductPrice(countryProductPriceDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateCountryProductPrice([FromBody]CountryProductPriceDto countryProductPriceDto)
        {
            // Validation
            var validationResult = await (new CountryProductPriceValidator(_countryProductPriceService)).ValidateAsync(countryProductPriceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            countryProductPriceDto.Creator = null;
            countryProductPriceDto.Updator = null;
            countryProductPriceDto.UpdatedBy = LoggedInUserId;
            countryProductPriceDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            countryProductPriceDto.ProductName = null;
            countryProductPriceDto.CountryName = null;

            _response = await _countryProductPriceService.UpdateCountryProductPrice(countryProductPriceDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int countryProductPriceId, bool isActive)
        {
            //if (!isActive)
            //{
            //    // Validation
            //    var validationResult = await _countryProductPriceService.IsUsed(countryProductPriceId);
            //    if (!validationResult.IsPassed)
            //    {
            //        return validationResult;
            //    }
            //}

            _response = await _countryProductPriceService.UpdateIsActive(countryProductPriceId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveCountryProductPrice(int countryProductPriceId)
        {
            //// Validation
            //var validationResult = await _countryProductPriceService.IsUsed(countryProductPriceId);
            //if (!validationResult.IsPassed)
            //{
            //    return validationResult;
            //}

            _response = await _countryProductPriceService.RemoveCountryProductPrice(countryProductPriceId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportCountryProductPrices([FromBody]List<CountryProductPriceDto> countryProductPriceDtos)
        {
            // Validation
            for (var i = 0; i < countryProductPriceDtos.Count; i++)
            {
                var validationResult = await (new ImportCountryProductPriceValidator()).ValidateAsync(countryProductPriceDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (CountryProductPriceName with CountryId)
            var isDuplicated = countryProductPriceDtos.GroupBy(x => new
            {
                CountryId = x.CountryId,
                ProductId = x.ProductId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                countryProductPriceDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = countryProductPriceDtos.GroupBy(x => new
                {
                    CountryId = x.CountryId,
                    ProductId = x.ProductId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Country with the Product" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Country with the Product";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            countryProductPriceDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.ProductName = null;
                x.CountryName = null;
                return x;
            }).ToList();

            _response = await _countryProductPriceService.ImportCountryProductPrices(countryProductPriceDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportCountryProductPrices(int? pageIndex = null, int? pageSize = null, [FromQuery] CountryProductPriceFilterDto filterDto = null)
        {
            var file = _countryProductPriceService.ExportCountryProductPrices(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}