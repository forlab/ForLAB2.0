using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // morbidity only
    [Table("ForecastPatientAssumptionValues", Schema = "Forecasting")]
    public class ForecastPatientAssumptionValue : BaseEntity
    {
        public int ForecastInfoId { get; set; }
        public int PatientAssumptionParameterId { get; set; }
        public decimal? Value { get; set; }
        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual PatientAssumptionParameter PatientAssumptionParameter { get; set; }
    }
}
