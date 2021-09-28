using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.FrequentlyAskedQuestion;
using ForLab.Services.CMS.FrequentlyAskedQuestion;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class FrequentlyAskedQuestionsController : BaseController
    {
        private readonly IFrequentlyAskedQuestionService _frequentlyAskedQuestionService;

        public FrequentlyAskedQuestionsController(
            IFrequentlyAskedQuestionService frequentlyAskedQuestionService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _frequentlyAskedQuestionService = frequentlyAskedQuestionService;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] FrequentlyAskedQuestionFilterDto filterDto)
        {
            _response = _frequentlyAskedQuestionService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] FrequentlyAskedQuestionFilterDto filterDto)
        {
            _response = _frequentlyAskedQuestionService.GetAllAsDrp(filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseDTO> GetFrequentlyAskedQuestionDetails(int frequentlyAskedQuestionId)
        {
            _response = await _frequentlyAskedQuestionService.GetFrequentlyAskedQuestionDetails(frequentlyAskedQuestionId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateFrequentlyAskedQuestion([FromBody]FrequentlyAskedQuestionDto frequentlyAskedQuestionDto)
        {
            //Validation
            var validationResult = await (new FrequentlyAskedQuestionValidator(_frequentlyAskedQuestionService)).ValidateAsync(frequentlyAskedQuestionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            frequentlyAskedQuestionDto.Creator = null;
            frequentlyAskedQuestionDto.Updator = null;
            frequentlyAskedQuestionDto.CreatedBy = LoggedInUserId;
            frequentlyAskedQuestionDto.CreatedOn = DateTime.Now;

            _response = await _frequentlyAskedQuestionService.CreateFrequentlyAskedQuestion(frequentlyAskedQuestionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateFrequentlyAskedQuestion([FromBody]FrequentlyAskedQuestionDto frequentlyAskedQuestionDto)
        {
            // Validation
            var validationResult = await (new FrequentlyAskedQuestionValidator(_frequentlyAskedQuestionService)).ValidateAsync(frequentlyAskedQuestionDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            frequentlyAskedQuestionDto.Creator = null;
            frequentlyAskedQuestionDto.Updator = null;
            frequentlyAskedQuestionDto.UpdatedBy = LoggedInUserId;
            frequentlyAskedQuestionDto.UpdatedOn = DateTime.Now;

            _response = await _frequentlyAskedQuestionService.UpdateFrequentlyAskedQuestion(frequentlyAskedQuestionDto);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int frequentlyAskedQuestionId, bool isActive)
        {
            _response = await _frequentlyAskedQuestionService.UpdateIsActive(LoggedInUserId, frequentlyAskedQuestionId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveFrequentlyAskedQuestion(int frequentlyAskedQuestionId)
        {
            _response = await _frequentlyAskedQuestionService.RemoveFrequentlyAskedQuestion(frequentlyAskedQuestionId, LoggedInUserId);
            return _response;
        }

    }
}
