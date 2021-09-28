using ForLab.DTO.Common;
namespace ForLab.DTO.Lookup.LaboratoryCategory
{
    public class LaboratoryCategoryDto:DynamicLookupDto
    {
        public bool Shared { get; set; }
    }
    public class LaboratoryCategoryDrp : DropdownDrp
    {
    }
}
