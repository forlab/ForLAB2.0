
using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.Program;
using ForLab.Repositories.DiseaseProgram.Program;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.Program
{
    public interface IProgramService : IGService<ProgramDto, Data.DbModels.DiseaseProgramSchema.Program, IProgramRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProgramFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ProgramFilterDto filterDto = null);
        Task<IResponseDTO> GetProgramDetailsForForcast(int programId);
        Task<IResponseDTO> GetProgramDetails(int programId);
        Task<IResponseDTO> CreateProgram(ProgramDto programDto);
        Task<IResponseDTO> UpdateProgram(ProgramDto programDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int programId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveProgram(int programId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportPrograms(int? pageIndex = null, int? pageSize = null, ProgramFilterDto filterDto = null);
        Task<IResponseDTO> ImportPrograms(List<ProgramDto> programDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsNameUnique(ProgramDto programDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int programDto);
    }
}