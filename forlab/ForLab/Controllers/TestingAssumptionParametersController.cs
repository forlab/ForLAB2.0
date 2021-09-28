using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Services.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Validators.TestingAssumptionParameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class TestingAssumptionParametersController : BaseController
    {
        private readonly ITestingAssumptionParameterService _testingAssumptionParameterService;

        public TestingAssumptionParametersController(
            ITestingAssumptionParameterService testingAssumptionParameterService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _testingAssumptionParameterService = testingAssumptionParameterService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] TestingAssumptionParameterFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _testingAssumptionParameterService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] TestingAssumptionParameterFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _testingAssumptionParameterService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllTestingAssumptionsForForcast(string programIds)
        {
            _response = _testingAssumptionParameterService.GetAllTestingAssumptionsForForcast(programIds);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetTestingAssumptionParameterDetails(int testingAssumptionParameterId)
        {
            _response = await _testingAssumptionParameterService.GetTestingAssumptionParameterDetails(testingAssumptionParameterId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateTestingAssumptionParameter([FromBody]TestingAssumptionParameterDto testingAssumptionParameterDto)
        {
            //Validation
            var validationResult = await (new TestingAssumptionParameterValidator(_testingAssumptionParameterService)).ValidateAsync(testingAssumptionParameterDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            testingAssumptionParameterDto.Creator = null;
            testingAssumptionParameterDto.Updator = null;
            testingAssumptionParameterDto.CreatedBy = LoggedInUserId;
            testingAssumptionParameterDto.CreatedOn = DateTime.Now;
            //testingAssumptionParameterDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            testingAssumptionParameterDto.ProgramName = null;

            _response = await _testingAssumptionParameterService.CreateTestingAssumptionParameter(testingAssumptionParameterDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateTestingAssumptionParameter([FromBody]TestingAssumptionParameterDto testingAssumptionParameterDto)
        {
            // Validation
            var validationResult = await (new TestingAssumptionParameterValidator(_testingAssumptionParameterService)).ValidateAsync(testingAssumptionParameterDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            testingAssumptionParameterDto.Creator = null;
            testingAssumptionParameterDto.Updator = null;
            testingAssumptionParameterDto.UpdatedBy = LoggedInUserId;
            testingAssumptionParameterDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            testingAssumptionParameterDto.ProgramName = null;

            _response = await _testingAssumptionParameterService.UpdateTestingAssumptionParameter(testingAssumptionParameterDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int testingAssumptionParameterId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _testingAssumptionParameterService.IsUsed(testingAssumptionParameterId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _testingAssumptionParameterService.UpdateIsActive(testingAssumptionParameterId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveTestingAssumptionParameter(int testingAssumptionParameterId)
        {
            // Validation
            var validationResult = await _testingAssumptionParameterService.IsUsed(testingAssumptionParameterId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _testingAssumptionParameterService.RemoveTestingAssumptionParameter(testingAssumptionParameterId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportTestingAssumptionParameters(int? pageIndex = null, int? pageSize = null, [FromQuery] TestingAssumptionParameterFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _testingAssumptionParameterService.ExportTestingAssumptionParameters(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportTestingAssumptionParameters([FromBody]List<TestingAssumptionParameterDto> testingAssumptionParameterDtos)
        {
            // Validation
            for (var i = 0; i < testingAssumptionParameterDtos.Count; i++)
            {
                var validationResult = await (new ImportTestingAssumptionParameterValidator()).ValidateAsync(testingAssumptionParameterDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (TestingAssumptionParameterName with ProgramId)
            var isDuplicated = testingAssumptionParameterDtos.GroupBy(x => new
            {
                Name = x.Name.Trim().ToLower(),
                ProgramId = x.ProgramId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                testingAssumptionParameterDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = testingAssumptionParameterDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    ProgramId = x.ProgramId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Testing Assumption Parameter name with the Program" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Testing Assumption Parameter name with the Program";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            testingAssumptionParameterDtos.Select(x =>
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

            _response = await _testingAssumptionParameterService.ImportTestingAssumptionParameters(testingAssumptionParameterDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
