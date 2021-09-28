using System;

namespace ForLab.DTO.Vendor.VendorContact
{
    public class ExportVendorContactDto
    {
        public string VendorName { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
