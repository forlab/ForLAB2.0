using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.ProductAssumptionParameter
{
    public interface IProductAssumptionParameterService : IGService<ProductAssumptionParameterDto, Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter, IProductAssumptionParameterRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProductAssumptionParameterFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ProductAssumptionParameterFilterDto filterDto = null);
        IResponseDTO GetAllProductAssumptionsForForcast(string programIds);
        Task<IResponseDTO> GetProductAssumptionParameterDetails(int productAssumptionParameterId);
        Task<IResponseDTO> CreateProductAssumptionParameter(ProductAssumptionParameterDto productAssumptionParameterDto);
        Task<IResponseDTO> UpdateProductAssumptionParameter(ProductAssumptionParameterDto productAssumptionParameterDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int productAssumptionParameterId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveProductAssumptionParameter(int productAssumptionParameterId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportProductAssumptionParameters(int? pageIndex = null, int? pageSize = null, ProductAssumptionParameterFilterDto filterDto = null);
        Task<IResponseDTO> ImportProductAssumptionParameters(List<ProductAssumptionParameterDto> productAssumptionParameterDtos, int LoggedInUserId, bool IsSuperAdmin);
        // Validators methods
        bool IsNameUnique(ProductAssumptionParameterDto productAssumptionParameterDto);
        Task<IResponseDTO> IsUsed(int productAssumptionParameterDto);
    }
}
