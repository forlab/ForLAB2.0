using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ProductSchema
{
    [Table("CountryProductPrices", Schema = "Product")]
    public class CountryProductPrice : BaseEntity
    {
        public int ProductId { get; set; }
        public int CountryId { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Country Country { get; set; }
    }
}
