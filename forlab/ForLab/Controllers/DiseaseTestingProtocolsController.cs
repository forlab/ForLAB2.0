
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Disease.DiseaseTestingProtocol;
using ForLab.Services.Disease.DiseaseTestingProtocol;
using ForLab.Validators.DiseaseTestingProtocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class DiseaseTestingProtocolsController : BaseController
    {
        private readonly IDiseaseTestingProtocolService _diseaseTestingProtocolService;

        public DiseaseTestingProtocolsController(
            IDiseaseTestingProtocolService diseaseTestingProtocolService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _diseaseTestingProtocolService = diseaseTestingProtocolService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] DiseaseTestingProtocolFilterDto filterDto)
        {
            _response = _diseaseTestingProtocolService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] DiseaseTestingProtocolFilterDto filterDto)
        {
            _response = _diseaseTestingProtocolService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetDiseaseTestingProtocolDetails(int diseaseTestingProtocolId)
        {
            _response = await _diseaseTestingProtocolService.GetDiseaseTestingProtocolDetails(diseaseTestingProtocolId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateDiseaseTestingProtocol([FromBody]DiseaseTestingProtocolDto diseaseTestingProtocolDto)
        {
            //Validation
            var validationResult = await (new DiseaseTestingProtocolValidator(_diseaseTestingProtocolService)).ValidateAsync(diseaseTestingProtocolDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            diseaseTestingProtocolDto.Creator = null;
            diseaseTestingProtocolDto.Updator = null;
            diseaseTestingProtocolDto.CreatedBy = LoggedInUserId;
            diseaseTestingProtocolDto.CreatedOn = DateTime.Now;
            //diseaseTestingProtocolDto.IsActive = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            diseaseTestingProtocolDto.DiseaseName = null;
            diseaseTestingProtocolDto.TestingProtocolName = null;

            _response = await _diseaseTestingProtocolService.CreateDiseaseTestingProtocol(diseaseTestingProtocolDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateDiseaseTestingProtocol([FromBody]DiseaseTestingProtocolDto diseaseTestingProtocolDto)
        {
            // Validation
            var validationResult = await (new DiseaseTestingProtocolValidator(_diseaseTestingProtocolService)).ValidateAsync(diseaseTestingProtocolDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            diseaseTestingProtocolDto.Creator = null;
            diseaseTestingProtocolDto.Updator = null;
            diseaseTestingProtocolDto.UpdatedBy = LoggedInUserId;
            diseaseTestingProtocolDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            diseaseTestingProtocolDto.DiseaseName = null;
            diseaseTestingProtocolDto.TestingProtocolName = null;

            _response = await _diseaseTestingProtocolService.UpdateDiseaseTestingProtocol(diseaseTestingProtocolDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int diseaseTestingProtocolId, bool isActive)
        {
            _response = await _diseaseTestingProtocolService.UpdateIsActive(LoggedInUserId, diseaseTestingProtocolId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveDiseaseTestingProtocol(int diseaseTestingProtocolId)
        {
            _response = await _diseaseTestingProtocolService.RemoveDiseaseTestingProtocol(diseaseTestingProtocolId, LoggedInUserId);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportDiseaseTestingProtocols(int? pageIndex = null, int? pageSize = null, [FromQuery] DiseaseTestingProtocolFilterDto filterDto = null)
        {
            var file = _diseaseTestingProtocolService.ExportDiseaseTestingProtocols(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportTestingProtocols([FromBody]List<DiseaseTestingProtocolDto> diseaseTestingProtocolDtos)
        {
            // Validation
            for (var i = 0; i < diseaseTestingProtocolDtos.Count; i++)
            {
                var validationResult = await (new ImportDiseaseTestingProtocolValidator()).ValidateAsync(diseaseTestingProtocolDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Set relation variables with null to avoid unexpected EF errors
            diseaseTestingProtocolDtos.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.TestingProtocolName = null;
                x.DiseaseName = null;
                return x;
            }).ToList();

            _response = await _diseaseTestingProtocolService.ImportDiseaseTestingProtocols(diseaseTestingProtocolDtos);
            return _response;
        }

    }
}
