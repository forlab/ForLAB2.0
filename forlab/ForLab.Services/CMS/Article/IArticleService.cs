using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.Article;
using ForLab.Repositories.CMS.Article;
using ForLab.Services.Generics;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.Article
{
    public interface IArticleService : IGService<ArticleDto, Data.DbModels.CMSSchema.Article, IArticleRepository>
    {
        IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, ArticleFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ArticleFilterDto filterDto = null);
        Task<IResponseDTO> GetArticleDetails(string rootPath, int articleId);
        Task<IResponseDTO> CreateArticle(ArticleDto articleDto, List<IFormFile> files = null);
        Task<IResponseDTO> UpdateArticle(ArticleDto articleDto, List<IFormFile> files = null);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int articleId, bool IsActive);
        Task<IResponseDTO> RemoveArticle(int articleId, int loggedInUserId);

        // Validators methods
        bool IsTitleUnique(ArticleDto articleDto);
    }
}
