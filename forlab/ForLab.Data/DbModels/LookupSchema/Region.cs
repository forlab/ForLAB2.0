using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.SecuritySchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("Regions", Schema = "Lookup")]
    public class Region : DynamicLookup
    {
        public Region()
        {
            Laboratories = new HashSet<Laboratory>();
            RegionProductPrices = new HashSet<RegionProductPrice>();
            UserRegionSubscriptions = new HashSet<UserRegionSubscription>();
        }
        public int CountryId { get; set; }
        public string ShortName { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Laboratory> Laboratories { get; set; }
        public virtual ICollection<RegionProductPrice> RegionProductPrices { get; set; }
        public virtual ICollection<UserRegionSubscription> UserRegionSubscriptions { get; set; }
    }
}
