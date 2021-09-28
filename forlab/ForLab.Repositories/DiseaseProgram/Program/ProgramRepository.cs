using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.DiseaseProgram.Program
{
    public class ProgramRepository : GRepository<Data.DbModels.DiseaseProgramSchema.Program>, IProgramRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProgramRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
