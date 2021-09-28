using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.DiseaseSchema
{
    // this object will have no create screen , it will be fetched from global api
    [Table("Diseases", Schema = "Disease")]
    public class Disease :  BaseEntity
    {
        public Disease()
        {
            CountryDiseaseIncidents = new HashSet<CountryDiseaseIncident>();
            DiseaseTestingProtocols = new HashSet<DiseaseTestingProtocol>();
            Programs = new HashSet<Program>();
            ForecastMorbidityWhoBases = new HashSet<ForecastMorbidityWhoBase>();
        }
        public string Description { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CountryDiseaseIncident> CountryDiseaseIncidents { get; set; }
        public virtual ICollection<DiseaseTestingProtocol> DiseaseTestingProtocols { get; set; }
        public virtual ICollection<ForecastMorbidityWhoBase> ForecastMorbidityWhoBases { get; set; }
        public virtual ICollection<Program> Programs { get; set; }
    }
}
