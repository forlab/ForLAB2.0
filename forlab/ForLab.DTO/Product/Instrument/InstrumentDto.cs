using ForLab.DTO.Common;

namespace ForLab.DTO.Product.Instrument
{
  public  class InstrumentDto: BaseEntityDto
    {
        public int VendorId { get; set; }
        public string Name { get; set; }
        public int MaxThroughPut { get; set; } // max daily or hourly
        public int ThroughPutUnitId { get; set; }
        public int ReagentSystemId { get; set; }
        public int ControlRequirement { get; set; }
        public int ControlRequirementUnitId { get; set; }
        public int TestingAreaId { get; set; }
        public bool Shared { get; set; }

        //UI
        public string VendorName { get; set; }
        public string ThroughPutUnitName { get; set; }
        public string TestingAreaName { get; set; }
        public string ReagentSystemName { get; set; }
        public string ControlRequirementUnitName { get; set; }


    }
    public class InstrumentDrp : DropdownDrp
    {
    }
}
