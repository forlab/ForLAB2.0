using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.API.Helpers;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.Feature;
using ForLab.Services.CMS.Feature;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class FeaturesController : BaseController
    {
        private readonly IFeatureService _featureService;

        public FeaturesController(
            IFeatureService featureService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _featureService = featureService;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] FeatureFilterDto filterDto)
        {
            _response = _featureService.GetAll(ServerRootPath, pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] FeatureFilterDto filterDto)
        {
            _response = _featureService.GetAllAsDrp(filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseDTO> GetFeatureDetails(int featureId)
        {
            _response = await _featureService.GetFeatureDetails(ServerRootPath,featureId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateFeature([ModelBinder(BinderType = typeof(JsonModelBinder))] FeatureDto featureDto)
        {
            //Validation
            var validationResult = await (new FeatureValidator(_featureService)).ValidateAsync(featureDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            featureDto.Creator = null;
            featureDto.Updator = null;
            featureDto.CreatedBy = LoggedInUserId;
            featureDto.CreatedOn = DateTime.Now;

            var file = Request?.Form?.Files.Count() > 0 ? Request?.Form?.Files[0] : null;
            _response = await _featureService.CreateFeature(featureDto, file);
          
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateFeature([ModelBinder(BinderType = typeof(JsonModelBinder))] FeatureDto featureDto)
        {
            // Validation
            var validationResult = await (new FeatureValidator(_featureService)).ValidateAsync(featureDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            featureDto.Creator = null;
            featureDto.Updator = null;
            featureDto.UpdatedBy = LoggedInUserId;
            featureDto.UpdatedOn = DateTime.Now;

            var file = Request?.Form?.Files.Count() > 0 ? Request?.Form?.Files[0] : null;
            _response = await _featureService.UpdateFeature(featureDto, file);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int featureId, bool isActive)
        {
            _response = await _featureService.UpdateIsActive(LoggedInUserId, featureId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveFeature(int featureId)
        {
            _response = await _featureService.RemoveFeature(featureId, LoggedInUserId);
            return _response;
        }

    }
}
