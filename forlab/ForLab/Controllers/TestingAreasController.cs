using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.TestingArea;
using ForLab.Services.Lookup.TestingArea;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
  public class TestingAreasController : BaseController
    {
        private readonly ITestingAreaService _testingAreaService;

        public TestingAreasController(
            ITestingAreaService testingAreaService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _testingAreaService = testingAreaService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] TestingAreaFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _testingAreaService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] TestingAreaFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _testingAreaService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetTestingAreaDetails(int testingAreaId)
        {
            _response = await _testingAreaService.GetTestingAreaDetails(testingAreaId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateTestingArea([FromBody]TestingAreaDto testingAreaDto)
        {
            //Validation
            var validationResult = await (new TestingAreaValidator(_testingAreaService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(testingAreaDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            testingAreaDto.Creator = null;
            testingAreaDto.Updator = null;
            testingAreaDto.CreatedBy = LoggedInUserId;
            testingAreaDto.CreatedOn = DateTime.Now;
            testingAreaDto.IsActive = IsSuperAdmin ? true : false;
            testingAreaDto.Shared = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors

            _response = await _testingAreaService.CreateTestingArea(testingAreaDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateTestingArea([FromBody]TestingAreaDto testingAreaDto)
        {
            // Validation
            var validationResult = await (new TestingAreaValidator(_testingAreaService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(testingAreaDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            testingAreaDto.Creator = null;
            testingAreaDto.Updator = null;
            testingAreaDto.UpdatedBy = LoggedInUserId;
            testingAreaDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors

            _response = await _testingAreaService.UpdateTestingArea(testingAreaDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int testingAreaId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _testingAreaService.IsUsed(testingAreaId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _testingAreaService.UpdateIsActive(testingAreaId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _testingAreaService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _testingAreaService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveTestingArea(int testingAreaId)
        {
            // Validation
            var validationResult = await _testingAreaService.IsUsed(testingAreaId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _testingAreaService.RemoveTestingArea(testingAreaId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportTestingAreas([FromBody]List<TestingAreaDto> testingAreaDtos)
        {
            // Validation
            for (var i = 0; i < testingAreaDtos.Count; i++)
            {
                var validationResult = await (new ImportTestingAreaValidator()).ValidateAsync(testingAreaDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = testingAreaDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                testingAreaDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = testingAreaDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            testingAreaDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.IsActive = IsSuperAdmin ? true : false;
                x.Shared = IsSuperAdmin ? true : false;
                return x;
            }).ToList();

            _response = await _testingAreaService.ImportTestingAreas(testingAreaDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        
        [HttpPost]
        public IActionResult ExportTestingAreas(int? pageIndex = null, int? pageSize = null, [FromQuery] TestingAreaFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _testingAreaService.ExportTestingAreas(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}