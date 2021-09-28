using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.Feature
{
    public class FeatureDto : NullableBaseEntityDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string LogoPath { get; set; }
    }
}
