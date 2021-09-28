using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.TestingSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastLaboratoryTestServices", Schema = "Forecasting")]
    public class ForecastLaboratoryTestService : BaseEntity
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; } // which will be fetched from the machine learning model

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual Laboratory Laboratory { get; set; }
        public virtual Test Test { get; set; }
    }
}
