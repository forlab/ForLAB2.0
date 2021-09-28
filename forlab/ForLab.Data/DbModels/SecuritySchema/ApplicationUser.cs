using ForLab.Data.DbModels.LookupSchema;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ForLab.Data.DbModels.SecuritySchema
{
    public class ApplicationUser: IdentityUser<int>
    {
        public ApplicationUser()
        {
            Claims = new HashSet<ApplicationUserClaim>();
            Logins = new HashSet<ApplicationUserLogin>();
            Tokens = new HashSet<ApplicationUserToken>();
            UserRoles = new HashSet<ApplicationUserRole>();
            UserCountrySubscriptions = new HashSet<UserCountrySubscription>();
            UserRegionSubscriptions = new HashSet<UserRegionSubscription>();
            UserLaboratorySubscriptions = new HashSet<UserLaboratorySubscription>();
        }

        /*
        if subscribtion level is country then user should select which contenent then select countries that he want
        if subscribtion level is region then user should select specific country then select regions
        if subscribtion level is laboratory is user should select region and then select which laboratoies
         * 
         */

        public int? UserSubscriptionLevelId { get; set; }
        public int? RegionId { get; set; } // Set region id in case of laboratory level
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
        public DateTime NextPasswordExpiryDate { get; set; }
        public DateTime? EmailVerifiedDate { get; set; }

        public virtual UserSubscriptionLevel UserSubscriptionLevel { get; set; }
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<UserCountrySubscription> UserCountrySubscriptions { get; set; }
        public virtual ICollection<UserRegionSubscription> UserRegionSubscriptions { get; set; }
        public virtual ICollection<UserLaboratorySubscription> UserLaboratorySubscriptions { get; set; }
    }
}
