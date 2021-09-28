using ForLab.DTO.Common;

namespace ForLab.DTO.Security.User
{
    public class UserFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string Status { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public int UserSubscriptionLevelId { get; set; }
    }
}
