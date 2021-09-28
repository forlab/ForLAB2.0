using ForLab.Core.Interfaces;
using ForLab.DTO.Configuration.ConfigurationAudit;
using ForLab.DTO.Configuration;
using ForLab.Repositories.Configuration.Configuration;
using ForLab.Services.Generics;
using System.Threading.Tasks;
using ForLab.Core.Common;

namespace ForLab.Services.Configuration
{
    public interface IConfigurationService : IGService<ConfigurationDto, Data.DbModels.ConfigurationSchema.Configuration, IConfigurationRepository>
    {
        Task<IResponseDTO> GetConfigurationDetails();
        Task<IResponseDTO> UpdateConfiguration(ConfigurationDto configurationDto, int loggedInUserId);
        Task<IResponseDTO> GetTimeToChangePassword();
        Task<IResponseDTO> TimeToSessionTimeOut();
        IResponseDTO GetAllConfigurationAudits(int? pageIndex = null, int? pageSize = null, ConfigurationAuditFilterDto filterDto = null);
        GeneratedFile ExportConfigurationAudits(int? pageIndex = null, int? pageSize = null, ConfigurationAuditFilterDto filterDto = null);
    }
}
