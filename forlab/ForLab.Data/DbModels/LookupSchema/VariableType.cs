using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("VariableTypes", Schema = "Lookup")]
    public class VariableType : StaticLookup
    {
        // numeric, percentage
        public VariableType()
        {
            MMProgramParameters = new HashSet<PatientAssumptionParameter>();
        }

        public virtual ICollection<PatientAssumptionParameter> MMProgramParameters { get; set; }
    }
}
