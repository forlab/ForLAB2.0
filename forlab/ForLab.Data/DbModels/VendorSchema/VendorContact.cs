using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.VendorSchema
{
    [Table("VendorContacts", Schema = "Vendor")]
    public class VendorContact : BaseEntity
    {
        public int VendorId { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        public virtual Vendor Vendor { get; set; }
    }
}
