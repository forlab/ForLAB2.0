namespace ForLab.Core.Interfaces
{
    public interface ISendEmailWithErrorConfiguration
    {
        bool AllowSend { get; set; }
        string ToEmails { get; set; }
    }
}
