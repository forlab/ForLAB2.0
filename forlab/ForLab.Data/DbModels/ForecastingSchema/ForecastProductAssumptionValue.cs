using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // morbidity only
    [Table("ForecastProductAssumptionValues", Schema = "Forecasting")]
    public class ForecastProductAssumptionValue : BaseEntity
    {
        public int ProductAssumptionParameterId { get; set; }
        public int ForecastInfoId { get; set; }
        public decimal? Value { get; set; }
        public virtual ProductAssumptionParameter ProductAssumptionParameter { get; set; }
        public virtual ForecastInfo ForecastInfo { get; set; }
    }
}
