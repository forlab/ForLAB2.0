using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.Laboratory;
using ForLab.Services.Lookup.Laboratory;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoriesController : BaseController
    {
        private readonly ILaboratoryService _laboratoryService;

        public LaboratoriesController(
            ILaboratoryService laboratoryService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryService = laboratoryService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryDetails(int laboratoryId)
        {
            _response = await _laboratoryService.GetLaboratoryDetails(laboratoryId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratory([FromBody]LaboratoryDto laboratoryDto)
        {
            //Validation
            var validationResult = await (new LaboratoryValidator(_laboratoryService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(laboratoryDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryDto.Creator = null;
            laboratoryDto.Updator = null;
            laboratoryDto.CreatedBy = LoggedInUserId;
            laboratoryDto.CreatedOn = DateTime.Now;
            laboratoryDto.Shared = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryDto.LaboratoryCategoryName = null;
            laboratoryDto.LaboratoryLevelName = null;
            laboratoryDto.RegionName = null;

            _response = await _laboratoryService.CreateLaboratory(laboratoryDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratory([FromBody]LaboratoryDto laboratoryDto)
        {
            // Validation
            var validationResult = await (new LaboratoryValidator(_laboratoryService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(laboratoryDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryDto.Creator = null;
            laboratoryDto.Updator = null;
            laboratoryDto.UpdatedBy = LoggedInUserId;
            laboratoryDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryDto.LaboratoryCategoryName = null;
            laboratoryDto.LaboratoryLevelName = null;
            laboratoryDto.RegionName = null;

            _response = await _laboratoryService.UpdateLaboratory(laboratoryDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryService.IsUsed(laboratoryId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryService.UpdateIsActive(laboratoryId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _laboratoryService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _laboratoryService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratory(int laboratoryId)
        {
            // Validation
            var validationResult = await _laboratoryService.IsUsed(laboratoryId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryService.RemoveLaboratory(laboratoryId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratories([FromBody]List<LaboratoryDto> laboratoryDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryValidator()).ValidateAsync(laboratoryDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (LaboratoryName with ProgramId)
            var isDuplicated = laboratoryDtos.GroupBy(x => new
            {
                Name = x.Name.Trim().ToLower(),
                RegionId = x.RegionId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                laboratoryDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = laboratoryDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    RegionId = x.RegionId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the city name with the Region" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the city name with the Region";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                //x.IsActive = IsSuperAdmin ? true : false;
                x.LaboratoryCategoryName = null;
                x.LaboratoryLevelName = null;
                x.RegionName = null;
                return x;
            }).ToList();

            _response = await _laboratoryService.ImportLaboratories(laboratoryDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        
        [HttpPost]
        public IActionResult ExportLaboratories(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryFilterDto filterDto = null)
        {
            var file = _laboratoryService.ExportLaboratories(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}