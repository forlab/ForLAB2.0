using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.Vendor.VendorContact;
using ForLab.Services.Vendor.VendorContact;
using ForLab.Validators.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class VendorContactsController : BaseController
    {
        private readonly IVendorContactService _vendorContactService;

        public VendorContactsController(
            IVendorContactService vendorContactService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _vendorContactService = vendorContactService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] VendorContactFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _vendorContactService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] VendorContactFilterDto filterDto)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            _response = _vendorContactService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetVendorContactDetails(int vendorContactId)
        {
            _response = await _vendorContactService.GetVendorContactDetails(vendorContactId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> CreateVendorContact([FromBody] VendorContactDto vendorContactDto)
        {
            // Validation
            var validationResult = await (new VendorContactValidator(_vendorContactService)).ValidateAsync(vendorContactDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            vendorContactDto.Creator = null;
            vendorContactDto.Updator = null;
            vendorContactDto.CreatedBy = LoggedInUserId;
            vendorContactDto.CreatedOn = DateTime.Now;

            _response = await _vendorContactService.CreateVendorContact(vendorContactDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateVendorContact([FromBody] VendorContactDto vendorContactDto)
        {
            // Validation
            var validationResult = await (new VendorContactValidator(_vendorContactService)).ValidateAsync(vendorContactDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            vendorContactDto.Creator = null;
            vendorContactDto.Updator = null;
            vendorContactDto.UpdatedBy = LoggedInUserId;
            vendorContactDto.UpdatedOn = DateTime.Now;

            _response = await _vendorContactService.UpdateVendorContact(vendorContactDto, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int vendorContactId, bool isActive)
        {
            _response = await _vendorContactService.UpdateIsActive(vendorContactId, isActive, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveVendorContact(int vendorContactId)
        {
            _response = await _vendorContactService.RemoveVendorContact(vendorContactId, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPost]
        public async Task<IResponseDTO> ImportVendorContacts([FromBody]List<VendorContactDto> vendorContactDtos)
        {
            // Validation
            for (var i = 0; i < vendorContactDtos.Count; i++)
            {
                var validationResult = await (new ImportVendorContactValidator()).ValidateAsync(vendorContactDtos[i]);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = $"Error in row '{i + 1}' " + string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }
            }

            // Check the duplication
            var duplicateName = vendorContactDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (duplicateName)
            {
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the name";
                return _response;
            }
            var duplicateEmail = vendorContactDtos.GroupBy(x => x.Name.Trim().ToLower()).Any(g => g.Count() > 1);
            if (duplicateEmail)
            {
                _response.IsPassed = false;
                _response.Message = "You should not duplicate the Email";
                return _response;
            }

            // Set variables by the system
            // Set relation variables with null to avoid unexpected EF errors
            vendorContactDtos.Select(x =>
            {
                x.Creator = null;
                x.Updator = null;
                x.CreatedBy = LoggedInUserId;
                x.CreatedOn = DateTime.Now;
                return x;
            }).ToList();

            _response = await _vendorContactService.ImportVendorContacts(vendorContactDtos, LoggedInUserId, IsSuperAdmin);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportVendorContacts(int? pageIndex = null, int? pageSize = null, [FromQuery] VendorContactFilterDto filterDto = null)
        {
            // Set Data
            filterDto.LoggedInUserId = LoggedInUserId;
            filterDto.IsSuperAdmin = IsSuperAdmin;

            var file = _vendorContactService.ExportVendorContacts(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
