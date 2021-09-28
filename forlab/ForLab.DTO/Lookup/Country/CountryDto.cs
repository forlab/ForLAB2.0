using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.Country
{
    public class CountryDto : DynamicLookupDto
    {
        public int ContinentId { get; set; }
        public int CountryPeriodId { get; set; }
        public string ShortCode { get; set; }
        public string ShortName { get; set; }
        public string NativeName { get; set; }
        public string Flag { get; set; }
        public string CurrencyCode { get; set; }
        public string CallingCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public decimal Population { get; set; }

        // UI
        public string ContinentName { get; set; }
        public string CountryPeriodName { get; set; }
    }
    public class CountryDrp : DropdownDrp
    {
        public int CountryPeriodId { get; set; }
    }
}
