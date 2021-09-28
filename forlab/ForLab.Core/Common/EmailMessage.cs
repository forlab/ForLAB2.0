using MimeKit;
using System.Collections.Generic;


namespace ForLab.Core.Common
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
            CcAddresses = new List<EmailAddress>();
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> CcAddresses { get; set; }
        public string Subject { get; set; }
        public MimeEntity Body { get; set; }

    }
}
