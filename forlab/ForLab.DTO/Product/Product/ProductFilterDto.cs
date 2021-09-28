using ForLab.DTO.Common;


namespace ForLab.DTO.Product.Product
{
    public class ProductFilterDto : BaseFilterDto
    {
        public int VendorId { get; set; }
        public decimal? ManufacturerPrice { get; set; }
        public int ProductTypeId { get; set; }
        public string Name { get; set; }
        public string CatalogNo { get; set; }
        public int ProductBasicUnitId { get; set; }
        public string ProductTypeIds { get; set; }
    }
}
