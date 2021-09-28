using System;

namespace ForLab.DTO.Product.Product
{
    public class ExportProductDto
    {
        public string VendorName { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductBasicUnitName { get; set; }
        public decimal ManufacturerPrice { get; set; }
        public string Name { get; set; }
        public string CatalogNo { get; set; }
        public decimal PackSize { get; set; }

        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
