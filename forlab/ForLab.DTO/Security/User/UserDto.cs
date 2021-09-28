using ForLab.DTO.Security.UserSubscription;
using System;
using System.Collections.Generic;

namespace ForLab.DTO.Security.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PersonalImagePath { get; set; }
        public string IP { get; set; }
        public bool ChangePassword { get; set; }
        public string CallingCode { get; set; }
        public string JobTitle { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Status { get; set; } // Active, NotActive, Locked
        public string ElectronicSignature { get; set; }
        public DateTime NextPasswordExpiryDate { get; set; }
        public DateTime? EmailVerifiedDate { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public int? UserSubscriptionLevelId { get; set; }
        public int? RegionId { get; set; } // Set region id in case of laboratory level
        public List<UserCountrySubscriptionDto> UserCountrySubscriptionDtos { get; set; }
        public List<UserRegionSubscriptionDto> UserRegionSubscriptionDtos { get; set; }
        public List<UserLaboratorySubscriptionDto> UserLaboratorySubscriptionDtos { get; set; }

        // UI
        public string UserSubscriptionLevelName { get; set; }
        public bool ReomveProfileImage { get; set; }
    }

    public class UserDrp
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string PersonalImagePath { get; set; }
    }
}
