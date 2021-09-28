using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryWorkingDay
{
    public class LaboratoryWorkingDayFilterDto : BaseFilterDto
    {
        public int LaboratoryId { get; set; }
        public string Day { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
    }
}
