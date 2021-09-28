using ForLab.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Global.SendEmail
{
    public interface IEmailService
    {
        Task Send(EmailMessage emailMessage);


        // Templates
        #region User transactions
        Task AfterRegistiration(string email, string vaildToken);
        Task UnlockUserEmail(string email, string vaildToken);
        Task RequestToResetPassword(string email, string validToken);
        Task AfterResetPassword(string email);
        Task AfterEmailChanges(string email, DateTime date, string ip, string country);
        Task AfterPasswordChanges(string email, DateTime date, string ip, string country);
        Task AfterUserRoleChanges(string email, string companyLogoPath, string companyName, string from, string to);
        Task SendEmailConfirmationRequest(string email, string token);
        Task SendEmailReply(string email, string message);
        #endregion

        #region Security Emails
        Task AfterIpAddressChanges(string email, DateTime date, string ip, string country);
        Task AfterAccountIsDisabled(string email, DateTime date, string ip, string country);
        Task WrongPasswordAttempt(string email, DateTime date, string ip, string country);
        Task TwoFactorAuthCode(string email, string code);
        #endregion

        #region Ticket
        Task AfterCreatingTicketForAdmin(List<string> emails, string companyLogoPath, string fromEmail, string companyName, string ticketType, string ticketDetails, int ticketId);
        Task AfterTicketReplyFromAdmin(string email, string ticketDetails, int ticketId);
        #endregion

    }
}
