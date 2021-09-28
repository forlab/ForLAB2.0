using ForLab.DTO.Common;

namespace ForLab.DTO.Product.ProductUsage
{
    public class ProductUsageDto : BaseEntityDto
    {   
        public int? TestId { get; set; } // will be null in case (consumables, calibarators)
        public int ProductId { get; set; }
        public bool IsForControl { get; set; } // incase of test usage and product type is QC
        public decimal Amount { get; set; }
        public bool PerPeriod { get; set; } // incase of product usage
        public bool PerPeriodPerInstrument { get; set; } // incase of product usage
        public int? CountryPeriodId { get; set; } // incase of product usage, it should be active if per period
        public int? InstrumentId { get; set; } //  it should be active if PerPeriodPerInstrument

        // UI
        public string ProductName { get; set; }
        public string InstrumentName { get; set; }
        public string TestName { get; set; }
        public string CountryPeriodName { get; set; }
    }
    public class ProductUsageDrp : DropdownDrp
    {
        public string ProductName { get; set; }
        public string InstrumentName { get; set; }
    }
}
