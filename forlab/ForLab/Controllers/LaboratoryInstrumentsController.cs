using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryInstrument;
using ForLab.Services.Laboratory.LaboratoryInstrument;
using ForLab.Validators.LaboratoryInstrument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class LaboratoryInstrumentsController : BaseController
    {
        private readonly ILaboratoryInstrumentService _laboratoryInstrumentService;

        public LaboratoryInstrumentsController(
            ILaboratoryInstrumentService laboratoryInstrumentService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _laboratoryInstrumentService = laboratoryInstrumentService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] LaboratoryInstrumentFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryInstrumentService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] LaboratoryInstrumentFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _laboratoryInstrumentService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetLaboratoryInstrumentDetails(int laboratoryInstrumentId)
        {
            _response = await _laboratoryInstrumentService.GetLaboratoryInstrumentDetails(laboratoryInstrumentId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateLaboratoryInstrument([FromBody]LaboratoryInstrumentDto laboratoryInstrumentDto)
        {
            //Validation
            var validationResult = await (new LaboratoryInstrumentValidator(_laboratoryInstrumentService)).ValidateAsync(laboratoryInstrumentDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryInstrumentDto.Creator = null;
            laboratoryInstrumentDto.Updator = null;
            laboratoryInstrumentDto.CreatedBy = LoggedInUserId;
            laboratoryInstrumentDto.CreatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryInstrumentDto.LaboratoryName = null;
            laboratoryInstrumentDto.InstrumentName = null;

            _response = await _laboratoryInstrumentService.CreateLaboratoryInstrument(laboratoryInstrumentDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateLaboratoryInstrument([FromBody]LaboratoryInstrumentDto laboratoryInstrumentDto)
        {
            // Validation
            var validationResult = await (new LaboratoryInstrumentValidator(_laboratoryInstrumentService)).ValidateAsync(laboratoryInstrumentDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            laboratoryInstrumentDto.Creator = null;
            laboratoryInstrumentDto.Updator = null;
            laboratoryInstrumentDto.UpdatedBy = LoggedInUserId;
            laboratoryInstrumentDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryInstrumentDto.LaboratoryName = null;
            laboratoryInstrumentDto.InstrumentName = null;

            _response = await _laboratoryInstrumentService.UpdateLaboratoryInstrument(laboratoryInstrumentDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int laboratoryInstrumentId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _laboratoryInstrumentService.IsUsed(laboratoryInstrumentId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _laboratoryInstrumentService.UpdateIsActive(laboratoryInstrumentId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveLaboratoryInstrument(int laboratoryInstrumentId)
        {
            // Validation
            var validationResult = await _laboratoryInstrumentService.IsUsed(laboratoryInstrumentId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _laboratoryInstrumentService.RemoveLaboratoryInstrument(laboratoryInstrumentId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }
        
        
        [HttpPost]
        public IActionResult ExportLaboratoryInstruments(int? pageIndex = null, int? pageSize = null, [FromQuery] LaboratoryInstrumentFilterDto filterDto = null)
        {
            var file = _laboratoryInstrumentService.ExportLaboratoryInstruments(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

        
        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportLaboratoryInstruments([FromBody]List<LaboratoryInstrumentDto> laboratoryInstrumentDtos)
        {
            // Validation
            for (var i = 0; i < laboratoryInstrumentDtos.Count; i++)
            {
                var validationResult = await (new ImportLaboratoryInstrumentValidator()).ValidateAsync(laboratoryInstrumentDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication (LaboratoryId with InstrumentId)
            var isDuplicated = laboratoryInstrumentDtos.GroupBy(x => new
            {
                LaboratoryId = x.LaboratoryId,
                InstrumentId = x.InstrumentId
            }).Any(g => g.Count() > 1);

            if (isDuplicated)
            {
                laboratoryInstrumentDtos.Select((x, i) => { x.Id = i; return x; }).ToList();

                var duplicates = laboratoryInstrumentDtos.GroupBy(x => new
                {
                    LaboratoryId = x.LaboratoryId,
                    InstrumentId = x.InstrumentId
                }).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the Laboratory with the Instrument" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Laboratory with the Instrument";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            laboratoryInstrumentDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.LaboratoryName = null;
                x.InstrumentName = null;
                return x;
            }).ToList();

            _response = await _laboratoryInstrumentService.ImportLaboratoryInstruments(laboratoryInstrumentDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }

    }
}
