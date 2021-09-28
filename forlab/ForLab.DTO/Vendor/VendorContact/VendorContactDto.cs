using ForLab.DTO.Common;

namespace ForLab.DTO.Vendor.VendorContact
{
    public class VendorContactDto : BaseEntityDto
    {
        public int VendorId { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        //UI
        public string VendorName { get; set; }
    }
    public class VendorContactDrp : DropdownDrp
    {
    }
}
