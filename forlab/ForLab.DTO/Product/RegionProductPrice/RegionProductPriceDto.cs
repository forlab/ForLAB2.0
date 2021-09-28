using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Product.RegionProductPrice
{
    public class RegionProductPriceDto : BaseEntityDto
    {
        public int ProductId { get; set; }
        public int RegionId { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }

        // UI
        public int? RegionCountryId { get; set; }
        public string RegionCountryName { get; set; }
        public string RegionName { get; set; }
        public string ProductName { get; set; }
    }
    public class RegionProductPriceDrp : DropdownDrp
    {
        public string RegionName { get; set; }
        public string ProductName { get; set; }
    }
}
