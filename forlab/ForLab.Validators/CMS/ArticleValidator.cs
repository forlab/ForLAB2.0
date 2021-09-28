using FluentValidation;
using ForLab.DTO.CMS.Article;
using ForLab.DTO.CMS.ArticleImage;
using ForLab.Services.CMS.Article;
using System.Collections.Generic;
using System.Linq;

namespace ForLab.Validators.CMS
{
    public class ArticleValidator : AbstractValidator<ArticleDto>
    {
        readonly IArticleService _articleService;
        public ArticleValidator(IArticleService articleService)
        {
            _articleService = articleService;

            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueTitle).WithMessage("The title is already exist please try a new one");
            RuleFor(x => x.Content)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ProvidedDate)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ProvidedBy)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ArticleImageDtos)
                .NotEmpty()
                .NotNull()
                .Must(BeLimitedImages).WithMessage("The number of article images must not exceed 10");
        }

        private bool BeUniqueTitle(ArticleDto articleDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _articleService.IsTitleUnique(articleDto);
        }
        private bool BeLimitedImages(ArticleDto articleDto, List<ArticleImageDto> articleImageDtos)
        {
            if(articleImageDtos.Where(x => !x.IsDeleted).Count() > 10)
            {
                return false;
            }
            return true;
        }
    }
}
