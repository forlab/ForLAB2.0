using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastMorbidityPrograms", Schema = "Forecasting")]
    public class ForecastMorbidityProgram : BaseEntity
    {
        public ForecastMorbidityProgram()
        {
            ForecastMorbidityTargetBases = new HashSet<ForecastMorbidityTargetBase>();
        }
        public int ForecastInfoId { get; set; }
        public int ProgramId { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual Program Program { get; set; }
        public virtual ICollection<ForecastMorbidityTargetBase> ForecastMorbidityTargetBases { get; set; }
    }
}
