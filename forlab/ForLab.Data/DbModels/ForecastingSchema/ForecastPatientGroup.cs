using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // morbidity methodology
    [Table("ForecastPatientGroups", Schema = "Forecasting")]
    public class ForecastPatientGroup : BaseEntity
    {
        public int ForecastInfoId { get; set; }
        public int PatientGroupId { get; set; }
        public int ProgramId { get; set; }
        public decimal Percentage { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual PatientGroup PatientGroup { get; set; }
        public virtual Program Program { get; set; }
    }
}
