using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Configuration.ConfigurationAudit
{
    public class ConfigurationAuditRepository : GRepository<Data.DbModels.ConfigurationSchema.ConfigurationAudit>, IConfigurationAuditRepository
    {
        private readonly AppDbContext _appDbContext;
        public ConfigurationAuditRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
