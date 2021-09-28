using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseSchema;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastMorbidityWhoBases", Schema = "Forecasting")]
    public class ForecastMorbidityWhoBase : BaseEntity
    {
        // if forecast morbidity who , bring only previous 5 years
        public int ForecastInfoId { get; set; }
        public int DiseaseId { get; set; }
        public int CountryId { get; set; }
        public string Period { get; set; }
        public decimal Count { get; set; }

        public virtual Disease Disease { get; set; }
        public virtual Country Country { get; set; }
        public virtual ForecastInfo ForecastInfo { get; set; }
    }
}
