using ForLab.DTO.Common;

namespace ForLab.DTO.Security.User
{
    public class LoginParamsDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
        public string EmergencyCode { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class VerifyPasswordParamsDto
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }
    public class ResetPasswordParamsDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public LocationDto LocationDto { get; set; }

    }

    public class ChangePasswordParamsDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class ChangeEmailParamsDto
    {
        public int UserId { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class LogoutTransactionParamsDto
    {
        public int UserId { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class VerifyPhoneParamsDtos
    {
        public string Phone { get; set; }
        public string VerificationCode { get; set; }
    }

    public class ChangePhoneParamsDto
    {
        public int UserId { get; set; }
        public string NewPhone { get; set; }
        public string NewCallingCode { get; set; }
        public string VerificationCode { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class UpdateProfileParamsDto
    {
        public UserDto UserDto { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class ChangeUserStatusParamsDto
    {
        public int UserId { get; set; }
        public string Status { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class ForgetPassParamsDto
    {
        public string Email { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class SelfRegistrationParamsDto
    {
        public UserDto UserDto { get; set; }
        public int PlanId { get; set; }
        public string Token { get; set; }
        public string StripeCouponId { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class SetPasswordParamsDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public LocationDto LocationDto { get; set; }
    }

    public class ConfirmEmailParamsDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public LocationDto LocationDto { get; set; }
    }
}
