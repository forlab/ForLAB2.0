using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.InquiryQuestionReply
{
    public class InquiryQuestionReplyRepository : GRepository<Data.DbModels.CMSSchema.InquiryQuestionReply>, IInquiryQuestionReplyRepository
    {
        private readonly AppDbContext _appDbContext;
        public InquiryQuestionReplyRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
