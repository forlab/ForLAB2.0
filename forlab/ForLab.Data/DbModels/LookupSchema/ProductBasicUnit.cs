using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ProductBasicUnits", Schema = "Lookup")]
    public class ProductBasicUnit : StaticLookup
    {
        //gm, cm…etc
        public ProductBasicUnit()
        {
            Products = new HashSet<Product>();
        }

        public virtual ICollection<Product> Products { get; set; }

    }
}
