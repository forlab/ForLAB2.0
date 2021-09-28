using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Disease.Disease;
using ForLab.Services.Disease.Disease;
using ForLab.Validators.Disease;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class DiseasesController : BaseController
    {
        private readonly IDiseaseService _diseaseService;

        public DiseasesController(
            IDiseaseService diseaseService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _diseaseService = diseaseService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] DiseaseFilterDto filterDto)
        {
            _response = _diseaseService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] DiseaseFilterDto filterDto)
        {
            _response = _diseaseService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetDiseaseDetails(int diseaseId)
        {
            _response = await _diseaseService.GetDiseaseDetails(diseaseId);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportDiseases(int? pageIndex = null, int? pageSize = null, [FromQuery] DiseaseFilterDto filterDto = null)
        {
            var file = _diseaseService.ExportDiseases(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
        
        
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateDisease([FromBody]DiseaseDto diseaseDto)
        {
            //Validation
            var validationResult = await (new DiseaseValidator(_diseaseService)).ValidateAsync(diseaseDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }


            // Set variables by the system
            diseaseDto.Creator = null;
            diseaseDto.Updator = null;
            diseaseDto.CreatedBy = LoggedInUserId;
            diseaseDto.CreatedOn = DateTime.Now;

            _response = await _diseaseService.CreateDisease(diseaseDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateDisease([FromBody]DiseaseDto diseaseDto)
        {
            // Validation
            var validationResult = await (new DiseaseValidator(_diseaseService)).ValidateAsync(diseaseDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            diseaseDto.Creator = null;
            diseaseDto.Updator = null;
            diseaseDto.UpdatedBy = LoggedInUserId;
            diseaseDto.UpdatedOn = DateTime.Now;

            _response = await _diseaseService.UpdateDisease(diseaseDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int diseaseId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _diseaseService.IsUsed(diseaseId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _diseaseService.UpdateIsActive(LoggedInUserId, diseaseId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _diseaseService.UpdateIsActiveForSelected(LoggedInUserId, ids, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveDisease(int diseaseId)
        {
            // Validation
            var validationResult = await _diseaseService.IsUsed(diseaseId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _diseaseService.RemoveDisease(diseaseId, LoggedInUserId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> ImportDiseases([FromBody]List<DiseaseDto> diseaseDtos)
        {
            // Validation
            for (var i = 0; i < diseaseDtos.Count; i++)
            {
                var validationResult = await (new ImportDiseaseValidator()).ValidateAsync(diseaseDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var duplicates = diseaseDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (duplicates)
            {
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            diseaseDtos.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                return x;
            }).ToList();

            _response = await _diseaseService.ImportDiseases(diseaseDtos);
            return _response;
        }
    }
}
