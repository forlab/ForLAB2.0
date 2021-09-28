namespace ForLab.Core.Interfaces
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; set; }
        string FromEmail { get; set; }
        int SmtpPort { get; set; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        bool Ssl { get; set; }

        string PopServer { get; set; }
        int PopPort { get; set; }
        string PopUsername { get; set; }
        string PopPassword { get; set; }
    }
}
