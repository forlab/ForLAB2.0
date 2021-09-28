using ForLab.Data.BaseModeling;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("InquiryQuestions", Schema = "CMS")]
    public class InquiryQuestion : NullableBaseEntity
    {
        public InquiryQuestion()
        {
            InquiryQuestionReplies = new HashSet<InquiryQuestionReply>();
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool ReplyProvided { get; set; }

        public virtual ICollection<InquiryQuestionReply> InquiryQuestionReplies { get; set; } 
    }
}
