using System;

namespace ForLab.DTO.Product.ProductUsage
{
    public class ExportProductUsageDto
    {
        public string ProductName { get; set; }
        public bool PerPeriodPerInstrument { get; set; } // incase of product usage
        public string InstrumentName { get; set; }
        public bool PerPeriod { get; set; } // incase of product usage
        public string CountryPeriodName { get; set; }
        public decimal Amount { get; set; }

        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
