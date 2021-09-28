using ForLab.DTO.Common;

namespace ForLab.DTO.Disease.Disease
{
    public class DiseaseDto : BaseEntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class DiseaseDrp : DropdownDrp
    {
    }
}
