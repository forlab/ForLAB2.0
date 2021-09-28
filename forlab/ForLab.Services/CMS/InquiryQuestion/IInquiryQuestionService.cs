using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.InquiryQuestion;
using ForLab.DTO.CMS.InquiryQuestionReply;
using ForLab.Repositories.CMS.InquiryQuestion;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.InquiryQuestion
{
    public interface IInquiryQuestionService : IGService<InquiryQuestionDto, Data.DbModels.CMSSchema.InquiryQuestion, IInquiryQuestionRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, InquiryQuestionFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(InquiryQuestionFilterDto filterDto = null);
        Task<IResponseDTO> GetInquiryQuestionDetails(int inquiryQuestionId);
        Task<IResponseDTO> CreateInquiryQuestion(InquiryQuestionDto inquiryQuestionDto);
        Task<IResponseDTO> UpdateInquiryQuestion(InquiryQuestionDto inquiryQuestionDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int inquiryQuestionId, bool IsActive);
        Task<IResponseDTO> RemoveInquiryQuestion(int inquiryQuestionId, int loggedInUserId);
        Task<IResponseDTO> CreateInquiryQuestionReply(InquiryQuestionReplyDto inquiryQuestionReplyDto);
    }
}
