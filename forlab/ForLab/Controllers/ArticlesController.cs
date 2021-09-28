using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForLab.API.Helpers;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.Article;
using ForLab.Services.CMS.Article;
using ForLab.Validators.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ArticlesController : BaseController
    {
        private readonly IArticleService _articleService;

        public ArticlesController(
            IArticleService articleService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _articleService = articleService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ArticleFilterDto filterDto)
        {
            _response = _articleService.GetAll(ServerRootPath, pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpGet]
        public IResponseDTO GetAllAsDrp([FromQuery] ArticleFilterDto filterDto)
        {
            _response = _articleService.GetAllAsDrp(filterDto);
            return _response;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseDTO> GetArticleDetails(int articleId)
        {
            _response = await _articleService.GetArticleDetails(ServerRootPath, articleId);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IResponseDTO> CreateArticle([ModelBinder(BinderType = typeof(JsonModelBinder))] ArticleDto articleDto)
        {
            //Validation
            var validationResult = await (new ArticleValidator(_articleService)).ValidateAsync(articleDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            articleDto.Creator = null;
            articleDto.Updator = null;
            articleDto.CreatedBy = LoggedInUserId;
            articleDto.CreatedOn = DateTime.Now;
            articleDto.ArticleImageDtos?.Select(x => { x.Creator = null; x.CreatedBy = LoggedInUserId; x.CreatedOn = DateTime.Now; return x; }).ToList();

            var files = Request?.Form?.Files?.Count() > 0 ? Request?.Form?.Files?.ToList() : null;
            _response = await _articleService.CreateArticle(articleDto, files);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateArticle([ModelBinder(BinderType = typeof(JsonModelBinder))] ArticleDto articleDto)
        {
            // Validation
            var validationResult = await (new ArticleValidator(_articleService)).ValidateAsync(articleDto);
            if (!validationResult.IsValid)
            {
                _response.IsPassed = false;
                _response.Message = string.Join(",\n\r", validationResult.Errors.Select(e => e.ErrorMessage).Distinct());
                _response.Data = null;
                return _response;
            }

            // Set variables by the system
            articleDto.Creator = null;
            articleDto.Updator = null;
            articleDto.UpdatedBy = LoggedInUserId;
            articleDto.UpdatedOn = DateTime.Now;
            articleDto.ArticleImageDtos?
                .Where(x => x.Id > 0 && x.IsDeleted)
                .Select(x => { x.Creator = null; x.UpdatedBy = LoggedInUserId; x.UpdatedOn = DateTime.Now; return x; }).ToList();
            articleDto.ArticleImageDtos?
                  .Where(x => x.Id == 0)
                .Select(x => { x.Creator = null; x.CreatedBy = LoggedInUserId; x.CreatedOn = DateTime.Now; return x; }).ToList();

            var files = Request?.Form?.Files?.Count() > 0 ? Request?.Form?.Files?.ToList() : null;
            _response = await _articleService.UpdateArticle(articleDto, files);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IResponseDTO> UpdateIsActive(int articleId, bool isActive)
        {
            _response = await _articleService.UpdateIsActive(LoggedInUserId, articleId, isActive);
            return _response;
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<IResponseDTO> RemoveArticle(int articleId)
        {
            _response = await _articleService.RemoveArticle(articleId, LoggedInUserId);
            return _response;
        }
    }
}
