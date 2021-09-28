using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.ProductSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ProductTypes", Schema = "Lookup")]
    public class ProductType: StaticLookup
    {
        // reagents, quality control, consumables, durables, calibrators for now , there is no add screen for now
        public ProductType()
        {
            Products = new HashSet<Product>();
            ForecastResults = new HashSet<ForecastResult>();
        }
       

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ForecastResult> ForecastResults { get; set; }
    }
}
