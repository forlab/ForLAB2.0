using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ProductSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // this is used in case of service or morbidity
    [Table("ForecastInstruments", Schema = "Forecasting")]
    public class ForecastInstrument : BaseEntity
    {
        public int InstrumentId { get; set; }
        public int ForecastInfoId { get; set; }

        public virtual Instrument Instrument { get; set; }
        public virtual ForecastInfo ForecastInfo { get; set; }
    }
}
