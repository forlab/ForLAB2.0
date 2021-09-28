using System;

namespace ForLab.DTO.Security.User
{
    public class ExportUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string JobTitle { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; } // Active, NotActive, Locked
        public string UserSubscriptionLevelName { get; set; }
        public DateTime NextPasswordExpiryDate { get; set; }
    }
}
