using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Testing.Test;
using ForLab.Services.Testing.Test;
using ForLab.Validators.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class TestsController : BaseController
    {
        private readonly ITestService _testService;

        public TestsController(
            ITestService testService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _testService = testService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] TestFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _testService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] TestFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _testService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetTestDetails(int testId)
        {
            _response = await _testService.GetTestDetails(testId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateTest([FromBody]TestDto testDto)
        {
            //Validation
            var validationResult = await (new TestValidator(_testService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(testDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            testDto.Creator = null;
            testDto.Updator = null;
            testDto.CreatedBy = LoggedInUserId;
            testDto.CreatedOn = DateTime.Now;
            testDto.Shared = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            testDto.TestingAreaName = null;

            _response = await _testService.CreateTest(testDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateTest([FromBody]TestDto testDto)
        {
            // Validation
            var validationResult = await (new TestValidator(_testService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(testDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            testDto.Creator = null;
            testDto.Updator = null;
            testDto.UpdatedBy = LoggedInUserId;
            testDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            testDto.TestingAreaName = null;

            _response = await _testService.UpdateTest(testDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int testId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _testService.IsUsed(testId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _testService.UpdateIsActive(testId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _testService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _testService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveTest(int testId)
        {
            // Validation
            var validationResult = await _testService.IsUsed(testId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _testService.RemoveTest(testId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportTests(int? pageIndex = null, int? pageSize = null, [FromQuery] TestFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _testService.ExportTests(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportTests([FromBody]List<TestDto> testDtos)
        {
            // Validation
            for (var i = 0; i < testDtos.Count; i++)
            {
                var validationResult = await (new ImportTestValidator()).ValidateAsync(testDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (TestName with CountryId)
            var isDuplicated = testDtos.GroupBy(x => new
            {
                Name = x.Name.Trim().ToLower(),
                TestingAreaId = x.TestingAreaId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                testDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = testDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    TestingAreaId = x.TestingAreaId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the test name with the Testing Area" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the test name with the Testing Area";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            testDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.TestingAreaName = null;
                return x;
            }).ToList();

            _response = await _testService.ImportTests(testDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
