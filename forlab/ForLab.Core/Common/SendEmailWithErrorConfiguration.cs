using ForLab.Core.Interfaces;

namespace ForLab.Core.Common
{
    public class SendEmailWithErrorConfiguration : ISendEmailWithErrorConfiguration
    {
        public bool AllowSend { get; set; }
        public string ToEmails { get; set; }
    }
}
