using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.ContactInfo
{
    public class ContactInfoRepository : GRepository<Data.DbModels.CMSSchema.ContactInfo>, IContactInfoRepository
    {
        private readonly AppDbContext _appDbContext;
        public ContactInfoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
