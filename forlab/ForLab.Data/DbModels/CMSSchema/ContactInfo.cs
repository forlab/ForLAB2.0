using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("ContactInfos", Schema = "CMS")]
    public class ContactInfo : NullableBaseEntity
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
