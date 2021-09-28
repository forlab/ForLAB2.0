
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryPatientStatistic;
using ForLab.Services.Laboratory.LaboratoryPatientStatistic;
using ForLab.Validators.LaboratoryPatientStatistic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryPatientStatisticsController : BaseController
    {
        private readonly ILaboratoryPatientStatisticService _laboratoryPatientStatisticService;

        public LaboratoryPatientStatisticsController(
            ILaboratoryPatientStatisticService laboratoryPatientStatisticService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryPatientStatisticService = laboratoryPatientStatisticService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryPatientStatisticFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryPatientStatisticService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryPatientStatisticFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryPatientStatisticService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryPatientStatisticDetails(int laboratoryPatientStatisticId)
        {
            _response = await _laboratoryPatientStatisticService.GetLaboratoryPatientStatisticDetails(laboratoryPatientStatisticId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryPatientStatistic([FromBody]LaboratoryPatientStatisticDto laboratoryPatientStatisticDto)
        {
            //Validation
            var validationResult = await (new LaboratoryPatientStatisticValidator(_laboratoryPatientStatisticService)).ValidateAsync(laboratoryPatientStatisticDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryPatientStatisticDto.Creator = null;
            laboratoryPatientStatisticDto.Updator = null;
            laboratoryPatientStatisticDto.CreatedBy = LoggedInUserId;
            laboratoryPatientStatisticDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryPatientStatisticDto.LaboratoryName = null;

            _response = await _laboratoryPatientStatisticService.CreateLaboratoryPatientStatistic(laboratoryPatientStatisticDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryPatientStatistic([FromBody]LaboratoryPatientStatisticDto laboratoryPatientStatisticDto)
        {
            // Validation
            var validationResult = await (new LaboratoryPatientStatisticValidator(_laboratoryPatientStatisticService)).ValidateAsync(laboratoryPatientStatisticDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryPatientStatisticDto.Creator = null;
            laboratoryPatientStatisticDto.Updator = null;
            laboratoryPatientStatisticDto.UpdatedBy = LoggedInUserId;
            laboratoryPatientStatisticDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryPatientStatisticDto.LaboratoryName = null;

            _response = await _laboratoryPatientStatisticService.UpdateLaboratoryPatientStatistic(laboratoryPatientStatisticDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryPatientStatisticId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryPatientStatisticService.IsUsed(laboratoryPatientStatisticId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryPatientStatisticService.UpdateIsActive(laboratoryPatientStatisticId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryPatientStatistic(int laboratoryPatientStatisticId)
        {
            // Validation
            var validationResult = await _laboratoryPatientStatisticService.IsUsed(laboratoryPatientStatisticId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryPatientStatisticService.RemoveLaboratoryPatientStatistic(laboratoryPatientStatisticId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        
        [HttpPost]
        public IActionResult ExportLaboratoryPatientStatistics(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryPatientStatisticFilterDto filterDto = null)
        {
            var file = _laboratoryPatientStatisticService.ExportLaboratoryPatientStatistics(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryPatientStatistics([FromBody]List<LaboratoryPatientStatisticDto> laboratoryPatientStatisticDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryPatientStatisticDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryPatientStatisticValidator()).ValidateAsync(laboratoryPatientStatisticDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (LaboratoryPatientStatisticName with CountryId)
            var isDuplicated = laboratoryPatientStatisticDtos.GroupBy(x => new
            {
                Period = x.Period.Date,
                LaboratoryId = x.LaboratoryId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                laboratoryPatientStatisticDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = laboratoryPatientStatisticDtos.GroupBy(x => new
                {
                    Period = x.Period.Date,
                    LaboratoryId = x.LaboratoryId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Period with the Laboratory Patient Statistic" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Period with the Laboratory Patient Statistic";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryPatientStatisticDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                //x.IsActive = IsSuperAdmin ? true : false;
                x.LaboratoryName = null;
                return x;
            }).ToList();

            _response = await _laboratoryPatientStatisticService.ImportLaboratoryPatientStatistics(laboratoryPatientStatisticDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
