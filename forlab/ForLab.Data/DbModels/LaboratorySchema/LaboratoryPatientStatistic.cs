using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LaboratorySchema
{
    [Table("LaboratoryPatientStatistics", Schema = "Laboratory")]
    public class LaboratoryPatientStatistic : BaseEntity
    {
        public int LaboratoryId { get; set; }
        public DateTime Period { get; set; }
        public int Count { get; set; }
        public virtual Laboratory Laboratory { get; set; }
    }
}
