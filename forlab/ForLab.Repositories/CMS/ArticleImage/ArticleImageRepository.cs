using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.ArticleImage
{
    public class ArticleImageRepository : GRepository<Data.DbModels.CMSSchema.ArticleImage>, IArticleImageRepository
    {
        private readonly AppDbContext _appDbContext;
        public ArticleImageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
