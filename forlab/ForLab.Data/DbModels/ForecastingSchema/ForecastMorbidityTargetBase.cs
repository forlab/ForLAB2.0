using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastMorbidityTargetBases", Schema = "Forecasting")]
    public class ForecastMorbidityTargetBase : BaseEntity
    {
        // this should be used if morbidity is target based
        public int ForecastInfoId { get; set; }
        public int ForecastLaboratoryId { get; set; }
        public int ForecastMorbidityProgramId { get; set; }
        public decimal CurrentPatient { get; set; } 
        public decimal TargetPatient { get; set; }


        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual ForecastLaboratory ForecastLaboratory { get; set; }
        public virtual ForecastMorbidityProgram  ForecastMorbidityProgram { get; set; }
    }
}
