using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("InquiryQuestionReplies", Schema = "CMS")]
    public class InquiryQuestionReply : BaseEntity
    {
        public int InquiryQuestionId { get; set; }
        public string Message { get; set; }
        public virtual InquiryQuestion InquiryQuestion { get; set; }
    }
}
