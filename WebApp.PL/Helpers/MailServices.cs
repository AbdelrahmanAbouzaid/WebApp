using Microsoft.Extensions.Options;
using MimeKit;
using WebApp.PL.Dtos;
using WebApp.PL.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace WebApp.PL.Helpers
{
    public class MailServices(IOptions<MailSettings> options) : IMailServices
    {
        public bool SendEmail(Email email)
        {
            try
            {
                var message = new MimeMessage();
                message.Subject = email.Subject;
                message.From.Add(new MailboxAddress(options.Value.DisplayName, options.Value.Email));
                message.To.Add(MailboxAddress.Parse(email.To));

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = email.Body;
                message.Body = bodyBuilder.ToMessageBody();

                var smtp = new SmtpClient();
                smtp.Connect(options.Value.Host, options.Value.Port,SecureSocketOptions.StartTls);
                smtp.Authenticate(options.Value.Email, options.Value.Password);

                smtp.Send(message);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
