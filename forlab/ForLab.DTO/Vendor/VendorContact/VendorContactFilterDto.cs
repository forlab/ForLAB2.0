using ForLab.DTO.Common;

namespace ForLab.DTO.Vendor.VendorContact
{
    public class VendorContactFilterDto : BaseFilterDto
    {
        public int VendorId { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }
}
