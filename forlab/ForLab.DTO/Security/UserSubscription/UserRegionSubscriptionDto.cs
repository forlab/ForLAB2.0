using ForLab.DTO.Common;


namespace ForLab.DTO.Security.UserSubscription
{
    public class UserRegionSubscriptionDto : BaseEntityDto
    {
        public int ApplicationUserId { get; set; }
        public int RegionId { get; set; }

        // UI
        public string RegionCountryName { get; set; }
        public string RegionName { get; set; }
    }
}
