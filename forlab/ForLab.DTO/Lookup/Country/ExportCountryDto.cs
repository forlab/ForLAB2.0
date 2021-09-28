
using System;

namespace ForLab.DTO.Lookup.Country
{
    public class ExportCountryDto
    {
        public string ContinentName { get; set; }
        public string Name { get; set; }
        public string CountryPeriodName { get; set; }
        public string ShortCode { get; set; }
        public string ShortName { get; set; }
        public string NativeName { get; set; }
        public string CurrencyCode { get; set; }
        public string CallingCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public decimal Population { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
