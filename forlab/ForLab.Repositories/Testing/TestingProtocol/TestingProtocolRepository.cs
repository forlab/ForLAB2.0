using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Testing.TestingProtocol
{
    public class TestingProtocolRepository : GRepository<Data.DbModels.TestingSchema.TestingProtocol>, ITestingProtocolRepository
    {
        private readonly AppDbContext _appDbContext;
        public TestingProtocolRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
