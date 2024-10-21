using System;
using Serilog;
using System.Net;
using System.Net.Mail;
using LocationMessanger.Settings;
using Microsoft.Extensions.Options;

namespace Common
{
    public class MailF
    {
        public ILogger log = Log.Logger;
        public MailF(IOptions<ServerSettings> settings)
        {
            ip = settings.Value.IP;
            domen = settings.Value.Domen;
            mailAddress = settings.Value.mail_address;
            mailPassword = settings.Value.mail_password;
            GmailServer = settings.Value.smtp_server;
            GmailPort = settings.Value.smtp_port;
            emailEnable = settings.Value.email_enable;
            smtp = new SmtpClient(GmailServer, GmailPort);
            smtp.Credentials = new NetworkCredential(mailAddress, mailPassword);
            from = new MailAddress(mailAddress, domen);
            smtp.EnableSsl = true;
            
        }
        public bool emailEnable = true;
        private string GmailServer = "smtp.gmail.com";
        private int GmailPort = 587;
        private string ip = "127.0.0.1";
        private string domen = "minimessanger";
        private string mailAddress;
        private string mailPassword;
        private MailAddress from;
        private SmtpClient smtp;
        
        public async void SendEmail(string email, string subject, string text)
        {
            MailAddress to = new MailAddress(email);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = text;
            message.IsBodyHtml = true;
            try
            {
                if (emailEnable)
                {
                    smtp.SendMailAsync(message);
                }
                log.Information("Send message to " + email);
            }
            catch (Exception e)
            {
                log.Error("Can't send email message, ex: " + e.Message);
            }
        }
    }
}
