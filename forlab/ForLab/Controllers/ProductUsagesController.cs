using ForLab.Core.Interfaces;
using ForLab.DTO.Product.ProductUsage;
using ForLab.Services.Product.ProductUsage;
using ForLab.Validators.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ProductUsagesController : BaseController
    {
        private readonly IProductUsageService _productUsageService;

        public ProductUsagesController(
            IProductUsageService productUsageService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _productUsageService = productUsageService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ProductUsageFilterDto filterDto)
        {
            _response = _productUsageService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ProductUsageFilterDto filterDto)
        {
            _response = _productUsageService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetProductUsageDetails(int productUsageId)
        {
            _response = await _productUsageService.GetProductUsageDetails(productUsageId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateProductUsage([FromBody]ProductUsageDto productUsageDto)
        {
            //Validation
            var validationResult = await (new ProductUsageValidator(_productUsageService)).ValidateAsync(productUsageDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            productUsageDto.Creator = null;
            productUsageDto.Updator = null;
            productUsageDto.CreatedBy = LoggedInUserId;
            productUsageDto.CreatedOn = DateTime.Now;
            //productUsageDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            productUsageDto.InstrumentName = null;
            productUsageDto.CountryPeriodName = null;
            productUsageDto.ProductName = null;
            productUsageDto.TestName = null;

            _response = await _productUsageService.CreateProductUsage(productUsageDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateProductUsage([FromBody]ProductUsageDto productUsageDto)
        {
            // Validation
            var validationResult = await (new ProductUsageValidator(_productUsageService)).ValidateAsync(productUsageDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            productUsageDto.Creator = null;
            productUsageDto.Updator = null;
            productUsageDto.UpdatedBy = LoggedInUserId;
            productUsageDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            productUsageDto.InstrumentName = null;
            productUsageDto.CountryPeriodName = null;
            productUsageDto.ProductName = null;
            productUsageDto.TestName = null;


            _response = await _productUsageService.UpdateProductUsage(productUsageDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int productUsageId, bool isActive)
        {
            //if (!isActive)
            //{
            //    // Validation
            //    var validationResult = await _productUsageService.IsUsed(productUsageId);
            //    if (!validationResult.IsPassed)
            //    {
            //        return validationResult;
            //    }
            //}

            _response = await _productUsageService.UpdateIsActive(productUsageId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveProductUsage(int productUsageId)
        {
            //// Validation
            //var validationResult = await _productUsageService.IsUsed(productUsageId);
            //if (!validationResult.IsPassed)
            //{
            //    return validationResult;
            //}

            _response = await _productUsageService.RemoveProductUsage(productUsageId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportProductUsages(int? pageIndex = null, int? pageSize = null, [FromQuery] ProductUsageFilterDto filterDto = null)
        {
            var file = _productUsageService.ExportProductUsages(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportProductUsages([FromBody]List<ProductUsageDto> productUsageDtos, bool isProduct)
        {
            // Validation
            for (var i = 0; i < productUsageDtos.Count; i++)
            {
                var validationResult = await (new ImportProductUsageValidator()).ValidateAsync(productUsageDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = new List<object> 
                    {
                        new { RowNumber = i + 1, Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct()) }
                    };
                    return _response;
                }
            }

            // Check the duplication
            if(isProduct)
            {
                // First
                var isDuplicated = productUsageDtos.Where(x => x.PerPeriod).GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    PerPeriod = x.PerPeriod,
                    CountryPeriodId = x.CountryPeriodId
                }).Any(g => g.Count() > 1);
                if (isDuplicated)
                {
                    productUsageDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                    var duplicates = productUsageDtos.GroupBy(x => new
                    {
                        ProductId = x.ProductId,
                        PerPeriod = x.PerPeriod,
                        CountryPeriodId = x.CountryPeriodId
                    }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                    _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Product with the same Period" });
                    _response.IsPassed = false;
                    _response.Message = "You should not duplicate the Product with the same Period";
                    return _response;
                }

                // Second
                var isDuplicated2 = productUsageDtos.Where(x => x.PerPeriodPerInstrument).GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    PerPeriodPerInstrument = x.PerPeriodPerInstrument,
                    InstrumentId = x.InstrumentId
                }).Any(g => g.Count() > 1);
                if (isDuplicated2)
                {
                    productUsageDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                    var duplicates = productUsageDtos.GroupBy(x => new
                    {
                        ProductId = x.ProductId,
                        PerPeriodPerInstrument = x.PerPeriodPerInstrument,
                        InstrumentId = x.InstrumentId
                    }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                    _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Product with the same Instrument" });
                    _response.IsPassed = false;
                    _response.Message = "You should not duplicate the Product with the same Instrument";
                    return _response;
                }
            } 
            else 
            {
                var isDuplicated = productUsageDtos.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    TestId = x.TestId,
                    InstrumentId = x.InstrumentId
                }).Any(g => g.Count() > 1);
                if (isDuplicated)
                {
                    productUsageDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                    var duplicates = productUsageDtos.GroupBy(x => new
                    {
                        ProductId = x.ProductId,
                        TestId = x.TestId,
                        InstrumentId = x.InstrumentId
                    }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                    _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Product with the same Test and Instrument" });
                    _response.IsPassed = false;
                    _response.Message = "You should not duplicate the Product with the same Test and Instrument";
                    return _response;
                }
            }


            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            productUsageDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.ProductName = null;
                x.InstrumentName = null;
                x.CountryPeriodName = null;
                x.TestName = null;
                return x;
            }).ToList();

            _response = await _productUsageService.ImportProductUsages(productUsageDtos, isProduct, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
