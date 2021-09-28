using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.ProductBasicUnit;
using ForLab.Repositories.Lookup.ProductBasicUnit;
using ForLab.Services.Generics;

namespace ForLab.Services.Lookup.ProductBasicUnit
{
    public interface IProductBasicUnitService : IGService<ProductBasicUnitDto, Data.DbModels.LookupSchema.ProductBasicUnit, IProductBasicUnitRepository>
    {
        IResponseDTO GetProductBasicUnits();
    }
}
