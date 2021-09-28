using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastLaboratoryTestService
{
    public class ForecastLaboratoryTestServiceDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; } // which will be fetched from the machine learning model

        // UI
        public string ForecastInfoName { get; set; }
        public string LaboratoryName { get; set; }
        public string TestName { get; set; }
    }
}
