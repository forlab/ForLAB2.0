using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Services.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Validators.PatientAssumptionParameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class PatientAssumptionParametersController : BaseController
    {
        private readonly IPatientAssumptionParameterService _patientAssumptionParameterService;

        public PatientAssumptionParametersController(
            IPatientAssumptionParameterService patientAssumptionParameterService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _patientAssumptionParameterService = patientAssumptionParameterService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] PatientAssumptionParameterFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _patientAssumptionParameterService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] PatientAssumptionParameterFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _patientAssumptionParameterService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllPatientAssumptionsForForcast(string programIds)
        {
            _response = _patientAssumptionParameterService.GetAllPatientAssumptionsForForcast(programIds);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetPatientAssumptionParameterDetails(int patientAssumptionParameterId)
        {
            _response = await _patientAssumptionParameterService.GetPatientAssumptionParameterDetails(patientAssumptionParameterId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreatePatientAssumptionParameter([FromBody]PatientAssumptionParameterDto patientAssumptionParameterDto)
        {
            //Validation
            var validationResult = await (new PatientAssumptionParameterValidator(_patientAssumptionParameterService)).ValidateAsync(patientAssumptionParameterDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            patientAssumptionParameterDto.Creator = null;
            patientAssumptionParameterDto.Updator = null;
            patientAssumptionParameterDto.CreatedBy = LoggedInUserId;
            patientAssumptionParameterDto.CreatedOn = DateTime.Now;
            //patientAssumptionParameterDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            patientAssumptionParameterDto.ProgramName = null;

            _response = await _patientAssumptionParameterService.CreatePatientAssumptionParameter(patientAssumptionParameterDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdatePatientAssumptionParameter([FromBody]PatientAssumptionParameterDto patientAssumptionParameterDto)
        {
            // Validation
            var validationResult = await (new PatientAssumptionParameterValidator(_patientAssumptionParameterService)).ValidateAsync(patientAssumptionParameterDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            patientAssumptionParameterDto.Creator = null;
            patientAssumptionParameterDto.Updator = null;
            patientAssumptionParameterDto.UpdatedBy = LoggedInUserId;
            patientAssumptionParameterDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            patientAssumptionParameterDto.ProgramName = null;

            _response = await _patientAssumptionParameterService.UpdatePatientAssumptionParameter(patientAssumptionParameterDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int patientAssumptionParameterId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _patientAssumptionParameterService.IsUsed(patientAssumptionParameterId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _patientAssumptionParameterService.UpdateIsActive(patientAssumptionParameterId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemovePatientAssumptionParameter(int patientAssumptionParameterId)
        {
            // Validation
            var validationResult = await _patientAssumptionParameterService.IsUsed(patientAssumptionParameterId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _patientAssumptionParameterService.RemovePatientAssumptionParameter(patientAssumptionParameterId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportPatientAssumptionParameters(int? pageIndex = null, int? pageSize = null, [FromQuery] PatientAssumptionParameterFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _patientAssumptionParameterService.ExportPatientAssumptionParameters(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportPatientAssumptionParameters([FromBody]List<PatientAssumptionParameterDto> patientAssumptionParameterDtos)
        {
            // Validation
            for (var i = 0; i < patientAssumptionParameterDtos.Count; i++)
            {
                var validationResult = await (new ImportPatientAssumptionParameterValidator()).ValidateAsync(patientAssumptionParameterDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (PatientAssumptionParameterName with CountryId)
            var isDuplicated = patientAssumptionParameterDtos.GroupBy(x => new
            {
                Name = x.Name.Trim().ToLower(),
                ProgramId = x.ProgramId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                patientAssumptionParameterDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = patientAssumptionParameterDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    ProgramId = x.ProgramId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Patient Assumption Parameter name with the Program" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Patient Assumption Parameter name with the Program";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            patientAssumptionParameterDtos.Select(x =>
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

            _response = await _patientAssumptionParameterService.ImportPatientAssumptionParameters(patientAssumptionParameterDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
