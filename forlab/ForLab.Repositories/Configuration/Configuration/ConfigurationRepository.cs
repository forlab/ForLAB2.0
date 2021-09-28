using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Configuration.Configuration
{
    public class ConfigurationRepository : GRepository<Data.DbModels.ConfigurationSchema.Configuration>, IConfigurationRepository
    {

        private readonly AppDbContext _appDbContext;
        public ConfigurationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
