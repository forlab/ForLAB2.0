using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.InquiryQuestionReply
{
    public class InquiryQuestionReplyFilterDto:BaseFilterDto
    {
        public int InquiryQuestionId { get; set; }
        public string Message { get; set; }

    }
}
