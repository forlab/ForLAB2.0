using System;

namespace ForLab.DTO.Product.ProductUsage
{
    public class ExportTestUsageDto
    {
        public string ProductName { get; set; }
        public string InstrumentName { get; set; }
        public string TestName { get; set; }
        public bool IsForControl { get; set; } // incase of test usage and product type is QC
        public decimal Amount { get; set; }


        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
