using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Laboratory.LaboratoryTestService
{
    public class LaboratoryTestServiceFilterDto : BaseFilterDto
    {
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }
        public DateTime? ServiceDuration { get; set; }
        public decimal? TestPerformed { get; set; }
    }
}
