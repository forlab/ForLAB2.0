using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.Instrument;
using ForLab.Services.Product.Instrument;
using ForLab.Validators.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class InstrumentsController : BaseController
    {
        private readonly IInstrumentService _instrumentService;

        public InstrumentsController(
            IInstrumentService instrumentService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _instrumentService = instrumentService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] InstrumentFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _instrumentService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] InstrumentFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _instrumentService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetInstrumentDetails(int instrumentId)
        {
            _response = await _instrumentService.GetInstrumentDetails(instrumentId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateInstrument([FromBody]InstrumentDto instrumentDto)
        {
            //Validation
            var validationResult = await (new InstrumentValidator(_instrumentService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(instrumentDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            instrumentDto.Creator = null;
            instrumentDto.Updator = null;
            instrumentDto.CreatedBy = LoggedInUserId;
            instrumentDto.CreatedOn = DateTime.Now;
            instrumentDto.Shared = IsSuperAdmin ? true : false;
            // Set relation variables with null to avoid unexpected EF errors
            instrumentDto.VendorName = null;
            instrumentDto.ControlRequirementUnitName = null;
            instrumentDto.TestingAreaName = null;
            instrumentDto.ThroughPutUnitName = null;

            _response = await _instrumentService.CreateInstrument(instrumentDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateInstrument([FromBody]InstrumentDto instrumentDto)
        {
            // Validation
            var validationResult = await (new InstrumentValidator(_instrumentService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(instrumentDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            instrumentDto.Creator = null;
            instrumentDto.Updator = null;
            instrumentDto.UpdatedBy = LoggedInUserId;
            instrumentDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            instrumentDto.VendorName = null;
            instrumentDto.ControlRequirementUnitName = null;
            instrumentDto.TestingAreaName = null;
            instrumentDto.ThroughPutUnitName = null;

            _response = await _instrumentService.UpdateInstrument(instrumentDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int instrumentId, bool isActive)
        {
            if (!isActive)
            {
                // Validation
                var validationResult = await _instrumentService.IsUsed(instrumentId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _instrumentService.UpdateIsActive(instrumentId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _instrumentService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateSharedForSelected([FromBody] List<int> ids, bool shared)
        {
            _response = await _instrumentService.UpdateSharedForSelected(ids, shared, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveInstrument(int instrumentId)
        {
            // Validation
            var validationResult = await _instrumentService.IsUsed(instrumentId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _instrumentService.RemoveInstrument(instrumentId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportInstruments([FromBody]List<InstrumentDto> instrumentDtos)
        {
            // Validation
            for (var i = 0; i < instrumentDtos.Count; i++)
            {
                var validationResult = await (new ImportInstrumentValidator()).ValidateAsync(instrumentDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = instrumentDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                instrumentDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = instrumentDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            instrumentDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                x.ControlRequirementUnitName = null;
                x.ReagentSystemName = null;
                x.TestingAreaName = null;
                x.ThroughPutUnitName = null;
                x.VendorName = null;
                return x;
            }).ToList();

            _response = await _instrumentService.ImportInstruments(instrumentDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportInstruments(int? pageIndex = null, int? pageSize = null, [FromQuery] InstrumentFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _instrumentService.ExportInstruments(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}