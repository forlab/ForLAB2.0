using ForLab.DTO.Common;

namespace ForLab.DTO.Testing.Test
{
    public class TestDto : BaseEntityDto
    {
        public int TestingAreaId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Shared { get; set; }

        // UI
        public string TestingAreaName { get; set; }
    }
    public class TestDrp : DropdownDrp
    {
    }
}
