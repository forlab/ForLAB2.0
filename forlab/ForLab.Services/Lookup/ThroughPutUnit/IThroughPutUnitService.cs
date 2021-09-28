using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.ThroughPutUnit;
using ForLab.Repositories.Lookup.ThroughPutUnit;
using ForLab.Services.Generics;

namespace ForLab.Services.Lookup.ThroughPutUnit
{
    public interface IThroughPutUnitService : IGService<ThroughPutUnitDto, Data.DbModels.LookupSchema.ThroughPutUnit, IThroughPutUnitRepository>
    {
        IResponseDTO GetThroughPutUnits();
    }
}
