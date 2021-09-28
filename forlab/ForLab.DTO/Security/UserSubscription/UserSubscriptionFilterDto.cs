using ForLab.DTO.Common;

namespace ForLab.DTO.Security.UserSubscription
{
    public class UserSubscriptionFilterDto : BaseFilterDto
    {
        public int ApplicationUserId { get; set; }
        public int CountryId { get; set; }
        public int RegionId { get; set; }
        public int LaboratoryId { get; set; }
        public string Name { get; set; }
    }
}
