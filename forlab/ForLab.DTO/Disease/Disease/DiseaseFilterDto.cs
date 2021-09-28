using ForLab.DTO.Common;

namespace ForLab.DTO.Disease.Disease
{
    public class DiseaseFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
