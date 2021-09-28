using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.Services.DiseaseProgram.ProgramTest;
using ForLab.Validators.DiseaseProgram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ProgramTestsController : BaseController
    {
        private readonly IProgramTestService _programTestService;

        public ProgramTestsController(
            IProgramTestService programTestService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _programTestService = programTestService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ProgramTestFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _programTestService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ProgramTestFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _programTestService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetProgramTestDetails(int programTestId)
        {
            _response = await _programTestService.GetProgramTestDetails(programTestId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateProgramTest([FromBody]ProgramTestDto programTestDto)
        {
            //Validation
            var validationResult = await (new ProgramTestValidator(_programTestService)).ValidateAsync(programTestDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            programTestDto.Creator = null;
            programTestDto.Updator = null;
            programTestDto.CreatedBy = LoggedInUserId;
            programTestDto.CreatedOn = DateTime.Now;
            programTestDto.TestingProtocolPatientGroupId = null;
            programTestDto.TestingProtocolPatientGroupName = null;
            programTestDto.TestingProtocolCalculationPeriodName = null;
            programTestDto.TestingProtocolBaseLine = null;
            programTestDto.TestingProtocolTestAfterFirstYear = null;
            programTestDto.TestingProtocolPatientGroupName = null;
            programTestDto.ProgramName = null;
            programTestDto.TestingProtocolDto.CreatedBy = LoggedInUserId;
            programTestDto.TestingProtocolDto.CreatedOn = DateTime.Now;
            programTestDto.TestingProtocolDto.TestingProtocolCalculationPeriodMonthDtos.Select(y => { y.CreatedBy = LoggedInUserId; y.CreatedOn = DateTime.Now; return y; }).ToList();
            // Set relation variables with null to avoid unexpected EF errors
            programTestDto.ProgramName = null;
            programTestDto.TestName = null;

            _response = await _programTestService.CreateProgramTest(programTestDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateProgramTest([FromBody]ProgramTestDto programTestDto)
        {
            // Validation
            var validationResult = await (new ProgramTestValidator(_programTestService)).ValidateAsync(programTestDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            programTestDto.Creator = null;
            programTestDto.Updator = null;
            programTestDto.UpdatedBy = LoggedInUserId;
            programTestDto.UpdatedOn = DateTime.Now;
            programTestDto.TestingProtocolPatientGroupId = null;
            programTestDto.TestingProtocolName = null;
            programTestDto.TestingProtocolPatientGroupName = null;
            programTestDto.TestingProtocolCalculationPeriodName = null;
            programTestDto.TestingProtocolBaseLine = null;
            programTestDto.TestingProtocolTestAfterFirstYear = null;
            programTestDto.TestingProtocolPatientGroupName = null;
            programTestDto.ProgramName = null;
            programTestDto.TestingProtocolDto.Creator = null;
            programTestDto.TestingProtocolDto.Updator = null;
            programTestDto.TestingProtocolDto.CalculationPeriodName = null;
            programTestDto.TestingProtocolDto.PatientGroupName = null;
            programTestDto.TestingProtocolDto.TestName = null;
            programTestDto.TestingProtocolDto.UpdatedBy = LoggedInUserId;
            programTestDto.TestingProtocolDto.UpdatedOn = DateTime.Now;
            programTestDto.TestingProtocolDto.TestingProtocolCalculationPeriodMonthDtos.Select(y => { y.Creator = null; y.Updator = null; y.CreatedBy = LoggedInUserId; y.CreatedOn = DateTime.Now; return y; }).ToList();
            // Set relation variables with null to avoid unexpected EF errors
            programTestDto.ProgramName = null;
            programTestDto.TestName = null;

            _response = await _programTestService.UpdateProgramTest(programTestDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int programTestId, bool isActive)
        {
            //if (!isActive)
            //{
            //    // Validation
            //    var validationResult = await _programTestService.IsUsed(programTestId);
            //    if (!validationResult.IsPassed)
            //    {
            //        return validationResult;
            //    }
            //}

            _response = await _programTestService.UpdateIsActive(programTestId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveProgramTest(int programTestId)
        {
            //// Validation
            //var validationResult = await _programTestService.IsUsed(programTestId);
            //if (!validationResult.IsPassed)
            //{
            //    return validationResult;
            //}

            _response = await _programTestService.RemoveProgramTest(programTestId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportProgramTests(int? pageIndex = null, int? pageSize = null, [FromQuery] ProgramTestFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _programTestService.ExportProgramTests(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportProgramTests([FromBody]List<ProgramTestDto> programTestDtos)
        {
            // Validation
            for (var i = 0; i < programTestDtos.Count; i++)
            {
                var validationResult = await (new ImportProgramTestValidator()).ValidateAsync(programTestDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (ProgramTestName with CountryId)
            var duplicates = programTestDtos.GroupBy(x => new
            {
                TestingProtocolId = x.TestingProtocolId,
                TestId = x.TestId,
                TestingProtocolPatientGroupId = x.TestingProtocolPatientGroupId,
                ProgramId = x.ProgramId
            }).Any(g => g.Count() > 1);
            if (duplicates)
            {
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Testing Protocol with Patient Group per Program";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            programTestDtos.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.ProgramName = null;
                x.TestName = null;
                return x;
            }).ToList();

            _response = await _programTestService.ImportProgramTests(programTestDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
    }
}
