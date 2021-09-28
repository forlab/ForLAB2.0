using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.Region;
using ForLab.Services.Lookup.Region;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class RegionsController : BaseController
    {
        private readonly IRegionService _regionService;

        public RegionsController(
            IRegionService regionService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _regionService = regionService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] RegionFilterDto filterDto)
        {
            _response = _regionService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] RegionFilterDto filterDto)
        {
            _response = _regionService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetRegionDetails(int countryId)
        {
            _response = await _regionService.GetRegionDetails(countryId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateRegion([FromBody]RegionDto regionDto)
        {
            //Validation
            var validationResult = await (new RegionValidator(_regionService)).ValidateAsync(regionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            regionDto.Creator = null;
            regionDto.Updator = null;
            regionDto.CreatedBy = LoggedInUserId;
            regionDto.CreatedOn = DateTime.Now;
            regionDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            regionDto.CountryName = null;
            regionDto.CountryFlag = null;

            _response = await _regionService.CreateRegion(regionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateRegion([FromBody]RegionDto regionDto)
        {
            // Validation
            var validationResult = await (new RegionValidator(_regionService)).ValidateAsync(regionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            regionDto.Creator = null;
            regionDto.Updator = null;
            regionDto.UpdatedBy = LoggedInUserId;
            regionDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            regionDto.CountryName = null;
            regionDto.CountryFlag = null;

            _response = await _regionService.UpdateRegion(regionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int regionId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _regionService.IsUsed(regionId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _regionService.UpdateIsActive(LoggedInUserId, regionId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _regionService.UpdateIsActiveForSelected(LoggedInUserId, ids, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveRegion(int regionId)
        {
            // Validation
            var validationResult = await _regionService.IsUsed(regionId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _regionService.RemoveRegion(regionId, LoggedInUserId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> ImportRegions([FromBody]List<RegionDto> regionDtos)
        {
            // Validation
            for (var i = 0; i < regionDtos.Count; i++)
            {
                var validationResult = await (new ImportRegionValidator()).ValidateAsync(regionDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (RegionName with CountryId)
            var isDuplicated = regionDtos.GroupBy(x => new
                                    {
                                        Name = x.Name.Trim().ToLower(),
                                        CountryId = x.CountryId
                                    }).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                regionDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = regionDtos.GroupBy(x => new
                {
                    Name = x.Name.Trim().ToLower(),
                    CountryId = x.CountryId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the region name with the country" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the region name with the country";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            regionDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.IsActive = IsSuperAdmin ? true : false;
                x.CountryName = null;
                x.CountryFlag = null;
                return x;
            }).ToList();

            _response = await _regionService.ImportRegions(regionDtos);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportRegions(int? pageIndex = null, int? pageSize = null, [FromQuery] RegionFilterDto filterDto = null)
        {
            var file = _regionService.ExportRegions(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}