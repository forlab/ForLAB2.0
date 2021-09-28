using ForLab.Data.BaseModeling;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastCategories", Schema = "Forecasting")]
    public class ForecastCategory : BaseEntity
    {
        // only using this table if the upload is using aggregate feature
        public ForecastCategory()
        {
            ForecastLaboratories = new HashSet<ForecastLaboratory>();
        }
        public int ForecastInfoId { get; set; }
        public string Name { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual ICollection<ForecastLaboratory> ForecastLaboratories { get; set; }
    }
}
