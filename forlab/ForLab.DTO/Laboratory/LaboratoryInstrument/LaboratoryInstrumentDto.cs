using ForLab.DTO.Common;


namespace ForLab.DTO.Laboratory.LaboratoryInstrument
{
    public class LaboratoryInstrumentDto : BaseEntityDto
    {
        public int InstrumentId { get; set; }
        public int LaboratoryId { get; set; }
        public int Quantity { get; set; }
        public decimal TestRunPercentage { get; set; }

        //UI
        public string InstrumentName { get; set; }
        public string LaboratoryName { get; set; }
        public int? LaboratoryRegionCountryId { get; set; }
        public int? LaboratoryRegionId { get; set; }
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
    }
    public class LaboratoryInstrumentDrp : DropdownDrp
    {
        public string InstrumentName { get; set; }
        public string LaboratoryName { get; set; }
    }
}