using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace UtilityLayer.Helper.SendEmail
{
    public class EmailNotificationModel
    {
        public string? EMAIL_ID { get; set; }
        public string? SUBJECT { get; set; }
        public string? BODY { get; set; }
        public bool IS_HTML_BODY { get; set; }
    }
    public class EmailSmtpModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
    }
    public class SendEmailHelper
    {
        #region Properties
        private static string userName = string.Empty;
        private static string password = string.Empty;
        private static string smtpServer = string.Empty;
        private static int smtpPort;
        #endregion

        #region Mapped 
        public static void Configure(EmailSmtpModel emailSmtp)
        {
            userName = emailSmtp.UserName ?? "";
            password = emailSmtp.Password ?? "";
            smtpServer = emailSmtp.SmtpServer ?? "";
            smtpPort = emailSmtp.Port;
        }
        #endregion

        #region Method : SendEmail 
        public static async Task<bool> SendEmail(EmailNotificationModel model)
        {
            MailMessage mailMessage = new MailMessage(userName, model.EMAIL_ID, model.SUBJECT, model.BODY)
            {
                IsBodyHtml = model.IS_HTML_BODY
            };

            NetworkCredential cred = new NetworkCredential(userName, password);
            SmtpClient mailClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = cred
            };

            bool isSuccess;
            try
            {
                mailClient.SendAsync(mailMessage,null);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccess;
        }
        #endregion
    }
}
