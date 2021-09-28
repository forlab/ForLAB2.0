using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.TestingSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.DiseaseSchema
{
    [Table("DiseaseTestingProtocols", Schema = "Disease")]
    public class DiseaseTestingProtocol : BaseEntity
    {
        public int DiseaseId { get; set; }
        public int TestingProtocolId { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual TestingProtocol TestingProtocol { get; set; }
    }
}
