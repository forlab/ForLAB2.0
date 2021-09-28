using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastLaboratoryTestService
{
    public class ForecastLaboratoryTestServiceFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; }
        public string Name { get; set; }
    }
}
