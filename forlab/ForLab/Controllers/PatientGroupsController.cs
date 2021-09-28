using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.PatientGroup;
using ForLab.Services.Lookup.PatientGroup;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class PatientGroupsController : BaseController
    {
        private readonly IPatientGroupService _patientGroupService;

        public PatientGroupsController(
            IPatientGroupService patientGroupService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _patientGroupService = patientGroupService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery]  PatientGroupFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _patientGroupService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] PatientGroupFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _patientGroupService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetPatientGroupDetails(int patientGroupId)
        {
            _response = await _patientGroupService.GetPatientGroupDetails(patientGroupId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreatePatientGroup([FromBody]PatientGroupDto patientGroupDto)
        {
            //Validation
            var validationResult = await (new PatientGroupValidator(_patientGroupService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(patientGroupDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            patientGroupDto.Creator = null;
            patientGroupDto.Updator = null;
            patientGroupDto.CreatedBy = LoggedInUserId;
            patientGroupDto.CreatedOn = DateTime.Now;
            patientGroupDto.IsActive = IsSuperAdmin ? true : false;
            patientGroupDto.Shared = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors

            _response = await _patientGroupService.CreatePatientGroup(patientGroupDto);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdatePatientGroup([FromBody]PatientGroupDto patientGroupDto)
        {
            // Validation
            var validationResult = await (new PatientGroupValidator(_patientGroupService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(patientGroupDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            patientGroupDto.Creator = null;
            patientGroupDto.Updator = null;
            patientGroupDto.UpdatedBy = LoggedInUserId;
            patientGroupDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors

            _response = await _patientGroupService.UpdatePatientGroup(patientGroupDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int patientGroupId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _patientGroupService.IsUsed(patientGroupId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _patientGroupService.UpdateIsActive(patientGroupId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _patientGroupService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _patientGroupService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemovePatientGroup(int patientGroupId)
        {
            // Validation
            var validationResult = await _patientGroupService.IsUsed(patientGroupId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _patientGroupService.RemovePatientGroup(patientGroupId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportPatientGroups([FromBody]List<PatientGroupDto> patientGroupDtos)
        {
            // Validation
            for(var i=0; i< patientGroupDtos.Count; i++)
            {
                var validationResult = await (new ImportPatientGroupValidator()).ValidateAsync(patientGroupDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = patientGroupDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                patientGroupDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = patientGroupDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            patientGroupDtos.Select(x =>
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

            _response = await _patientGroupService.ImportPatientGroups(patientGroupDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportPatientGroups(int? pageIndex = null, int? pageSize = null, [FromQuery] PatientGroupFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _patientGroupService.ExportPatientGroups(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}