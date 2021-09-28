using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LaboratorySchema
{
    [Table("LaboratoryWorkingDays", Schema = "Laboratory")]
    public class LaboratoryWorkingDay : BaseEntity
    {
        public int LaboratoryId { get; set; }
        public string Day { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        public virtual Laboratory Laboratory { get; set; }
    }
}
