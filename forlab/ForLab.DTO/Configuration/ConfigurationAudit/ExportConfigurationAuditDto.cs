using System;

namespace ForLab.DTO.Configuration.ConfigurationAudit
{
    public class ExportConfigurationAuditDto
    {  
        // info
        public string Action { get; set; }
        public string Creator { get; set; }
        public DateTime DateOfAction { get; set; }


        // User Managment
        public int NumOfDaysToChangePassword { get; set; }
        public int AccountLoginAttempts { get; set; }
        public int PasswordExpiryTime { get; set; }
        public double UserPhotosize { get; set; }
        public double AttachmentsMaxSize { get; set; }
        public int TimesCountBeforePasswordReuse { get; set; }
        public int TimeToSessionTimeOut { get; set; }
    }
}
