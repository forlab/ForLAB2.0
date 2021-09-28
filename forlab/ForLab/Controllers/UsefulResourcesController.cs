using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.API.Helpers;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.UsefulResource;
using ForLab.Services.CMS.UsefulResource;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class UsefulResourcesController : BaseController
    {
        private readonly IUsefulResourceService _usefulResourceService;

        public UsefulResourcesController(
            IUsefulResourceService usefulResourceService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _usefulResourceService = usefulResourceService;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] UsefulResourceFilterDto filterDto)
        {
            _response = _usefulResourceService.GetAll(ServerRootPath, pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] UsefulResourceFilterDto filterDto)
        {
            _response = _usefulResourceService.GetAllAsDrp(filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseDTO> GetUsefulResourceDetails(int usefulResourceId)
        {
            _response = await _usefulResourceService.GetUsefulResourceDetails(usefulResourceId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IResponseDTO> CreateUsefulResources([ModelBinder(BinderType = typeof(JsonModelBinder))] List<UsefulResourceDto> usefulResourceDtos )
        {
            foreach (var item in usefulResourceDtos)
            {
                //Validation
                var validationResult = await (new UsefulResourceValidator(_usefulResourceService)).ValidateAsync(item);
                if (!validationResult.IsValid)
                {
                    _response.IsPassed = false;
                    _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                    _response.Data = null;
                    return _response;
                }

                // Set variables by the system
                item.Creator = null;
                item.Updator = null;
                item.CreatedBy = LoggedInUserId;
                item.CreatedOn = DateTime.Now;
            }

            var files = Request?.Form?.Files?.Count() > 0 ? Request?.Form?.Files?.ToList() : null;
            _response = await _usefulResourceService.CreateUsefulResources(usefulResourceDtos, files);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IResponseDTO> CreateUsefulResource([ModelBinder(BinderType = typeof(JsonModelBinder))] UsefulResourceDto usefulResourceDto)
        {
            //Validation
            var validationResult = await (new UsefulResourceValidator(_usefulResourceService)).ValidateAsync(usefulResourceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            usefulResourceDto.Creator = null;
            usefulResourceDto.Updator = null;
            usefulResourceDto.CreatedBy = LoggedInUserId;
            usefulResourceDto.CreatedOn = DateTime.Now;

            var file = Request?.Form?.Files.Count() > 0 ? Request?.Form?.Files[0] : null;
            _response = await _usefulResourceService.CreateUsefulResource(usefulResourceDto, file);
          
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut, DisableRequestSizeLimit]
        public async Task<IResponseDTO> UpdateUsefulResource([ModelBinder(BinderType = typeof(JsonModelBinder))] UsefulResourceDto usefulResourceDto)
        {
            // Validation
            var validationResult = await (new UsefulResourceValidator(_usefulResourceService)).ValidateAsync(usefulResourceDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            usefulResourceDto.Creator = null;
            usefulResourceDto.Updator = null;
            usefulResourceDto.UpdatedBy = LoggedInUserId;
            usefulResourceDto.UpdatedOn = DateTime.Now;

            var file = Request?.Form?.Files.Count() > 0 ? Request?.Form?.Files[0] : null;
            _response = await _usefulResourceService.UpdateUsefulResource(usefulResourceDto, file);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int usefulResourceId, bool isActive)
        {
            _response = await _usefulResourceService.UpdateIsActive(LoggedInUserId, usefulResourceId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveUsefulResource(int usefulResourceId)
        {
            _response = await _usefulResourceService.RemoveUsefulResource(usefulResourceId, LoggedInUserId);
            return _response;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IResponseDTO> IncrementDownloadCount(int usefulResourceId)
        {
            _response = await _usefulResourceService.IncrementDownloadCount(usefulResourceId);
            return _response;
        }
    }
}
