using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.DiseaseProgram.TestingAssumptionParameter
{
    public class TestingAssumptionParameterRepository : GRepository<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter>, ITestingAssumptionParameterRepository
    {
        private readonly AppDbContext _appDbContext;
        public TestingAssumptionParameterRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
