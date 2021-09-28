using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Testing.TestingProtocol;
using ForLab.Repositories.Testing.TestingProtocol;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.Testing.TestingProtocol
{
    public interface ITestingProtocolService : IGService<TestingProtocolDto, Data.DbModels.TestingSchema.TestingProtocol, ITestingProtocolRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestingProtocolFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(TestingProtocolFilterDto filterDto = null);
        Task<IResponseDTO> GetTestingProtocolDetails(int testingProtocolId);
        GeneratedFile ExportTestingProtocols(int? pageIndex = null, int? pageSize = null, TestingProtocolFilterDto filterDto = null);
    }
}
