using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.TestingSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.DiseaseProgramSchema
{
    [Table("ProgramTests", Schema = "DiseaseProgram")]
    public class ProgramTest : BaseEntity
    {
        public int ProgramId { get; set; }
        public int TestId { get; set; }
        public int TestingProtocolId { get; set; }

        public virtual Program Program { get; set; }
        public virtual Test Test { get; set; }
        public virtual TestingProtocol TestingProtocol { get; set; }
    }
}
