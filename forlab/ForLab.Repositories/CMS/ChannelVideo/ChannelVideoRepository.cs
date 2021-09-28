using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.CMS.ChannelVideo
{
    public class ChannelVideoRepository : GRepository<Data.DbModels.CMSSchema.ChannelVideo>, IChannelVideoRepository
    {
        private readonly AppDbContext _appDbContext;
        public ChannelVideoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
