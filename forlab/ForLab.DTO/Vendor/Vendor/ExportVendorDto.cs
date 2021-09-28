using System;

namespace ForLab.DTO.Vendor.Vendor
{
    public class ExportVendorDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
