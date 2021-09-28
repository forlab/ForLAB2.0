using ForLab.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Preview.AccSecurity.Service;

namespace ForLab.Services.Global.Twilio
{
    public class TwilioService : ITwilioService
    {
        private readonly IConfiguration _configuration;
        private readonly IResponseDTO _responseDTO;
        private readonly string _accountSID;
        private readonly string _authToken;
        private readonly string _pathServiceSID;
        public TwilioService(IConfiguration configuration, IResponseDTO responseDTO)
        {
            _configuration = configuration;
            _responseDTO = responseDTO;
            _accountSID = _configuration["Twilio:AccountSID"];
            _authToken = _configuration["Twilio:AuthToken"];
            _pathServiceSID = _configuration["Twilio:PathServiceSid"];
        }
        public async Task<IResponseDTO> PhoneVerificationCheck(string to, string verificationCode)
        {
            if (string.IsNullOrEmpty(to))
            {
                _responseDTO.Message = "Invalid phone number";
                _responseDTO.IsPassed = false;
                return _responseDTO;
            }
            if (string.IsNullOrEmpty(verificationCode) || verificationCode?.Length < 4)
            {
                _responseDTO.Message = "Invalid verification code";
                _responseDTO.IsPassed = false;
                return _responseDTO;
            }

            try
            {
                TwilioClient.Init(_accountSID, _authToken);
                var verificationCheck = await VerificationCheckResource.CreateAsync(to: to, code: verificationCode, pathServiceSid: _pathServiceSID);
                if (!verificationCheck.Valid.Value)
                {
                    _responseDTO.Data = null;
                    _responseDTO.Message = "Verification code is wrong";
                    _responseDTO.IsPassed = false;
                    return _responseDTO;
                }
                if (verificationCheck.Status == "pending")
                {
                    _responseDTO.Data = null;
                    _responseDTO.Message = "Verification code is wrong";
                    _responseDTO.IsPassed = false;
                    return _responseDTO;
                }
            }
            catch (Exception ex)
            {
                _responseDTO.Data = null;
                _responseDTO.Message = $"Error {ex.Message}";
                _responseDTO.IsPassed = false;
            }


            return _responseDTO;
        }

        public async Task<IResponseDTO> SendPhoneVerificationCode(string to)
        {
            try
            {
                if (string.IsNullOrEmpty(to))
                {
                    _responseDTO.Message = "Invalid Phone Number";
                    _responseDTO.IsPassed = false;
                    return _responseDTO;
                }

                TwilioClient.Init(_accountSID, _authToken);
                var verification = await VerificationResource.CreateAsync(to: to, channel: "sms", pathServiceSid: _pathServiceSID);
                _responseDTO.Data = null;
                _responseDTO.Message = "Verification code is sent";
                _responseDTO.IsPassed = true;
            }
            catch (Exception ex)
            {
                _responseDTO.Data = null;
                _responseDTO.Message = $"Error {ex.Message}";
                _responseDTO.IsPassed = false;
            }

            return _responseDTO;
        }
    }
}
