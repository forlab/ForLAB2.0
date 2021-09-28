using System;


namespace ForLab.DTO.Laboratory.LaboratoryInstrument
{
    public class ExportLaboratoryInstrumentDto
    {
        public int Quantity { get; set; }
        public decimal TestRunPercentage { get; set; }
        public string InstrumentName { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

