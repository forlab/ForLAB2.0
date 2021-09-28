using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.TestingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    // it will be one year, two years
    [Table("CalculationPeriods", Schema = "Lookup")]
    public class CalculationPeriod : StaticLookup
    {
        public CalculationPeriod()
        {
            CalculationPeriodMonths = new HashSet<CalculationPeriodMonth>();
            TestingProtocols = new HashSet<TestingProtocol>();
        }
        public virtual ICollection<CalculationPeriodMonth> CalculationPeriodMonths { get; set; }
        public virtual ICollection<TestingProtocol> TestingProtocols { get; set; }
    }
}
