using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.Repositories.DiseaseProgram.ProgramTest;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.ProgramTest
{
    public interface IProgramTestService : IGService<ProgramTestDto, Data.DbModels.DiseaseProgramSchema.ProgramTest, IProgramTestRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProgramTestFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ProgramTestFilterDto filterDto = null);
        Task<IResponseDTO> GetProgramTestDetails(int programTestId);
        Task<IResponseDTO> CreateProgramTest(ProgramTestDto programTestDto);
        Task<IResponseDTO> UpdateProgramTest(ProgramTestDto programTestDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int programTestId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveProgramTest(int programTestId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportProgramTests(int? pageIndex = null, int? pageSize = null, ProgramTestFilterDto filterDto = null);
        Task<IResponseDTO> ImportProgramTests(List<ProgramTestDto> programTestDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsProgramTestPatientGroupUnique(ProgramTestDto programTestDto);
        bool IsProgramTestTestingProtocolUnique(ProgramTestDto programTestDto);
    }
}
