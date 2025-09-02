using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    /// <summary>
    /// Helper para envio de email via SMTP usando a biblioteca MailKit.
    /// </summary>
    public class EMailHelper : IEMailHelper
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="EMailHelper"/> com as configurações necessárias para envio de email.
        /// </summary>
        /// <param name="configuration">Interface para obter configurações da aplicação.</param>
        public EMailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Envia um email assíncrono para o destinatário especificado com o assunto e corpo em HTML.
        /// </summary>
        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var nameFrom = _configuration["Mail:NameFrom"];
                var from = _configuration["Mail:From"];
                var smtp = _configuration["Mail:Smtp"];
                var portStr = _configuration["Mail:Port"];
                var password = _configuration["Mail:Password"];

                if (!int.TryParse(portStr, out int port))
                {
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(nameFrom ?? from, from));
                message.To.Add(MailboxAddress.Parse(email)); 
                message.Subject = subject;

                var bodybuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage,
                };
                message.Body = bodybuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                   
                    SecureSocketOptions security = SecureSocketOptions.StartTls;
                    if (port == 465)
                    {
                        security = SecureSocketOptions.SslOnConnect;
                    }

                    await client.ConnectAsync(smtp, port, security);
                    await client.AuthenticateAsync(from, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                return false;
            }
        }
    }
}
