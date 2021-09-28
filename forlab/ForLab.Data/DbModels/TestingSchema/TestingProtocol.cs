using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using ForLab.Data.DbModels.DiseaseSchema;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LookupSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.TestingSchema
{
    [Table("TestingProtocols", Schema = "Testing")]
    public class TestingProtocol : BaseEntity
    {
        public TestingProtocol()
        {
            DiseaseTestingProtocols = new HashSet<DiseaseTestingProtocol>();
            ProgramTests = new HashSet<ProgramTest>();
            TestingProtocolCalculationPeriodMonths = new HashSet<TestingProtocolCalculationPeriodMonth>();
            ForecastMorbidityTestingProtocolMonths = new HashSet<ForecastMorbidityTestingProtocolMonth>();
        }
        public string Name { get; set; }
        public int TestId { get; set; }
        public int PatientGroupId { get; set; }
        public int CalculationPeriodId { get; set; }
        public int BaseLine { get; set; }
        public int TestAfterFirstYear { get; set; }


        public virtual Test Test { get; set; }
        public virtual CalculationPeriod CalculationPeriod { get; set; }
        public virtual PatientGroup PatientGroup { get; set; }
        public virtual ICollection<DiseaseTestingProtocol> DiseaseTestingProtocols { get; set; }
        public virtual ICollection<ProgramTest> ProgramTests { get; set; }
        public virtual ICollection<TestingProtocolCalculationPeriodMonth> TestingProtocolCalculationPeriodMonths { get; set; }
        public virtual ICollection<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
    }
}
