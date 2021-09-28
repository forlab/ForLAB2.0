using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.TestingArea
{
    public class TestingAreaDto : DynamicLookupDto
    {
        public bool Shared { get; set; }
    }
    public class TestingAreaDrp : DropdownDrp
    {
    }
}