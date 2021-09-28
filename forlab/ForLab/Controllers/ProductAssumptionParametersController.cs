using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Services.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Validators.ProductAssumptionParameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ProductAssumptionParametersController : BaseController
    {
        private readonly IProductAssumptionParameterService _productAssumptionParameterService;

        public ProductAssumptionParametersController(
            IProductAssumptionParameterService productAssumptionParameterService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _productAssumptionParameterService = productAssumptionParameterService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ProductAssumptionParameterFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _productAssumptionParameterService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ProductAssumptionParameterFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _productAssumptionParameterService.GetAllAsDrp(filterDto);
            return _response;
        }

        [HttpGet]
        public IResponseDTO GetAllProductAssumptionsForForcast(string programIds)
        {
            _response = _productAssumptionParameterService.GetAllProductAssumptionsForForcast(programIds);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetProductAssumptionParameterDetails(int productAssumptionParameterId)
        {
            _response = await _productAssumptionParameterService.GetProductAssumptionParameterDetails(productAssumptionParameterId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateProductAssumptionParameter([FromBody]ProductAssumptionParameterDto productAssumptionParameterDto)
        {
            //Validation
            var validationResult = await (new ProductAssumptionParameterValidator(_productAssumptionParameterService)).ValidateAsync(productAssumptionParameterDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            productAssumptionParameterDto.Creator = null;
            productAssumptionParameterDto.Updator = null;
            productAssumptionParameterDto.CreatedBy = LoggedInUserId;
            productAssumptionParameterDto.CreatedOn = DateTime.Now;
            //productAssumptionParameterDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            productAssumptionParameterDto.ProgramName = null;

            _response = await _productAssumptionParameterService.CreateProductAssumptionParameter(productAssumptionParameterDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateProductAssumptionParameter([FromBody]ProductAssumptionParameterDto productAssumptionParameterDto)
        {
            // Validation
            var validationResult = await (new ProductAssumptionParameterValidator(_productAssumptionParameterService)).ValidateAsync(productAssumptionParameterDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            productAssumptionParameterDto.Creator = null;
            productAssumptionParameterDto.Updator = null;
            productAssumptionParameterDto.UpdatedBy = LoggedInUserId;
            productAssumptionParameterDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            productAssumptionParameterDto.ProgramName = null;

            _response = await _productAssumptionParameterService.UpdateProductAssumptionParameter(productAssumptionParameterDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int productAssumptionParameterId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _productAssumptionParameterService.IsUsed(productAssumptionParameterId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _productAssumptionParameterService.UpdateIsActive(productAssumptionParameterId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveProductAssumptionParameter(int productAssumptionParameterId)
        {
            // Validation
            var validationResult = await _productAssumptionParameterService.IsUsed(productAssumptionParameterId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _productAssumptionParameterService.RemoveProductAssumptionParameter(productAssumptionParameterId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        

        [HttpPost]
        public IActionResult ExportProductAssumptionParameters(int? pageIndex = null, int? pageSize = null, [FromQuery] ProductAssumptionParameterFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _productAssumptionParameterService.ExportProductAssumptionParameters(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportProductAssumptionParameters([FromBody]List<ProductAssumptionParameterDto> productAssumptionParameterDtos)
        {
            // Validation
            for (var i = 0; i < productAssumptionParameterDtos.Count; i++)
            {
                var validationResult = await (new ImportProductAssumptionParameterValidator()).ValidateAsync(productAssumptionParameterDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (ProductAssumptionParameterName with CountryId)
            var isDuplicated = productAssumptionParameterDtos.GroupBy(x => new
            {
                Name = x.Name.Trim().ToLower(),
                ProgramId = x.ProgramId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                productAssumptionParameterDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = productAssumptionParameterDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    ProgramId = x.ProgramId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Product Assumption Parameter name with the Program" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Product Assumption Parameter name with the Program";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            productAssumptionParameterDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                //x.IsActive = IsSuperAdmin ? true : false;
                x.ProgramName = null;
                return x;
            }).ToList();

            _response = await _productAssumptionParameterService.ImportProductAssumptionParameters(productAssumptionParameterDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
    }
}
