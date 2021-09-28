using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastLaboratories", Schema = "Forecasting")]
    public class ForecastLaboratory : BaseEntity
    {
        // (if aggregate so forecast category should be set, if site by site then category will be null) 
        //so each forecast category assigned to one or more than laboratory 
        // so laboratory should be assigned to one group in the same forecast
        public ForecastLaboratory()
        {
            ForecastMorbidityTargetBasess = new HashSet<ForecastMorbidityTargetBase>();
        }
        public int ForecastInfoId { get; set; }
        public int? ForecastCategoryId { get; set; }
        public int LaboratoryId { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual ForecastCategory ForecastCategory { get; set; }
        public virtual Laboratory Laboratory { get; set; }
        public virtual ICollection<ForecastMorbidityTargetBase> ForecastMorbidityTargetBasess { get; set; } 
    }
}
