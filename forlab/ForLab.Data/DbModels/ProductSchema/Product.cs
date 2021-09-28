using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LaboratorySchema;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.VendorSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ProductSchema
{
    [Table("Products", Schema = "Product")]
    public class Product: BaseEntity
    {
        public Product()
        {
            CountryProductPrices = new HashSet<CountryProductPrice>();
            ProductUsages = new HashSet<ProductUsage>();
            LaboratoryConsumptions = new HashSet<LaboratoryConsumption>();
            RegionProductPrices = new HashSet<RegionProductPrice>();
            LaboratoryProductPrices = new HashSet<LaboratoryProductPrice>();
            ForecastLaboratoryConsumptions = new HashSet<ForecastLaboratoryConsumption>();
            ForecastResults = new HashSet<ForecastResult>();
        }
        public int VendorId { get; set; }
        public decimal ManufacturerPrice { get; set; }
        public int ProductTypeId { get; set; }
        public string Name { get; set; }
        public string CatalogNo { get; set; }
        public int ProductBasicUnitId { get; set; }
        public decimal PackSize { get; set; }
        public bool Shared { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual ProductType ProductType { get; set; }
        public ProductBasicUnit ProductBasicUnit { get; set; }
        public virtual ICollection<CountryProductPrice> CountryProductPrices { get; set; }
        public virtual ICollection<RegionProductPrice> RegionProductPrices { get; set; }
        public virtual ICollection<LaboratoryProductPrice> LaboratoryProductPrices { get; set; }
        public virtual ICollection<ProductUsage> ProductUsages { get; set; }
        public virtual ICollection<LaboratoryConsumption> LaboratoryConsumptions { get; set; }
        public virtual ICollection<ForecastLaboratoryConsumption> ForecastLaboratoryConsumptions { get; set; }
        public virtual ICollection<ForecastResult> ForecastResults { get; set; }
    }
}
