using ForLab.DTO.CMS.InquiryQuestionReply;
using ForLab.DTO.Common;
using System.Collections.Generic;

namespace ForLab.DTO.CMS.InquiryQuestion
{
    public class InquiryQuestionDto : NullableBaseEntityDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool ReplyProvided { get; set; }
        public List<InquiryQuestionReplyDto> InquiryQuestionReplyDtos { get; set; }
    }
    public class InquiryQuestionDrp : DropdownDrp
    {
    }
}
