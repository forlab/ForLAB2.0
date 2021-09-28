using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.InquiryQuestionReply
{
    public  class InquiryQuestionReplyDto : BaseEntityDto
    {
        public int InquiryQuestionId { get; set; }
        public string Message { get; set; }

        //UI
        public string InquiryQuestionName { get; set; }
    }
    public class InquiryQuestionReplyDrp : DropdownDrp
    {
    }
}
