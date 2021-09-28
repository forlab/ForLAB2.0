using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.PatientAssumptionParameter
{
    public interface IPatientAssumptionParameterService : IGService<PatientAssumptionParameterDto, Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter, IPatientAssumptionParameterRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, PatientAssumptionParameterFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(PatientAssumptionParameterFilterDto filterDto = null);
        IResponseDTO GetAllPatientAssumptionsForForcast(string programIds);
        Task<IResponseDTO> GetPatientAssumptionParameterDetails(int patientAssumptionParameterId);
        Task<IResponseDTO> CreatePatientAssumptionParameter(PatientAssumptionParameterDto patientAssumptionParameterDto);
        Task<IResponseDTO> UpdatePatientAssumptionParameter(PatientAssumptionParameterDto patientAssumptionParameterDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int patientAssumptionParameterId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemovePatientAssumptionParameter(int patientAssumptionParameterId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportPatientAssumptionParameters(int? pageIndex = null, int? pageSize = null, PatientAssumptionParameterFilterDto filterDto = null);
        Task<IResponseDTO> ImportPatientAssumptionParameters(List<PatientAssumptionParameterDto> patientAssumptionParameterDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsNameUnique(PatientAssumptionParameterDto patientAssumptionParameterDto);
        Task<IResponseDTO> IsUsed(int patientAssumptionParameterDto);
    }
}
