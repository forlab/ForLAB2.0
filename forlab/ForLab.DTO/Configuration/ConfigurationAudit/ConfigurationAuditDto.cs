using ForLab.DTO.Common;

namespace ForLab.DTO.Configuration.ConfigurationAudit
{
    public class ConfigurationAuditDto : AuditEntityDto
    {
        public int ConfigurationId { get; set; }
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
