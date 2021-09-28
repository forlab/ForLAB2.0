using ForLab.DTO.Common;
using ForLab.DTO.Forecasting.ForecastLaboratory;
using System.Collections.Generic;

namespace ForLab.DTO.Forecasting.ForecastCategory
{
    public class ForecastCategoryDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public string Name { get; set; }
        public List<ForecastLaboratoryDto> ForecastLaboratoryDtos { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
    }
}
