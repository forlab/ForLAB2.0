using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.InquiryQuestion
{
    public class InquiryQuestionRepository : GRepository<Data.DbModels.CMSSchema.InquiryQuestion>, IInquiryQuestionRepository
    {
        private readonly AppDbContext _appDbContext;
        public InquiryQuestionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
