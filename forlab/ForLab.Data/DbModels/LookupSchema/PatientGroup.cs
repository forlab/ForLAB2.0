using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.TestingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("PatientGroups", Schema = "Lookup")]
    public class PatientGroup : DynamicLookup
    {
        public PatientGroup()
        {
            ForecastPatientGroups = new HashSet<ForecastPatientGroup>();
            TestingProtocols = new HashSet<TestingProtocol>();
            ForecastMorbidityTestingProtocolMonths = new HashSet<ForecastMorbidityTestingProtocolMonth>();
        }
        public bool Shared { get; set; }
        public virtual ICollection<ForecastPatientGroup> ForecastPatientGroups { get; set; }
        public virtual ICollection<TestingProtocol> TestingProtocols { get; set; }
        public virtual ICollection<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
    }
}
