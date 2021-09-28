using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("EntityTypes", Schema = "Lookup")]
    public class EntityType : StaticLookup
    {
        public EntityType()
        {
            MMProgramParameters = new HashSet<PatientAssumptionParameter>();
        }

        public virtual ICollection<PatientAssumptionParameter> MMProgramParameters { get; set; }
    }
}
