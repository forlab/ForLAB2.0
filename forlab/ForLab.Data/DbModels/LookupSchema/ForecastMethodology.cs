using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("ForecastMethodologies", Schema = "Lookup")]
    public class ForecastMethodology : StaticLookup
    {
        //service, consumption, dempgraphic morbidity
        public ForecastMethodology()
        {
            ForecastInfos = new HashSet<ForecastInfo>();
        }

        public virtual ICollection<ForecastInfo> ForecastInfos { get; set; }
    }
}
