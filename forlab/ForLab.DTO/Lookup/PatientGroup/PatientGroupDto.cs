using ForLab.DTO.Common;
namespace ForLab.DTO.Lookup.PatientGroup
{
    public class PatientGroupDto: DynamicLookupDto
    {
        public bool Shared { get; set; }
    }
    public class PatientGroupDrp : DropdownDrp
    {
    }
}
