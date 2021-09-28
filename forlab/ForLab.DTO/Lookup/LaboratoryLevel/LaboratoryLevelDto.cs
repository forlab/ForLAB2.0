using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.LaboratoryLevel
{
    public  class LaboratoryLevelDto: DynamicLookupDto
    {
        public bool Shared { get; set; }
    }
    public class LaboratoryLevelDrp : DropdownDrp
    {
    }
}
