using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.Region
{
    public class RegionDto : DynamicLookupDto
    {
        public int CountryId { get; set; }
        public string ShortName { get; set; }

        // UI
        public string CountryName { get; set; }
        public string CountryFlag { get; set; }
    }

    public class RegionDrp : DropdownDrp
    {
        public int CountryId { get; set; }
    }
}