using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.FrequentlyAskedQuestion
{
    public class FrequentlyAskedQuestionFilterDto : BaseFilterDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
