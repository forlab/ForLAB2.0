using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.Region
{
  public  class RegionFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public string ShortName { get; set; }
    }
}
