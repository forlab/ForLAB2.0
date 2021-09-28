using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.VendorSchema
{
    [Table("Vendors", Schema = "Vendor")]
    public class Vendor : BaseEntity
    {
        public Vendor()
        {
            VendorContacts = new HashSet<VendorContact>();
            Products = new HashSet<Product>();
            Instruments = new HashSet<Instrument>();
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }

        public virtual ICollection<VendorContact> VendorContacts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Instrument> Instruments { get; set; }
    }
}
