using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Disease.CountryDiseaseIncident;
using ForLab.Services.Disease.CountryDiseaseIncident;
using ForLab.Validators.CountryDiseaseIncident;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class CountryDiseaseIncidentsController : BaseController
    {
        private readonly ICountryDiseaseIncidentService _countryDiseaseIncidentService;

        public CountryDiseaseIncidentsController(
            ICountryDiseaseIncidentService countryDiseaseIncidentService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _countryDiseaseIncidentService = countryDiseaseIncidentService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] CountryDiseaseIncidentFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _countryDiseaseIncidentService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] CountryDiseaseIncidentFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _countryDiseaseIncidentService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetCountryDiseaseIncidentDetails(int countryDiseaseIncidentId)
        {
            _response = await _countryDiseaseIncidentService.GetCountryDiseaseIncidentDetails(countryDiseaseIncidentId);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportCountryDiseaseIncidents(int? pageIndex = null, int? pageSize = null, [FromQuery] CountryDiseaseIncidentFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _countryDiseaseIncidentService.ExportCountryDiseaseIncidents(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateCountryDiseaseIncident([FromBody]CountryDiseaseIncidentDto countryDiseaseIncidentDto)
        {
            //Validation
            var validationResult = await (new CountryDiseaseIncidentValidator(_countryDiseaseIncidentService)).ValidateAsync(countryDiseaseIncidentDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            countryDiseaseIncidentDto.Creator = null;
            countryDiseaseIncidentDto.Updator = null;
            countryDiseaseIncidentDto.CreatedBy = LoggedInUserId;
            countryDiseaseIncidentDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            countryDiseaseIncidentDto.DiseaseName = null;
            countryDiseaseIncidentDto.CountryName = null;

            _response = await _countryDiseaseIncidentService.CreateCountryDiseaseIncident(countryDiseaseIncidentDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateCountryDiseaseIncident([FromBody]CountryDiseaseIncidentDto countryDiseaseIncidentDto)
        {
            // Validation
            var validationResult = await (new CountryDiseaseIncidentValidator(_countryDiseaseIncidentService)).ValidateAsync(countryDiseaseIncidentDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            countryDiseaseIncidentDto.Creator = null;
            countryDiseaseIncidentDto.Updator = null;
            countryDiseaseIncidentDto.UpdatedBy = LoggedInUserId;
            countryDiseaseIncidentDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            countryDiseaseIncidentDto.DiseaseName = null;
            countryDiseaseIncidentDto.CountryName = null;

            _response = await _countryDiseaseIncidentService.UpdateCountryDiseaseIncident(countryDiseaseIncidentDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int countryDiseaseIncidentId, bool isActive)
        {
            _response = await _countryDiseaseIncidentService.UpdateIsActive(LoggedInUserId, countryDiseaseIncidentId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _countryDiseaseIncidentService.UpdateIsActiveForSelected(LoggedInUserId, ids, isActive);
            return _response;
        }

        
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveCountryDiseaseIncident(int countryDiseaseIncidentId)
        {
            _response = await _countryDiseaseIncidentService.RemoveCountryDiseaseIncident(countryDiseaseIncidentId, LoggedInUserId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> ImportCountryDiseaseIncidents([FromBody]List<CountryDiseaseIncidentDto> countryDiseaseIncidentDtos)
        {
            // Validation
            for (var i = 0; i < countryDiseaseIncidentDtos.Count; i++)
            {
                var validationResult = await (new ImportCountryDiseaseIncidentValidator()).ValidateAsync(countryDiseaseIncidentDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (CountryId, DiseaseId, Year)
            var isDuplicated = countryDiseaseIncidentDtos.GroupBy(x => new
            {
                CountryId = x.CountryId,
                DiseaseId = x.DiseaseId,
                Year = x.Year
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                countryDiseaseIncidentDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = countryDiseaseIncidentDtos.GroupBy(x => new
                {
                    CountryId = x.CountryId,
                    DiseaseId = x.DiseaseId,
                    Year = x.Year
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate Country Disease with the the same Year" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate Country Disease with the the same Year";
                return _response;
            }

            // Set relation variables with null to avoid unexpected EF errors
            countryDiseaseIncidentDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.CountryName = null;
                x.DiseaseName = null;
                return x;
            }).ToList();

            _response = await _countryDiseaseIncidentService.ImportCountryDiseaseIncidents(countryDiseaseIncidentDtos);
            return _response;
        }

    }
}
