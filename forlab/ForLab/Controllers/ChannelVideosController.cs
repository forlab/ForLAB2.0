using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.API.Helpers;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.ChannelVideo;
using ForLab.Services.CMS.ChannelVideo;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ChannelVideosController : BaseController
    {
        private readonly IChannelVideoService _channelVideoService;

        public ChannelVideosController(
            IChannelVideoService channelVideoService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _channelVideoService = channelVideoService;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ChannelVideoFilterDto filterDto)
        {
            _response = _channelVideoService.GetAll(ServerRootPath, pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ChannelVideoFilterDto filterDto)
        {
            _response = _channelVideoService.GetAllAsDrp(filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseDTO> GetChannelVideoDetails(int channelVideoId)
        {
            _response = await _channelVideoService.GetChannelVideoDetails(channelVideoId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IResponseDTO> CreateChannelVideo([ModelBinder(BinderType = typeof(JsonModelBinder))]ChannelVideoDto channelVideoDto)
        {
            //Validation
            var validationResult = await (new ChannelVideoValidator(_channelVideoService)).ValidateAsync(channelVideoDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            channelVideoDto.Creator = null;
            channelVideoDto.Updator = null;
            channelVideoDto.CreatedBy = LoggedInUserId;
            channelVideoDto.CreatedOn = DateTime.Now;

            var file = Request?.Form?.Files.Count() > 0 ? Request?.Form?.Files[0] : null;
            _response = await _channelVideoService.CreateChannelVideo(channelVideoDto, file);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateChannelVideo([ModelBinder(BinderType = typeof(JsonModelBinder))] ChannelVideoDto channelVideoDto)
        {
            // Validation
            var validationResult = await (new ChannelVideoValidator(_channelVideoService)).ValidateAsync(channelVideoDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            channelVideoDto.Creator = null;
            channelVideoDto.Updator = null;
            channelVideoDto.UpdatedBy = LoggedInUserId;
            channelVideoDto.UpdatedOn = DateTime.Now;

            var file = Request?.Form?.Files.Count() > 0 ? Request?.Form?.Files[0] : null;
            _response = await _channelVideoService.UpdateChannelVideo(channelVideoDto, file);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int channelVideoId, bool isActive)
        {

            _response = await _channelVideoService.UpdateIsActive(LoggedInUserId, channelVideoId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveChannelVideo(int channelVideoId)
        {

            _response = await _channelVideoService.RemoveChannelVideo(channelVideoId, LoggedInUserId);
            return _response;
        }
    }
}