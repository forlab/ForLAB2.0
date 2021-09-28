using ForLab.DTO.Common;

namespace ForLab.DTO.Product.ProductUsage
{
    public class ProductUsageFilterDto : BaseFilterDto
    {
        public int TestId { get; set; }
        public int ProductId { get; set; }
        public bool? IsForControl { get; set; }
        public decimal? Amount { get; set; }
        public bool? PerPeriod { get; set; }
        public bool? PerPeriodPerInstrument { get; set; }
        public int? CountryPeriodId { get; set; }
        public int? InstrumentId { get; set; }
        public string ProductTypeIds { get; set; }
        public bool ExportProductUsage { get; set; }
    }
}
