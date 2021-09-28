using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.DiseaseProgramSchema
{
    [Table("TestingAssumptionParameters", Schema = "DiseaseProgram")]
    public class TestingAssumptionParameter : BaseEntity
    {
        public TestingAssumptionParameter()
        {
            ForecastTestingAssumptionValues = new HashSet<ForecastTestingAssumptionValue>();
        }
        public int ProgramId { get; set; }
        public string Name { get; set; }

        // forecast method
        public bool IsNumeric { get; set; }
        public bool IsPercentage { get; set; }

        // variable effect
        public bool IsPositive { get; set; }
        public bool IsNegative { get; set; }

        public virtual Program Program { get; set; }
        public virtual ICollection<ForecastTestingAssumptionValue> ForecastTestingAssumptionValues { get; set; }
    }
}
