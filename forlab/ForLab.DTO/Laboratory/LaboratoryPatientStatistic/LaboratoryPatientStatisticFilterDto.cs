using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryPatientStatistic
{
    public class LaboratoryPatientStatisticFilterDto : BaseFilterDto
    {
        public int LaboratoryId { get; set; }
        public DateTime? Period { get; set; }
        public int Count { get; set; }
    }
}
