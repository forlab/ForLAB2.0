using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseSchema;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.DiseaseProgramSchema
{
    [Table("Programs", Schema = "DiseaseProgram")]
    public class Program : BaseEntity
    {
        public Program()
        {
            PatientAssumptionParameters = new HashSet<PatientAssumptionParameter>();
            ProductAssumptionParameters = new HashSet<ProductAssumptionParameter>();
            TestingAssumptionParameters = new HashSet<TestingAssumptionParameter>();
            ProgramTests = new HashSet<ProgramTest>();
            ForecastMorbidityPrograms = new HashSet<ForecastMorbidityProgram>();
            ForecastMorbidityTestingProtocolMonths = new HashSet<ForecastMorbidityTestingProtocolMonth>();
            ForecastPatientGroups = new HashSet<ForecastPatientGroup>();
        }

        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public int NumberOfYears { get; set; }

        public virtual Disease Disease { get; set; }
        public virtual ICollection<PatientAssumptionParameter> PatientAssumptionParameters { get; set; }
        public virtual ICollection<ProductAssumptionParameter> ProductAssumptionParameters { get; set; }
        public virtual ICollection<TestingAssumptionParameter> TestingAssumptionParameters { get; set; }
        public virtual ICollection<ProgramTest> ProgramTests { get; set; }
        public virtual ICollection<ForecastMorbidityProgram> ForecastMorbidityPrograms { get; set; }
        public virtual ICollection<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
        public virtual ICollection<ForecastPatientGroup> ForecastPatientGroups { get; set; }
    }
}
