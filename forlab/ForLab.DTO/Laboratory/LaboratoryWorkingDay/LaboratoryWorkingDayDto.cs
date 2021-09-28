using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryWorkingDay
{
    public class LaboratoryWorkingDayDto : BaseEntityDto
    {
        public int LaboratoryId { get; set; }
        public string Day { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        //UI
        public string FormatedFromTime { get; set; }
        public string FormatedToTime { get; set; }
        public string LaboratoryName { get; set; }
        public int? LaboratoryRegionCountryId { get; set; }
        public int? LaboratoryRegionId { get; set; }
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
    }
    public class LaboratoryWorkingDayDrp : DropdownDrp
    {
        public string LaboratoryName { get; set; }
    }
}
