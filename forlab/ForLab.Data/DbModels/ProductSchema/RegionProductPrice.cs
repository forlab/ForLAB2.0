using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ProductSchema
{
    [Table("RegionProductPrices", Schema = "Product")]
    public class RegionProductPrice : BaseEntity
    {
        public int ProductId { get; set; }
        public int RegionId { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Region Region { get; set; }
    }
}
