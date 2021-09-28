using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.Country;
using ForLab.Services.Lookup.Country;
using ForLab.Validators.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class CountriesController : BaseController
    {
        private readonly ICountryService _countryService;

        public CountriesController(
            ICountryService countryService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _countryService = countryService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] CountryFilterDto filterDto)
        {
            _response = _countryService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] CountryFilterDto filterDto)
        {
            _response = _countryService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetCountryDetails(int countryId)
        {
            _response = await _countryService.GetCountryDetails(countryId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateCountry([FromBody]CountryDto countryDto)
        {
            //Validation
            var validationResult = await (new CountryValidator(_countryService)).ValidateAsync(countryDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            countryDto.Creator = null;
            countryDto.Updator = null;
            countryDto.CreatedBy = LoggedInUserId;
            countryDto.CreatedOn = DateTime.Now;
            countryDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            countryDto.ContinentName = null;
            countryDto.CountryPeriodName = null;

            _response = await _countryService.CreateCountry(countryDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateCountry([FromBody]CountryDto countryDto)
        {
            // Validation
            var validationResult = await (new CountryValidator(_countryService)).ValidateAsync(countryDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            countryDto.Creator = null;
            countryDto.Updator = null;
            countryDto.UpdatedBy = LoggedInUserId;
            countryDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            countryDto.ContinentName = null;
            countryDto.CountryPeriodName = null;

            _response = await _countryService.UpdateCountry(countryDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int countryId, bool isActive)
        {
            if(!isActive)
            {
                // Validation
                var validationResult = await _countryService.IsUsed(countryId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _countryService.UpdateIsActive(LoggedInUserId, countryId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _countryService.UpdateIsActiveForSelected(LoggedInUserId, ids, isActive);
            return _response;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveCountry(int countryId)
        {
            // Validation
            var validationResult = await _countryService.IsUsed(countryId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _countryService.RemoveCountry(countryId, LoggedInUserId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> ImportCountries([FromBody]List<CountryDto> countryDtos)
        {
            // Validation
            for (var i = 0; i < countryDtos.Count; i++)
            {
                var validationResult = await (new ImportCountryValidator()).ValidateAsync(countryDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = countryDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                countryDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = countryDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }
            var isDuplicated2 = countryDtos.GroupBy(x => x.ShortCode.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated2)
            {
                countryDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = countryDtos.GroupBy(x => x.ShortCode.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Short Code" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Short Code";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            countryDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.IsActive = IsSuperAdmin ? true : false;
                x.ContinentName = null;
                x.CountryPeriodName = null;
                return x;
            }).ToList();

            _response = await _countryService.ImportCountries(countryDtos);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportCountries(int? pageIndex = null, int? pageSize = null, [FromQuery] CountryFilterDto filterDto = null)
        {
            var file = _countryService.ExportCountries(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}