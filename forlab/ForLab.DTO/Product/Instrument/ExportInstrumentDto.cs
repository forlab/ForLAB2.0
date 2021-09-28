using System;

namespace ForLab.DTO.Product.Instrument
{
  public  class ExportInstrumentDto
    {
        public string Name { get; set; }
        public int MaxThroughPut { get; set; } 
        public int ControlRequirement { get; set; }
        public int ControlRequirementUnitId { get; set; }
        public string VendorName { get; set; }
        public string ThroughPutUnitName { get; set; }
        public string TestingAreaName { get; set; }
        public string ReagentSystemName { get; set; }
        public string ControlRequirementUnitName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }


    }
}
