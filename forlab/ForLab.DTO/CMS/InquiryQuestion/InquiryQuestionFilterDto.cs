using ForLab.DTO.Common;
namespace ForLab.DTO.CMS.InquiryQuestion
{
   public class InquiryQuestionFilterDto:BaseFilterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool? ReplyProvided { get; set; }
    }
}
