using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.TestingSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // this is used in case of service
    [Table("ForecastTests", Schema = "Forecasting")]
    public class ForecastTest : BaseEntity
    {      
        public int ForecastInfoId { get; set; }
        public int TestId { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual Test Test { get; set; }
    }
}
