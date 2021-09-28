using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // morbidity only
    [Table("ForecastTestingAssumptionValues", Schema = "Forecasting")]
    public class ForecastTestingAssumptionValue : BaseEntity
    {
        public int TestingAssumptionParameterId { get; set; }
        public int ForecastInfoId { get; set; }
        public decimal? Value { get; set; }
        public virtual TestingAssumptionParameter TestingAssumptionParameter { get; set; }
        public virtual ForecastInfo ForecastInfo { get; set; }
    }
}
