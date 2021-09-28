using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.FrequentlyAskedQuestion;
using ForLab.Repositories.CMS.FrequentlyAskedQuestion;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.FrequentlyAskedQuestion
{
    public interface IFrequentlyAskedQuestionService : IGService<FrequentlyAskedQuestionDto, Data.DbModels.CMSSchema.FrequentlyAskedQuestion, IFrequentlyAskedQuestionRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, FrequentlyAskedQuestionFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(FrequentlyAskedQuestionFilterDto filterDto = null);
        Task<IResponseDTO> GetFrequentlyAskedQuestionDetails(int frequentlyAskedQuestionId);
        Task<IResponseDTO> CreateFrequentlyAskedQuestion(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto);
        Task<IResponseDTO> UpdateFrequentlyAskedQuestion(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int frequentlyAskedQuestionId, bool IsActive);
        Task<IResponseDTO> RemoveFrequentlyAskedQuestion(int frequentlyAskedQuestionId, int loggedInUserId);
        // Validators methods
        bool IsQuestionUnique(FrequentlyAskedQuestionDto frequentlyAskedQuestionDto);
    }
}
