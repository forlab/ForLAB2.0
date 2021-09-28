using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.ContactInfo
{
    public class ContactInfoDto : NullableBaseEntityDto
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        // Social Links
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
    }
}
