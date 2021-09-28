using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ForLab.Data.DbModels.DiseaseProgramSchema
{
    // morbidity only
    [Table("PatientAssumptionParameters", Schema = "DiseaseProgram")]
    public class PatientAssumptionParameter : BaseEntity
    {
        public PatientAssumptionParameter()
        {
            ForecastPatientAssumptionValues = new HashSet<ForecastPatientAssumptionValue>();
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
        public virtual ICollection<ForecastPatientAssumptionValue> ForecastPatientAssumptionValues { get; set; }
    }
}
