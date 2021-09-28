using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.Program;
using ForLab.Services.DiseaseProgram.Program;
using ForLab.Validators.Program;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ProgramsController : BaseController
    {
        private readonly IProgramService _programService;

        public ProgramsController(
            IProgramService programService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _programService = programService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ProgramFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _programService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ProgramFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _programService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetProgramDetails(int programId)
        {
            _response = await _programService.GetProgramDetails(programId);
            return _response;
        }

        [HttpGet]
        public async Task<IResponseDTO> GetProgramDetailsForForcast(int programId)
        {
            _response = await _programService.GetProgramDetailsForForcast(programId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateProgram([FromBody]ProgramDto programDto)
        {
            //Validation
            var validationResult = await (new ProgramValidator(_programService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(programDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }
            
            // Set variables by the system
            programDto.Creator = null;
            programDto.Updator = null;
            programDto.CreatedBy = LoggedInUserId;
            programDto.CreatedOn = DateTime.Now;
            //programDto.IsActive = IsSuperAdmin ? true : false;
            programDto.ProgramTestDtos?.Select(x => 
            { 
                x.Creator = null; 
                x.TestName = null; 
                x.TestingProtocolPatientGroupId = null; 
                x.TestingProtocolPatientGroupName = null;
                x.TestingProtocolCalculationPeriodName = null;
                x.TestingProtocolBaseLine = null;
                x.TestingProtocolTestAfterFirstYear = null;
                x.TestingProtocolPatientGroupName = null;
                x.ProgramName = null; 
                x.CreatedBy = LoggedInUserId; 
                x.CreatedOn = DateTime.Now;
                x.TestingProtocolDto.CreatedBy = LoggedInUserId;
                x.TestingProtocolDto.CreatedOn = DateTime.Now;
                x.TestingProtocolDto.TestingProtocolCalculationPeriodMonthDtos.Select(y => { y.CreatedBy = LoggedInUserId; y.CreatedOn = DateTime.Now; return y; }).ToList();
                return x; 
            }).ToList();
            programDto.TestingAssumptionParameterDtos?.Select(x => { x.Creator = null; x.ProgramName = null; x.CreatedBy = LoggedInUserId; x.CreatedOn = DateTime.Now; return x; }).ToList();
            programDto.ProductAssumptionParameterDtos?.Select(x => { x.Creator = null; x.ProgramName = null; x.CreatedBy = LoggedInUserId; x.CreatedOn = DateTime.Now; return x; }).ToList();
            programDto.PatientAssumptionParameterDtos?.Select(x => { x.Creator = null; x.ProgramName = null; x.CreatedBy = LoggedInUserId; x.CreatedOn = DateTime.Now; return x; }).ToList();
            // Set relation variables with null to avoid unexpected EF errors
            programDto.DiseaseName = null;

            _response = await _programService.CreateProgram(programDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateProgram([FromBody]ProgramDto programDto)
        {
            // Validation
            var validationResult = await (new ProgramValidator(_programService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(programDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            programDto.Creator = null;
            programDto.Updator = null;
            programDto.UpdatedBy = LoggedInUserId;
            programDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            programDto.DiseaseName = null;

            _response = await _programService.UpdateProgram(programDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int programId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _programService.IsUsed(programId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _programService.UpdateIsActive(programId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveProgram(int programId)
        {
            // Validation
            var validationResult = await _programService.IsUsed(programId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _programService.RemoveProgram(programId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportPrograms(int? pageIndex = null, int? pageSize = null, [FromQuery] ProgramFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _programService.ExportPrograms(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportPrograms([FromBody]List<ProgramDto> programDtos)
        {
            // Validation
            for (var i = 0; i < programDtos.Count; i++)
            {
                var validationResult = await (new ImportProgramValidator()).ValidateAsync(programDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (ProgramName with CountryId)
            var isDuplicated = programDtos.GroupBy(x => new
            {
                Name = x.Name.Trim().ToLower(),
                DiseaseId = x.DiseaseId
            }).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                programDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = programDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    DiseaseId = x.DiseaseId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the program name with the Disease" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the program name with the Disease";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            programDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                //x.IsActive = IsSuperAdmin ? true : false;
                x.ProgramTestDtos = null;
                x.TestingAssumptionParameterDtos = null;
                x.ProductAssumptionParameterDtos = null;
                x.PatientAssumptionParameterDtos = null;
                return x;
            }).ToList();

            _response = await _programService.ImportPrograms(programDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


    }
}