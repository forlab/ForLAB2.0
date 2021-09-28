using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryTestService;
using ForLab.Services.Laboratory.LaboratoryTestService;
using ForLab.Validators.LaboratoryTestService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryTestServicesController : BaseController
    {
        private readonly ILaboratoryTestService _laboratoryTestService;

        public LaboratoryTestServicesController(
            ILaboratoryTestService laboratoryTestService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryTestService = laboratoryTestService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryTestServiceFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryTestService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryTestServiceFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryTestService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryTestServiceDetails(int laboratoryTestServiceId)
        {
            _response = await _laboratoryTestService.GetLaboratoryTestServiceDetails(laboratoryTestServiceId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryTestService([FromBody]LaboratoryTestServiceDto laboratoryTestServiceDto)
        {
            //Validation
            var validationResult = await (new LaboratoryTestServiceValidator(_laboratoryTestService)).ValidateAsync(laboratoryTestServiceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryTestServiceDto.Creator = null;
            laboratoryTestServiceDto.Updator = null;
            laboratoryTestServiceDto.CreatedBy = LoggedInUserId;
            laboratoryTestServiceDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryTestServiceDto.LaboratoryName = null;
            laboratoryTestServiceDto.TestName = null;

            _response = await _laboratoryTestService.CreateLaboratoryTestService(laboratoryTestServiceDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryTestService([FromBody]LaboratoryTestServiceDto laboratoryTestServiceDto)
        {
            // Validation
            var validationResult = await (new LaboratoryTestServiceValidator(_laboratoryTestService)).ValidateAsync(laboratoryTestServiceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryTestServiceDto.Creator = null;
            laboratoryTestServiceDto.Updator = null;
            laboratoryTestServiceDto.UpdatedBy = LoggedInUserId;
            laboratoryTestServiceDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryTestServiceDto.LaboratoryName = null;
            laboratoryTestServiceDto.TestName = null;

            _response = await _laboratoryTestService.UpdateLaboratoryTestService(laboratoryTestServiceDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryTestServiceId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryTestService.IsUsed(laboratoryTestServiceId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryTestService.UpdateIsActive(laboratoryTestServiceId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryTestService(int laboratoryTestServiceId)
        {
            // Validation
            var validationResult = await _laboratoryTestService.IsUsed(laboratoryTestServiceId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryTestService.RemoveLaboratoryTestService(laboratoryTestServiceId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportLaboratoryTestServices(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryTestServiceFilterDto filterDto = null)
        {
            var file = _laboratoryTestService.ExportLaboratoryTestServices(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryTestServices([FromBody]List<LaboratoryTestServiceDto> laboratoryTestServiceDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryTestServiceDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryTestServiceValidator()).ValidateAsync(laboratoryTestServiceDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (LaboratoryTestServiceName with CountryId)
            var isDuplicated = laboratoryTestServiceDtos.GroupBy(x => new
            {
                ServiceDurationYear = x.ServiceDuration.Date.Year,
                ServiceDurationMonth = x.ServiceDuration.Date.Month,
                LaboratoryId = x.LaboratoryId,
                TestId = x.TestId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                laboratoryTestServiceDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = laboratoryTestServiceDtos.GroupBy(x => new
                {
                    ServiceDurationYear = x.ServiceDuration.Date.Year,
                    ServiceDurationMonth = x.ServiceDuration.Date.Month,
                    LaboratoryId = x.LaboratoryId,
                    TestId = x.TestId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Service Duration with the Laboratory and Test" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Service Duration with the Laboratory and Test";
                return _response;
            }
     
            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryTestServiceDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.LaboratoryName = null;
                x.TestName = null;
                return x;
            }).ToList();

            _response = await _laboratoryTestService.ImportLaboratoryTestServices(laboratoryTestServiceDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
    }
}
