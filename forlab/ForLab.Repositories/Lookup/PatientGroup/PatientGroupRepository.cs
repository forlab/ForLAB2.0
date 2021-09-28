using ForLab.Data.DataContext;
using ForLab.Repositories.Generics;

namespace ForLab.Repositories.Lookup.PatientGroup
{
    public class PatientGroupRepository : GRepository<Data.DbModels.LookupSchema.PatientGroup>, IPatientGroupRepository
    {
        private readonly AppDbContext _appDbContext;
        public PatientGroupRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
