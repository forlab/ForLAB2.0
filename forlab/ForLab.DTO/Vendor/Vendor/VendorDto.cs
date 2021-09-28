using ForLab.DTO.Common;
using ForLab.DTO.Vendor.VendorContact;
using System.Collections.Generic;

namespace ForLab.DTO.Vendor.Vendor
{
    public class VendorDto : BaseEntityDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }

        public List<VendorContactDto> VendorContactDtos { get; set; }
    }
    public class VendorDrp : DropdownDrp
    {
    }
}
