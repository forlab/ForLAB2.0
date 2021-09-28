using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastLaboratoryConsumption
{
    public class ForecastLaboratoryConsumptionDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string LaboratoryName { get; set; }
        public string ProductName { get; set; }
    }
}
