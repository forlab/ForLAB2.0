using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryConsumption;
using ForLab.Services.Laboratory.LaboratoryConsumption;
using ForLab.Validators.LaboratoryConsumption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryConsumptionsController : BaseController
    {
        private readonly ILaboratoryConsumptionService _laboratoryConsumptionService;

        public LaboratoryConsumptionsController(
            ILaboratoryConsumptionService laboratoryConsumptionService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryConsumptionService = laboratoryConsumptionService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryConsumptionFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryConsumptionService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryConsumptionFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryConsumptionService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryConsumptionDetails(int laboratoryConsumptionId)
        {
            _response = await _laboratoryConsumptionService.GetLaboratoryConsumptionDetails(laboratoryConsumptionId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryConsumption([FromBody]LaboratoryConsumptionDto laboratoryConsumptionDto)
        {
            //Validation
            var validationResult = await (new LaboratoryConsumptionValidator(_laboratoryConsumptionService)).ValidateAsync(laboratoryConsumptionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryConsumptionDto.Creator = null;
            laboratoryConsumptionDto.Updator = null;
            laboratoryConsumptionDto.CreatedBy = LoggedInUserId;
            laboratoryConsumptionDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryConsumptionDto.LaboratoryName = null;
            laboratoryConsumptionDto.ProductName = null;

            _response = await _laboratoryConsumptionService.CreateLaboratoryConsumption(laboratoryConsumptionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryConsumption([FromBody]LaboratoryConsumptionDto laboratoryConsumptionDto)
        {
            // Validation
            var validationResult = await (new LaboratoryConsumptionValidator(_laboratoryConsumptionService)).ValidateAsync(laboratoryConsumptionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryConsumptionDto.Creator = null;
            laboratoryConsumptionDto.Updator = null;
            laboratoryConsumptionDto.UpdatedBy = LoggedInUserId;
            laboratoryConsumptionDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryConsumptionDto.LaboratoryName = null;
            laboratoryConsumptionDto.ProductName = null;

            _response = await _laboratoryConsumptionService.UpdateLaboratoryConsumption(laboratoryConsumptionDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryConsumptionId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryConsumptionService.IsUsed(laboratoryConsumptionId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryConsumptionService.UpdateIsActive(laboratoryConsumptionId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryConsumption(int laboratoryConsumptionId)
        {
            // Validation
            var validationResult = await _laboratoryConsumptionService.IsUsed(laboratoryConsumptionId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryConsumptionService.RemoveLaboratoryConsumption(laboratoryConsumptionId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        
        [HttpPost]
        public IActionResult ExportLaboratoryConsumptions(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryConsumptionFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _laboratoryConsumptionService.ExportLaboratoryConsumptions(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryConsumptions([FromBody]List<LaboratoryConsumptionDto> laboratoryConsumptionDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryConsumptionDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryConsumptionValidator()).ValidateAsync(laboratoryConsumptionDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (LaboratoryConsumptionName with CountryId)
            var isDuplicated = laboratoryConsumptionDtos.GroupBy(x => new
            {
                ConsumptionDurationYear = x.ConsumptionDuration.Date.Year,
                ConsumptionDurationMonth = x.ConsumptionDuration.Date.Month,
                LaboratoryId = x.LaboratoryId,
                ProductId = x.ProductId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                laboratoryConsumptionDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = laboratoryConsumptionDtos.GroupBy(x => new
                {
                    ConsumptionDurationYear = x.ConsumptionDuration.Date.Year,
                    ConsumptionDurationMonth = x.ConsumptionDuration.Date.Month,
                    LaboratoryId = x.LaboratoryId,
                    ProductId = x.ProductId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate Consumption Duration name with the Product and Laboratory " });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate Consumption Duration name with the Product and Laboratory ";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryConsumptionDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.LaboratoryName = null;
                x.ProductName = null;
                return x;
            }).ToList();

            _response = await _laboratoryConsumptionService.ImportLaboratoryConsumptions(laboratoryConsumptionDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
