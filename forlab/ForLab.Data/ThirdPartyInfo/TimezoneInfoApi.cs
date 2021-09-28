using System.Collections.Generic;

namespace ForLab.Data.ThirdPartyInfo
{
    public class TimezoneInfoApi
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<TimezoneInfoApi_Zones> Zones { get; set; }
    }

    public class TimezoneInfoApi_Zones
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string ZoneName { get; set; }
        public float GmtOffset { get; set; }
        public float Timestamp { get; set; }
    }

}
