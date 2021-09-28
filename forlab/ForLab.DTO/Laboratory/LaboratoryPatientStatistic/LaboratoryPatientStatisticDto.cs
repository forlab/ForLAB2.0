using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryPatientStatistic
{
    public class LaboratoryPatientStatisticDto : BaseEntityDto
    {
        public int LaboratoryId { get; set; }
        public DateTime Period { get; set; }
        public int Count { get; set; }

        //UI
        public string LaboratoryName { get; set; }
        public int? LaboratoryRegionCountryId { get; set; }
        public int? LaboratoryRegionId { get; set; }
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
    }
    public class LaboratoryPatientStatisticDrp : DropdownDrp
    {
        public string LaboratoryName { get; set; }
    }
}

