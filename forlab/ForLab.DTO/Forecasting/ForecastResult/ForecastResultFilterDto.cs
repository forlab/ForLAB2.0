using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastResult
{
    public class ForecastResultFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int? TestId { get; set; }
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
    }
}
