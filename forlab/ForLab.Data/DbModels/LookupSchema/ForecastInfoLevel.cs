using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ForecastInfoLevels", Schema = "Lookup")]
    public class ForecastInfoLevel : StaticLookup
    {
        // country, region, laboratory
        public ForecastInfoLevel()
        {
            ForecastInfos = new HashSet<ForecastInfo>();
        }
        public virtual ICollection<ForecastInfo> ForecastInfos { get; set; }
    }
}
