using System;

namespace ForLab.DTO.Forecasting.ForecastLaboratory
{
    public class ExportForecastLaboratoryDto
    {
        public string ForecastCategoryName { get; set; }
        public string LaboratoryName { get; set; }
        public string ForecastInfoName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
