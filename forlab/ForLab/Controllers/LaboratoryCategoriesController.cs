using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.API.Controllers;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.LaboratoryCategory;
using ForLab.Services.Lookup.LaboratoryCategory;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.LaboratoryCategory
{
    [Authorize]
    public class LaboratoryCategoriesController : BaseController
    {
        private readonly ILaboratoryCategoryService _laboratoryCategoryService;

        public LaboratoryCategoriesController(
            ILaboratoryCategoryService laboratoryCategoryService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryCategoryService = laboratoryCategoryService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery]  LaboratoryCategoryFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryCategoryService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryCategoryFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryCategoryService.GetAllAsDrp(filterDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryCategoryDetails(int laboratoryCategoryId)
        {
            _response = await _laboratoryCategoryService.GetLaboratoryCategoryDetails(laboratoryCategoryId);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryCategory([FromBody]LaboratoryCategoryDto laboratoryCategoryDto)
        {
            //Validation
            var validationResult = await (new LaboratoryCategoryValidator(_laboratoryCategoryService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(laboratoryCategoryDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryCategoryDto.Creator = null;
            laboratoryCategoryDto.Updator = null;
            laboratoryCategoryDto.CreatedBy = LoggedInUserId;
            laboratoryCategoryDto.CreatedOn = DateTime.Now;
            laboratoryCategoryDto.IsActive = IsSuperAdmin ? true : false;
            laboratoryCategoryDto.Shared = IsSuperAdmin ? true : false;

            _response = await _laboratoryCategoryService.CreateLaboratoryCategory(laboratoryCategoryDto);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryCategory([FromBody]LaboratoryCategoryDto laboratoryCategoryDto)
        {
            // Validation
            var validationResult = await (new LaboratoryCategoryValidator(_laboratoryCategoryService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(laboratoryCategoryDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryCategoryDto.Creator = null;
            laboratoryCategoryDto.Updator = null;
            laboratoryCategoryDto.UpdatedBy = LoggedInUserId;
            laboratoryCategoryDto.UpdatedOn = DateTime.Now;

            _response = await _laboratoryCategoryService.UpdateLaboratoryCategory(laboratoryCategoryDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryCategoryId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryCategoryService.IsUsed(laboratoryCategoryId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryCategoryService.UpdateIsActive(laboratoryCategoryId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _laboratoryCategoryService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _laboratoryCategoryService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryCategory(int laboratoryCategoryId)
        {
            // Validation
            var validationResult = await _laboratoryCategoryService.IsUsed(laboratoryCategoryId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryCategoryService.RemoveLaboratoryCategory(laboratoryCategoryId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }



        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryCategories([FromBody]List<LaboratoryCategoryDto> laboratoryCategoryDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryCategoryDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryCategoryValidator()).ValidateAsync(laboratoryCategoryDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = laboratoryCategoryDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                laboratoryCategoryDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = laboratoryCategoryDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryCategoryDtos.Select(x =>
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

            _response = await _laboratoryCategoryService.ImportLaboratoryCategories(laboratoryCategoryDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        

        [HttpPost]
        public IActionResult ExportLaboratoryCategories(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryCategoryFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _laboratoryCategoryService.ExportLaboratoryCategories(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}