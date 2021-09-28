using ForLab.DTO.Common;

namespace ForLab.DTO.Security.UserSubscription
{
    public class UserLaboratorySubscriptionDto : BaseEntityDto
    {
        public int ApplicationUserId { get; set; }
        public int LaboratoryId { get; set; }

        // UI
        public string LaboratoryRegionCountryName { get; set; }
        public string LaboratoryRegionName { get; set; }
        public string LaboratoryName { get; set; }
    }
}
