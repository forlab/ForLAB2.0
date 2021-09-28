using ForLab.DTO.Common;

namespace ForLab.DTO.Vendor.Vendor
{
    public class VendorFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}
