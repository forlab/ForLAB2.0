using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.TestingSchema
{
    [Table("TestingProtocolCalculationPeriodMonths", Schema = "Testing")]
    public class TestingProtocolCalculationPeriodMonth : BaseEntity
    {
        public int TestingProtocolId { get; set; }
        public int CalculationPeriodMonthId { get; set; }
        public decimal? Value { get; set; }

        public virtual TestingProtocol TestingProtocol { get; set; }
        public virtual CalculationPeriodMonth CalculationPeriodMonth { get; set; }
    }
}
