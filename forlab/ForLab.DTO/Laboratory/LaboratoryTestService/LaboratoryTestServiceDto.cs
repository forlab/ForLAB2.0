using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryTestService
{
    public class LaboratoryTestServiceDto : BaseEntityDto
    {
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }
        public DateTime ServiceDuration { get; set; }
        public decimal TestPerformed { get; set; }

        //UI
        public string TestName { get; set; }
        public string LaboratoryName { get; set; }
        public int? LaboratoryRegionCountryId { get; set; }
        public int? LaboratoryRegionId { get; set; }
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
    }
    public class LaboratoryTestServiceDrp : DropdownDrp
    {
        public string TestName { get; set; }
        public string LaboratoryName { get; set; }
    }
}
