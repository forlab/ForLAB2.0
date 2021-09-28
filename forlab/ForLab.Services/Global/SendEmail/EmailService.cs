using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForLab.Services.Global.SendEmail
{
    public class EmailService : IEmailService
    {

        private readonly IEmailConfiguration _emailConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _frontEndURL;
        public EmailService(IEmailConfiguration emailConfiguration, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _emailConfiguration = emailConfiguration;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;

            // front url
            _frontEndURL = _configuration["ClientSubscriber:Url"];
        }


        public async Task Send(EmailMessage emailMessage)
        {
            try
            {
                var message = new MimeMessage();

                // Prepare the email object settings [to, cc, from]
                message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

                if (emailMessage.CcAddresses != null && emailMessage.CcAddresses.Count > 0)
                {
                    message.Cc.AddRange(emailMessage.CcAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                }

                message.From.Add(new MailboxAddress(_emailConfiguration.FromEmail, _emailConfiguration.FromEmail));

                // Prepare email subject
                message.Subject = emailMessage.Subject;

                // Prepare email content
                message.Body = emailMessage.Body;

                // Authenticate then send email
                using (var emailClient = new SmtpClient() { })
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;


                    // Connect to server with account credentials and ssl settings
                    emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort);
                    // emailClient();

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Authenticate the connection to server
                    emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                    // Send email
                    await emailClient.SendAsync(message);

                    // Disconnect the object
                    emailClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /*
         * Helper Function
        */
        private string GetTemplatePath(string templateName)
        {
            var pathToFile = _hostingEnvironment.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplates"
                    + Path.DirectorySeparatorChar.ToString()
                    + templateName;
            return pathToFile;
        }
        private string GetImagePath(string imagePath)
        {
            var pathToFile = _hostingEnvironment.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + imagePath;
            return pathToFile;
        }


        /*
         * Templates
        */

        #region User transactions
        public async Task AfterRegistiration(string email, string vaildToken)
        {
            var redirectPage = $"{_frontEndURL}/authentication/reset-password?email={email}&token={vaildToken}";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterRegistiration.html
            var pathToFile = GetTemplatePath("AfterRegistiration.html");

            var builder = new BodyBuilder();

            // check image


            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        redirectPage
                    );
            }

            var subject = $"Registration Success";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task UnlockUserEmail(string email, string vaildToken)
        {
            var redirectPage = $"{_frontEndURL}/authentication/reset-password?email={email}&token={vaildToken}";

            // Get TemplateFile located at wwwroot/EmailTemplates/UnlockUserTemplate.html
            var pathToFile = GetTemplatePath("UnlockUserTemplate.html");

            var builder = new BodyBuilder();

            // check image


            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        redirectPage
                    );
            }

            var subject = $"Account Unlocked";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task RequestToResetPassword(string email, string validToken)
        {
            var redirectPage = $"{_frontEndURL}/authentication/reset-password?email={email}&token={validToken}";

            // Get TemplateFile located at wwwroot/EmailTemplates/RequestToResetPassword.html
            var pathToFile = GetTemplatePath("RequestToResetPassword.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        redirectPage
                    );
            }

            var subject = $"Reset the Password";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task AfterResetPassword(string email)
        {
            var redirectPage = $"{_frontEndURL}/authentication/signin";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterResetThePassword.html
            var pathToFile = GetTemplatePath("AfterResetThePassword.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        redirectPage
                    );
            }

            var subject = $"Password reset";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task AfterEmailChanges(string email, DateTime date, string ip, string country)
        {
            var redirectPage = $"{_frontEndURL}/user/users/activity";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterEmailChanges.html
            var pathToFile = GetTemplatePath("AfterEmailChanges.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                      date.ToString("MM/dd/yyyy hh:mm tt"),
                      ip,
                      country,
                      _emailConfiguration.FromEmail,
                      redirectPage
                  );
            }

            var subject = $"Email address updated";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task AfterPasswordChanges(string email, DateTime date, string ip, string country)
        {
            var redirectPage = $"{_frontEndURL}/security/users/activity";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterPasswordChanges.html
            var pathToFile = GetTemplatePath("AfterPasswordChanges.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        date.ToString("MM/dd/yyyy hh:mm tt"),
                        ip,
                        country,
                        _emailConfiguration.FromEmail,
                        redirectPage
                    );
            }

            var subject = $"Password was changed";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task AfterUserRoleChanges(string email, string companyLogoPath, string companyName, string from, string to)
        {
            // Get TemplateFile located at wwwroot/EmailTemplates/AfterUserRoleChanges.html
            var pathToFile = GetTemplatePath("AfterUserRoleChanges.html");

            var builder = new BodyBuilder();
            // check image
            MimeEntity image;
            if (!string.IsNullOrEmpty(companyLogoPath))
            {
                image = builder.LinkedResources.Add(GetImagePath(companyLogoPath));
                image.ContentId = MimeUtils.GenerateMessageId();
            }
            else
            {
                image = builder.LinkedResources.Add(GetImagePath("EmailTemplates\\default.png"));
                image.ContentId = MimeUtils.GenerateMessageId();
            }

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        image.ContentId,
                        companyName,
                        from,
                        to
                    );
            }

            var subject = $"Security Alert for your account";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task SendEmailConfirmationRequest(string email, string token)
        {
            var redirectPage = $"{_frontEndURL}/authentication/verify-email?token={token}&email={email}";

            // Get TemplateFile located at wwwroot/EmailTemplates/SendEmailConfirmationRequest.html
            var pathToFile = GetTemplatePath("SendEmailConfirmationRequest.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        redirectPage
                    );
            }

            var subject = $"Email verification";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task SendEmailReply(string email, string message)
        {
            // Get TemplateFile located at wwwroot/EmailTemplates/SendEmailReply.html
            var pathToFile = GetTemplatePath("SendEmailReply.html");
            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        message
                    );
            }

            var subject = $"Reply to your question";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,

                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        #endregion


        #region Security Emails
        public async Task AfterIpAddressChanges(string email, DateTime date, string ip, string country)
        {
            var redirectPage = $"{_frontEndURL}/security/users/activity";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterIpAddressChanges.html
            var pathToFile = GetTemplatePath("AfterIpAddressChanges.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        date.ToString("MM/dd/yyyy hh:mm tt"),
                        ip,
                        country,
                        redirectPage
                    );
            }

            var subject = $"Security Alert for your account";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task AfterAccountIsDisabled(string email, DateTime date, string ip, string country)
        {
            // Get TemplateFile located at wwwroot/EmailTemplates/AfterAccountIsDisabled.html
            var pathToFile = GetTemplatePath("AfterAccountIsDisabled.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        date.ToString("MM/dd/yyyy hh:mm tt"),
                        ip,
                        country
                    );
            }

            var subject = $"Security Alert for your account";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task WrongPasswordAttempt(string email, DateTime date, string ip, string country)
        {
            var redirectPage = $"{_frontEndURL}/security/users/activity";

            // Get TemplateFile located at wwwroot/EmailTemplates/WrongPasswordAttempt.html
            var pathToFile = GetTemplatePath("WrongPasswordAttempt.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        date.ToString("MM/dd/yyyy hh:mm tt"),
                        ip,
                        country,
                        redirectPage
                    );
            }

            var subject = $"Security Alert for your account";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        public async Task TwoFactorAuthCode(string email, string code)
        {
            // Get TemplateFile located at wwwroot/EmailTemplates/TwoFactorAuthCode.html
            var pathToFile = GetTemplatePath("TwoFactorAuthCode.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                       code
                    );
            }

            var subject = $"Two-step verification";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        #endregion


        #region Ticket
        public async Task AfterCreatingTicketForAdmin(List<string> emails, string companyLogoPath, string fromEmail, string companyName, string ticketType, string ticketDetails, int ticketId)
        {
            var redirectPage = $"{_frontEndURL}/demo1/support/tickets/details/{ticketId}";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterCreatingTicketForAdmin.html
            var pathToFile = GetTemplatePath("AfterCreatingTicketForAdmin.html");

            var builder = new BodyBuilder();
            // check image
            MimeEntity image;
            if (!string.IsNullOrEmpty(companyLogoPath))
            {
                image = builder.LinkedResources.Add(GetImagePath(companyLogoPath));
                image.ContentId = MimeUtils.GenerateMessageId();
            }
            else
            {
                image = builder.LinkedResources.Add(GetImagePath("EmailTemplates\\default.png"));
                image.ContentId = MimeUtils.GenerateMessageId();
            }

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        image.ContentId,
                        fromEmail,
                        companyName,
                        ticketType,
                        ticketDetails,
                        redirectPage
                    );
            }

            var subject = $"New ticket for you";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = emails.ConvertAll(x => new EmailAddress
                {
                    Address = x,
                    Name = x
                })
            };



            await Send(emailMessage);
        }
        public async Task AfterTicketReplyFromAdmin(string email, string ticketDetails, int ticketId)
        {
            var redirectPage = $"{_frontEndURL}/demo1/support/tickets/details/{ticketId}";

            // Get TemplateFile located at wwwroot/EmailTemplates/AfterTicketReplyFromAdmin.html
            var pathToFile = GetTemplatePath("AfterTicketReplyFromAdmin.html");

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                builder.HtmlBody = string.Format(SourceReader.ReadToEnd(),
                        ticketDetails,
                        redirectPage
                    );
            }

            var subject = $"Ticket reply from ForLab team";
            var emailMessage = new EmailMessage()
            {
                Body = builder.ToMessageBody(),
                Subject = subject,
                ToAddresses = new List<EmailAddress>()
                {
                    new EmailAddress() { Address = email, Name = email }
                }
            };

            await Send(emailMessage);
        }
        #endregion
    }
}
