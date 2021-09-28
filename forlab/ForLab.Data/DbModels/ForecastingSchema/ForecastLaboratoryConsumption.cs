using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastLaboratoryConsumptions", Schema = "Forecasting")]
    public class ForecastLaboratoryConsumption : BaseEntity
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual Laboratory Laboratory { get; set; }
        public virtual Product Product { get; set; }
    }
}
