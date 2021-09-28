using ForLab.DTO.Common;

namespace ForLab.DTO.Product.Product
{
    public class ProductDto : BaseEntityDto
    {
        public int VendorId { get; set; }
        public decimal ManufacturerPrice { get; set; }
        public int ProductTypeId { get; set; }
        public string Name { get; set; }
        public string CatalogNo { get; set; }
        public int ProductBasicUnitId { get; set; }
        public decimal PackSize { get; set; }
        public bool Shared { get; set; }

        // UI
        public string VendorName { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductBasicUnitName { get; set; }
    }
    public class ProductDrp : DropdownDrp
    {
        public int ProductTypeId { get; set; }
    }
}
