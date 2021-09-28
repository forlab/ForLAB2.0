namespace ForLab.DTO.Common
{
    public class LocationDto
    {
        public string IP { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string RegionCode { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string ContinentName { get; set; }
        public string ContinentCode { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string ASN { get; set; }
        public string Flag { get; set; }
        public string Postal { get; set; }
        public string CallingCode { get; set; }
        public bool RTL { get; set; }
    }
}
