using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.LaboratoryLevel;
using ForLab.Services.Lookup.LaboratoryLevel;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryLevelsController : BaseController
    {
        private readonly ILaboratoryLevelService _laboratoryLevelService;

        public LaboratoryLevelsController(
            ILaboratoryLevelService laboratoryLevelService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryLevelService = laboratoryLevelService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryLevelFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryLevelService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryLevelFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryLevelService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryLevelDetails(int laboratoryLevelId)
        {
            _response = await _laboratoryLevelService.GetLaboratoryLevelDetails(laboratoryLevelId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreatelaboratoryLevel([FromBody]LaboratoryLevelDto laboratoryLevelDto)
        {
            //Validation
            var validationResult = await (new LaboratoryLevelValidator(_laboratoryLevelService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(laboratoryLevelDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryLevelDto.Creator = null;
            laboratoryLevelDto.Updator = null;
            laboratoryLevelDto.CreatedBy = LoggedInUserId;
            laboratoryLevelDto.CreatedOn = DateTime.Now;
            laboratoryLevelDto.IsActive = IsSuperAdmin ? true : false;
            laboratoryLevelDto.Shared = IsSuperAdmin ? true : false;

            _response = await _laboratoryLevelService.CreateLaboratoryLevel(laboratoryLevelDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryLevel([FromBody]LaboratoryLevelDto laboratoryLevelDto)
        {
            // Validation
            var validationResult = await (new LaboratoryLevelValidator(_laboratoryLevelService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(laboratoryLevelDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryLevelDto.Creator = null;
            laboratoryLevelDto.Updator = null;
            laboratoryLevelDto.UpdatedBy = LoggedInUserId;
            laboratoryLevelDto.UpdatedOn = DateTime.Now;

            _response = await _laboratoryLevelService.UpdateLaboratoryLevel(laboratoryLevelDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryLevelId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryLevelService.IsUsed(laboratoryLevelId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryLevelService.UpdateIsActive(laboratoryLevelId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _laboratoryLevelService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _laboratoryLevelService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryLevel(int laboratoryLevelId)
        {
            // Validation
            var validationResult = await _laboratoryLevelService.IsUsed(laboratoryLevelId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryLevelService.RemoveLaboratoryLevel(laboratoryLevelId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryLevels([FromBody]List<LaboratoryLevelDto> laboratoryLevelDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryLevelDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryLevelValidator()).ValidateAsync(laboratoryLevelDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = laboratoryLevelDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                laboratoryLevelDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = laboratoryLevelDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }
   
            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryLevelDtos.Select(x =>
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

            _response = await _laboratoryLevelService.ImportLaboratoryLevels(laboratoryLevelDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        
        [HttpPost]
        public IActionResult ExportLaboratoryLevels(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryLevelFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _laboratoryLevelService.ExportLaboratoryLevels(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}