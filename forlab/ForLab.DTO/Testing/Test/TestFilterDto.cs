using ForLab.DTO.Common;

namespace ForLab.DTO.Testing.Test
{
    public class TestFilterDto : BaseFilterDto
    {
        public int TestingAreaId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
