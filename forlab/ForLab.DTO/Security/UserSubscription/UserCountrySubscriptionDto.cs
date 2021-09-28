using ForLab.DTO.Common;

namespace ForLab.DTO.Security.UserSubscription
{
    public class UserCountrySubscriptionDto : BaseEntityDto
    {
        public int ApplicationUserId { get; set; }
        public int CountryId { get; set; }

        // UI
        public string CountryName { get; set; }
    }
}
