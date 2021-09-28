using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.FrequentlyAskedQuestion
{
    public class FrequentlyAskedQuestionDto : BaseEntityDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class FrequentlyAskedQuestionDrp : DropdownDrp
    {
        public string Question { get; set; }
    }
}
