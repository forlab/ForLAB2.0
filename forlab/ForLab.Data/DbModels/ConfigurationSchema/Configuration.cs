using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ConfigurationSchema
{
    [Table("Configurations", Schema = "Configuration")]
    public class Configuration
    {
        public Configuration()
        {
            ConfigurationAudits = new HashSet<ConfigurationAudit>();
        }

        public int Id { get; set; }
        // User Managment
        public int NumOfDaysToChangePassword { get; set; }
        public int AccountLoginAttempts { get; set; }
        public int PasswordExpiryTime { get; set; }
        public double UserPhotosize { get; set; }
        public double AttachmentsMaxSize { get; set; }
        public int TimesCountBeforePasswordReuse { get; set; }
        public int TimeToSessionTimeOut  { get; set; }

        public virtual ICollection<ConfigurationAudit> ConfigurationAudits { get; set; }
    }
}
