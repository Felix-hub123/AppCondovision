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
        /// <param name="email">Endereço de email do destinatário.</param>
        /// <param name="subject">Assunto do email.</param>
        /// <param name="htmlMessage">Conteúdo HTML do corpo do email.</param>
        /// <returns>Um booleano indicando se o envio do email foi bem-sucedido.</returns>
        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var nameFrom = _configuration["Mail:NameFrom"];
                var from = _configuration["Mail:From"];
                var smtp = _configuration["Mail:Smtp"];
                var portStr = _configuration["Mail:Port"]; // Mudei o nome para evitar conflito de tipo
                var password = _configuration["Mail:Password"];

                // --- AQUI ESTÁ A CORREÇÃO ---
                // Se a string da porta for nula ou não for um número, use um valor padrão (ex: 587)
                if (!int.TryParse(portStr, out int port))
                {
                    // Se a porta não puder ser convertida, pode registar um erro
                    // e retornar false, pois não é possível ligar ao servidor.
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(nameFrom ?? from, from));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;

                var bodybuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage,
                };
                message.Body = bodybuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(smtp, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(from, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception )
            {
                // ... (registar exceção)
                return false;
            }
        }
    }
}
