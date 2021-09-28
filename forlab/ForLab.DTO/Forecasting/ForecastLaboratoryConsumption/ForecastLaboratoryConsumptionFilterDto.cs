using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastLaboratoryConsumption
{
    public class ForecastLaboratoryConsumptionFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; }
        public string Name { get; set; }
    }
}
