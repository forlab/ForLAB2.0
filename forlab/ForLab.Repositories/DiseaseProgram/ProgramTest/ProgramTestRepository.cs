using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.DiseaseProgram.ProgramTest
{
    public class ProgramTestRepository : GRepository<Data.DbModels.DiseaseProgramSchema.ProgramTest>, IProgramTestRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProgramTestRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
