using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Forecasting.ForecastResult
{
    public class ForecastResultDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int? TestId { get; set; }
        public int ProductId { get; set; }
        public decimal AmountForecasted { get; set; } // which will be fetched from the machine learning model
        public decimal TotalValue { get; set; }
        public DateTime DurationDateTime { get; set; }
        public string Period { get; set; }
        public decimal PackSize { get; set; }
        public int PackQty { get; set; }
        public decimal PackPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProductTypeId { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string LaboratoryName { get; set; }
        public string TestName { get; set; }
        public string ProductName { get; set; }
        public string ProductTypeName { get; set; }
    }
}
