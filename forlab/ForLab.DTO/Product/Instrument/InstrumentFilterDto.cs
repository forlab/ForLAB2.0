using ForLab.DTO.Common;
namespace ForLab.DTO.Product.Instrument
{
   public class InstrumentFilterDto: BaseFilterDto
    {
        public int VendorId { get; set; }
        public string Name { get; set; }
        public int MaxThroughPut { get; set; } // max daily or hourly
        public int ThroughPutUnitId { get; set; }
        public int ReagentSystemId { get; set; }
        public int ControlRequirement { get; set; }
        public int ControlRequirementUnitId { get; set; }
        public int TestingAreaId { get; set; }
    }
}
