using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Vendor.Vendor;
using ForLab.Services.Vendor.Vendor;
using ForLab.Validators.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class VendorsController : BaseController
    {
        private readonly IVendorService _vendorService;

        public VendorsController(
            IVendorService vendorService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _vendorService = vendorService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] VendorFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _vendorService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] VendorFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _vendorService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetVendorDetails(int vendorId)
        {
            _response = await _vendorService.GetVendorDetails(vendorId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateVendor([FromBody] VendorDto vendorDto)
        {
            // Validation
            var validationResult = await (new VendorValidator(_vendorService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(vendorDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            vendorDto.Creator = null;
            vendorDto.Updator = null;
            vendorDto.CreatedBy = LoggedInUserId;
            vendorDto.CreatedOn = DateTime.Now;

            _response = await _vendorService.CreateVendor(vendorDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateVendor([FromBody] VendorDto vendorDto)
        {
            // Validation
            var validationResult = await (new VendorValidator(_vendorService, LoggedInUserId, IsSuperAdmin)).ValidateAsync(vendorDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            vendorDto.Creator = null;
            vendorDto.Updator = null;
            vendorDto.UpdatedBy = LoggedInUserId;
            vendorDto.UpdatedOn = DateTime.Now;

            _response = await _vendorService.UpdateVendor(vendorDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int vendorId, bool isActive)
        {
            if(!isActive)
            {
                // Validation
                var validationResult = await _vendorService.IsUsed(vendorId);
                if (!validationResult.IsPassed)
                {
                    return validationResult;
                }
            }

            _response = await _vendorService.UpdateIsActive(vendorId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActiveForSelected([FromBody] List<int> ids, bool isActive)
        {
            _response = await _vendorService.UpdateIsActiveForSelected(ids, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveVendor(int vendorId)
        {
            // Validation
            var validationResult = await _vendorService.IsUsed(vendorId);
            if (!validationResult.IsPassed)
            {
                return validationResult;
            }

            _response = await _vendorService.RemoveVendor(vendorId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportVendors([FromBody]List<VendorDto> vendorDtos)
        {
            // Validation
            for (var i = 0; i < vendorDtos.Count; i++)
            {
                var validationResult = await (new ImportVendorValidator()).ValidateAsync(vendorDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var isDuplicated = vendorDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated)
            {
                vendorDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = vendorDtos.GroupBy(x => x.Name.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the name" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }
            var isDuplicated2 = vendorDtos.GroupBy(x => x.Email.Trim().ToLower()).Any(g => g.Count() > 1);
            if (isDuplicated2)
            {
                vendorDtos.Select((x, i) => { x.Id = i; return x; }).ToList();
                var duplicates = vendorDtos.GroupBy(x => x.Email.Trim().ToLower()).Where(x => x.Count() > 1).SelectMany(x => x.Skip(1)).ToList();

                _response.Data = duplicates.ConvertAll(x => new { RowNumber = x.Id + 1, Message = $"You should not duplicate the email" });
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the email";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            vendorDtos.Select(x =>
            {
                x.Id = 0;
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                return x;
            }).ToList();

            _response = await _vendorService.ImportVendors(vendorDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportVendors(int? pageIndex = null, int? pageSize = null, [FromQuery] VendorFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _vendorService.ExportVendors(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}
