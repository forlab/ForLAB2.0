using ForLab.Core.Interfaces;
using System.Threading.Tasks;

namespace ForLab.Services.Global.Twilio
{
    public interface ITwilioService
    {
        Task<IResponseDTO> SendPhoneVerificationCode(string to);
        Task<IResponseDTO> PhoneVerificationCheck(string to, string verificationCode);
    }
}
