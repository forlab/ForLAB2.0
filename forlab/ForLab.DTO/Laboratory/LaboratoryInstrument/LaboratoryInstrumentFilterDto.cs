using ForLab.DTO.Common;

namespace ForLab.DTO.Laboratory.LaboratoryInstrument
{
    public class LaboratoryInstrumentFilterDto : BaseFilterDto
    {
        public int InstrumentId { get; set; }
        public int LaboratoryId { get; set; }
        public int Quantity { get; set; }
        public decimal? TestRunPercentage { get; set; }
    }
}
