using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.FrequentlyAskedQuestion
{
    public class FrequentlyAskedQuestionRepository : GRepository<Data.DbModels.CMSSchema.FrequentlyAskedQuestion>, IFrequentlyAskedQuestionRepository
    {
        private readonly AppDbContext _appDbContext;
        public FrequentlyAskedQuestionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
