using Bli.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain;
using UserService.Infrastructure.Options;

namespace UserService.Infrastructure.Service
{
    public class SendQQEmailSender : IEmailSender
    {
        private readonly ILogger<SendCloudEmailSender> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptionsSnapshot<EmialSmtpSettings> emialSmtpSettings;

        public SendQQEmailSender(ILogger<SendCloudEmailSender> logger,
           IHttpClientFactory httpClientFactory,
           IOptionsSnapshot<EmialSmtpSettings> emialSmtpSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.emialSmtpSettings = emialSmtpSettings;
        }


        private SendCloudEmailSettings GetSendCloudEmailSettings(string toEmial)
        {
            if(toEmial.EndsWith("@qq.com"))
            {
                return emialSmtpSettings.Value.smtp[0];
            }else if(toEmial.EndsWith("@163.com"))
            {
                return emialSmtpSettings.Value.smtp[1];
            }
            throw new Exception("未配置相关邮件地址");
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            SendCloudEmailSettings sendCloudSettings = GetSendCloudEmailSettings(toEmail);
            logger.LogInformation("SendCloud Email to {0},subject:{1},body:{2}", toEmail, subject, body);
            using (SmtpClient client = new SmtpClient(sendCloudSettings.ApiUser))
            {
                client.Credentials = new NetworkCredential(sendCloudSettings.From, sendCloudSettings.ApiKey);
                client.EnableSsl = true; 
                MailMessage message = new MailMessage(sendCloudSettings.From, toEmail, subject, body);
               await client.SendMailAsync(message);
            }
        }

      
    }
}
