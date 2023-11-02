using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using EServicesCms.Models;

namespace EServicesCms.Common
{
    public class EmailManager
    {
        private static string _failureMessage;
        public static string FailureMessage
        {
            get
            {
                return EmailManager._failureMessage;
            }
        }
        public EmailManager()
        {
        }
        public static EmailManager.EmailSentStatus sendEmail(EmailMessage msg)
        {
            EmailManager.EmailSentStatus _status = EmailManager.EmailSentStatus.FAILURE;
            try
            {
                MailMessage message = new MailMessage();
                //
                foreach (var address in msg.Receiver.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!message.To.Contains(new MailAddress(address)))
                        message.To.Add(address);
                }
                //
                if (!string.IsNullOrEmpty(msg.cc))
                    message.CC.Add(msg.cc);
                message.Subject = msg.Subject.ToString();
                message.IsBodyHtml = true;
                message.Body = msg.Message.ToString();
                
                if (msg.Attachments != null && msg.Attachments.Count > 0)
                {
                    foreach (MailAttachments iFile in msg.Attachments)
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(new System.IO.MemoryStream(iFile.FileData), iFile.FileName, iFile.FileType);
                        message.Attachments.Add(attachment);
                    }
                }
                SmtpClient client = new SmtpClient();

                message.From = new MailAddress(msg.Sender);
                client.Port = WebConfig.GetIntValue("SmtpPort");
                client.Host = WebConfig.GetStringValue("SmtpHost");
                client.EnableSsl = WebConfig.GetBoolValue("SmtpEnableSSL");
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.ServicePoint.MaxIdleTime = 1;
                client.Credentials = new System.Net.NetworkCredential(WebConfig.GetStringValue("SmtpUserName"), WebConfig.GetStringValue("SmtpPassword"));

                client.Send(message);
                _status = EmailManager.EmailSentStatus.SUCCESS;
            }
            catch (Exception e)
            {
                HttpContext.Current.Trace.Warn("sendEmail", e.ToString());
                _failureMessage = e.Message;
                _status = EmailManager.EmailSentStatus.FAILURE;
            }
            return _status;
        }
        public enum EmailSentStatus
        {
            SUCCESS,
            FAILURE
        }
    }
}