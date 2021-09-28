using ForLab.Core.Interfaces;
using ForLab.Data.Enums;
using ForLab.DTO.Forecasting.ForecastInfo;
using ForLab.Services.Forecasting.ForecastInfo;
using ForLab.Validators.Forecasting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastInfosController : BaseController
    {
        private readonly IForecastInfoService _forecastInfoService;

        public ForecastInfosController(
            IForecastInfoService forecastInfoService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastInfoService = forecastInfoService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastInfoFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _forecastInfoService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ForecastInfoFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _forecastInfoService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetForecastInfoDetails(int forecastInfoId)
        {
            _response = await _forecastInfoService.GetForecastInfoDetails(forecastInfoId);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetForecastInfoDetailsForUpdate(int forecastInfoId)
        {
            _response = await _forecastInfoService.GetForecastInfoDetailsForUpdate(forecastInfoId, LoggedInUserId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateForecastInfo([FromBody] ForecastInfoDto forecastInfoDto)
        {
            //Validation
            var validationResult = await (new ForecastInfoValidator(_forecastInfoService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(forecastInfoDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            forecastInfoDto.CreatedBy = LoggedInUserId;
            forecastInfoDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            forecastInfoDto.Creator = null;
            forecastInfoDto.Updator = null;

            _response = await _forecastInfoService.CreateForecastInfo(forecastInfoDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateForecastInfo([FromBody] ForecastInfoDto forecastInfoDto)
        {
            //Validation
            var validationResult = await (new ForecastInfoValidator(_forecastInfoService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(forecastInfoDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            forecastInfoDto.UpdatedBy = LoggedInUserId;
            forecastInfoDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            forecastInfoDto.Creator = null;
            forecastInfoDto.Updator = null;

            _response = await _forecastInfoService.UpdateForecastInfo(forecastInfoDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int forecastInfoId, bool isActive)
        {
            _response = await _forecastInfoService.UpdateIsActive(forecastInfoId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastInfos(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastInfoFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _forecastInfoService.ExportForecastInfo(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
