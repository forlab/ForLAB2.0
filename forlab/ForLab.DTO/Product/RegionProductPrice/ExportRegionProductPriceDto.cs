using System;

namespace ForLab.DTO.Product.RegionProductPrice
{
    public class ExportRegionProductPriceDto
    {
        public string RegionCountryName { get; set; }
        public string RegionName { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
