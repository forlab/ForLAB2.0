using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.InquiryQuestion;
using ForLab.DTO.CMS.InquiryQuestionReply;
using ForLab.Services.CMS.InquiryQuestion;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class InquiryQuestionsController : BaseController
    {
        private readonly IInquiryQuestionService _inquiryQuestionService;

        public InquiryQuestionsController(
            IInquiryQuestionService inquiryQuestionService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _inquiryQuestionService = inquiryQuestionService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] InquiryQuestionFilterDto filterDto)
        {
            _response = _inquiryQuestionService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] InquiryQuestionFilterDto filterDto)
        {
            _response = _inquiryQuestionService.GetAllAsDrp(filterDto);
            return _response;
        }


        [HttpGet]
        public async Task<IResponseDTO> GetInquiryQuestionDetails(int inquiryQuestionId)
        {
            _response = await _inquiryQuestionService.GetInquiryQuestionDetails(inquiryQuestionId);
            return _response;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IResponseDTO> CreateInquiryQuestion([FromBody]InquiryQuestionDto inquiryQuestionDto)
        {
            //Validation
            var validationResult = await (new InquiryQuestionValidator(_inquiryQuestionService)).ValidateAsync(inquiryQuestionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            inquiryQuestionDto.Creator = null;
            inquiryQuestionDto.Updator = null;
            if(LoggedInUserId == 0)
            {
                inquiryQuestionDto.CreatedBy = null;
            }
            inquiryQuestionDto.CreatedOn = DateTime.Now;
            _response = await _inquiryQuestionService.CreateInquiryQuestion(inquiryQuestionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateInquiryQuestion([FromBody]InquiryQuestionDto inquiryQuestionDto)
        {
            // Validation
            var validationResult = await (new InquiryQuestionValidator(_inquiryQuestionService)).ValidateAsync(inquiryQuestionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            inquiryQuestionDto.Creator = null;
            inquiryQuestionDto.Updator = null;
            inquiryQuestionDto.UpdatedBy = LoggedInUserId;
            inquiryQuestionDto.UpdatedOn = DateTime.Now;

            _response = await _inquiryQuestionService.UpdateInquiryQuestion(inquiryQuestionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int inquiryQuestionId, bool isActive)
        {

            _response = await _inquiryQuestionService.UpdateIsActive(LoggedInUserId, inquiryQuestionId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveInquiryQuestion(int inquiryQuestionId)
        {

            _response = await _inquiryQuestionService.RemoveInquiryQuestion(inquiryQuestionId, LoggedInUserId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateInquiryQuestionReply([FromBody] InquiryQuestionReplyDto inquiryQuestionReplyDto)
        {
            // Validation
            var validationResult = await (new InquiryQuestionReplyValidator()).ValidateAsync(inquiryQuestionReplyDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            inquiryQuestionReplyDto.Creator = null;
            inquiryQuestionReplyDto.Updator = null;
            inquiryQuestionReplyDto.CreatedBy = LoggedInUserId;
            inquiryQuestionReplyDto.CreatedOn = DateTime.Now;

            _response = await _inquiryQuestionService.CreateInquiryQuestionReply(inquiryQuestionReplyDto);
            return _response;
        }
    }
}