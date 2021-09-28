using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ScopeOfTheForecasts", Schema = "Lookup")]
    public class ScopeOfTheForecast : StaticLookup
    {
        // national, global, program based
        public ScopeOfTheForecast()
        {
            ForecastInfos = new HashSet<ForecastInfo>();
        }
        public virtual ICollection<ForecastInfo> ForecastInfos { get; set; }
    }
}
