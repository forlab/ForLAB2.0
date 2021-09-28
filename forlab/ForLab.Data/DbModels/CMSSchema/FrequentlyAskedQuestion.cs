using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("FrequentlyAskedQuestions", Schema = "CMS")]
    public class FrequentlyAskedQuestion : BaseEntity
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
