using System;
using System.Collections.Generic;

namespace ForLab.DTO.Security.User
{
    public class AuthorizedUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CallingCode { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool ChangePassword { get; set; }
        public int? RoleId { get; set; }
        public string Status { get; set; } // Active, NotActive, Locked
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string PersonalImagePath { get; set; }
        public bool EmailConfirmed { get; set; }
        public string DefaultLanguage { get; set; }
        public int? UserSubscriptionLevelId { get; set; }
        public List<string> UserRoles { get; set; }
  
    }
}
