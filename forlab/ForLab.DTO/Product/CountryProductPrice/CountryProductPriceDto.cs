using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Product.CountryProductPrice
{
    public class CountryProductPriceDto : BaseEntityDto
    {
        public int ProductId { get; set; }
        public int CountryId { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }

        // UI
        public string ProductName { get; set; }
        public string CountryName { get; set; }
    }
    public class CountryProductPriceDrp : DropdownDrp
    {
        public string ProductName { get; set; }
        public string CountryName { get; set; }
    }
}
