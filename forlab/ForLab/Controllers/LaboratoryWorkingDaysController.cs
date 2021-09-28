using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryWorkingDay;
using ForLab.Services.Laboratory.LaboratoryWorkingDay;
using ForLab.Validators.LaboratoryWorkingDay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ForLab.API.Helpers;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryWorkingDaysController : BaseController
    {
        private readonly ILaboratoryWorkingDayService _laboratoryWorkingDayService;

        public LaboratoryWorkingDaysController(
            ILaboratoryWorkingDayService laboratoryWorkingDayService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryWorkingDayService = laboratoryWorkingDayService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryWorkingDayFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryWorkingDayService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryWorkingDayFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryWorkingDayService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryWorkingDayDetails(int laboratoryWorkingDayId)
        {
            _response = await _laboratoryWorkingDayService.GetLaboratoryWorkingDayDetails(laboratoryWorkingDayId);
            return _response;
        }


        // We send body as FormData to make C# able to read time from string to TimeSpan
        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryWorkingDay([ModelBinder(BinderType = typeof(JsonModelBinder))] LaboratoryWorkingDayDto laboratoryWorkingDayDto)
        {
            //Validation
            var validationResult = await (new LaboratoryWorkingDayValidator(_laboratoryWorkingDayService)).ValidateAsync(laboratoryWorkingDayDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryWorkingDayDto.Creator = null;
            laboratoryWorkingDayDto.Updator = null;
            laboratoryWorkingDayDto.CreatedBy = LoggedInUserId;
            laboratoryWorkingDayDto.CreatedOn = DateTime.Now;
            //laboratoryWorkingDayDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryWorkingDayDto.LaboratoryName = null;

            _response = await _laboratoryWorkingDayService.CreateLaboratoryWorkingDay(laboratoryWorkingDayDto);
            return _response;
        }


        // We send body as FormData to make C# able to read time from string to TimeSpan
        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryWorkingDay([ModelBinder(BinderType = typeof(JsonModelBinder))] LaboratoryWorkingDayDto laboratoryWorkingDayDto)
        {
            // Validation
            var validationResult = await (new LaboratoryWorkingDayValidator(_laboratoryWorkingDayService)).ValidateAsync(laboratoryWorkingDayDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryWorkingDayDto.Creator = null;
            laboratoryWorkingDayDto.Updator = null;
            laboratoryWorkingDayDto.UpdatedBy = LoggedInUserId;
            laboratoryWorkingDayDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryWorkingDayDto.LaboratoryName = null;

            _response = await _laboratoryWorkingDayService.UpdateLaboratoryWorkingDay(laboratoryWorkingDayDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryWorkingDayId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryWorkingDayService.IsUsed(laboratoryWorkingDayId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryWorkingDayService.UpdateIsActive(laboratoryWorkingDayId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryWorkingDay(int laboratoryWorkingDayId)
        {
            // Validation
            var validationResult = await _laboratoryWorkingDayService.IsUsed(laboratoryWorkingDayId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryWorkingDayService.RemoveLaboratoryWorkingDay(laboratoryWorkingDayId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        [HttpPost]
        public IActionResult ExportLaboratoryWorkingDays(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryWorkingDayFilterDto filterDto = null)
        {
            var file = _laboratoryWorkingDayService.ExportLaboratoryWorkingDays(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}
