using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.DiseaseProgram.PatientAssumptionParameter
{
    public class PatientAssumptionParameterRepository : GRepository<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter>, IPatientAssumptionParameterRepository
    {
        private readonly AppDbContext _appDbContext;
        public PatientAssumptionParameterRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}