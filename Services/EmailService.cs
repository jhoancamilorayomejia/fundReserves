using System.Net;
using System.Net.Mail;

namespace FoundReserves.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {   // Lee la configuración del appsettings.json
            var from       = _configuration["EmailSettings:From"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port       = int.Parse(_configuration["EmailSettings:Port"]!);
            var username   = _configuration["EmailSettings:Username"];
            var password   = _configuration["EmailSettings:Password"];
            
            // MailMessage es la clase de .NET que representa el correo con remitente, 
            // destinatario, asunto y cuerpo.
            var message = new MailMessage(from!, toEmail, subject, body);
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl   = true  // conexión segura obligatoria
            };

            await client.SendMailAsync(message);
        }
    }
}