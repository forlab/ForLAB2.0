using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.TestingSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LaboratorySchema
{
    [Table("LaboratoryTestServices", Schema = "Laboratory")]
    public class LaboratoryTestService : BaseEntity
    {
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }
        public DateTime ServiceDuration { get; set; }
        public decimal TestPerformed { get; set; }

        public virtual Test Test { get; set; }
        public virtual Laboratory Laboratory { get; set; }
    }
}
