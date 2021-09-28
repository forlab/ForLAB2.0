using ForLab.DTO.Common;


namespace ForLab.DTO.Lookup.Country
{
    public class CountryFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public int ContinentId { get; set; }
        public int CountryPeriodId { get; set; }
        public string ShortCode { get; set; }
        public string NativeName { get; set; }
        public string CurrencyCode { get; set; }
        public string CallingCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public decimal? Population { get; set; }
    }
}
