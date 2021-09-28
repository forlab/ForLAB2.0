using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.API.Helpers;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.ContactInfo;
using ForLab.Services.CMS.ContactInfo;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ContactInfosController : BaseController
    {
        private readonly IContactInfoService _contactInfoService;

        public ContactInfosController(
            IContactInfoService contactInfoService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _contactInfoService = contactInfoService;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseDTO> GetContactInfoDetails()
        {
            _response = await _contactInfoService.GetContactInfoDetails();
            return _response;
        }


        [Authorize(Roles = "SuperAdmin, CountryLevel, RegionLevel, LaboratoryLevel")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateContactInfo([FromBody]ContactInfoDto contactInfoDto)
        {
            // validate
            var validationResult = await (new ContactInfoValidator()).ValidateAsync(contactInfoDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage));
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            contactInfoDto.UpdatedBy = LoggedInUserId;
            contactInfoDto.UpdatedOn = DateTime.Now;
            // Set relation variables with null to avoid unexpected EF errors
            contactInfoDto.Creator = null;
            contactInfoDto.Updator = null;

            _response = await _contactInfoService.UpdateContactInfo(contactInfoDto);
            return _response;
        }
    }
}
