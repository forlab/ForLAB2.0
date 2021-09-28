using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Product.LaboratoryProductPrice
{
    public class LaboratoryProductPriceDto : BaseEntityDto
    {
        public int ProductId { get; set; }
        public int LaboratoryId { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }

        // UI
        public int? LaboratoryRegionCountryId { get; set; }
        public int? LaboratoryRegionId { get; set; }
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string ProductName { get; set; }
        public string LaboratoryName { get; set; }
    }
    public class LaboratoryProductPriceDrp : DropdownDrp
    {
        public string ProductName { get; set; }
        public string LaboratoryName { get; set; }
    }
}
