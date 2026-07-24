using System.Net;
using System.Net.Mail;

namespace Freelify.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_configuration["EmailSettings:Email"]);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient(
    _configuration["EmailSettings:Host"],
    int.Parse(_configuration["EmailSettings:Port"]!));

            smtpClient.Credentials = new NetworkCredential(
    _configuration["EmailSettings:Email"],
    _configuration["EmailSettings:Password"]);

            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);



        }


    }
}
