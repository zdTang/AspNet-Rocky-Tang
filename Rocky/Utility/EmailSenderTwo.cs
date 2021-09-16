using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Rocky.Utility
{
    public class EmailSenderTwo : IEmailSender
    {
        public Task SendEmailAsync(string receiver, string subject, string htmlMessage)
        {
            return Execute( receiver, subject, htmlMessage);
        }


        public async Task Execute(string receiver, string subject, string body)
        {
            string emailAccount = WC.EmailSender;
            string Password = WC.EmailSenderPass;

            SmtpClient smtpClient = new SmtpClient(emailAccount, 25);

            smtpClient.Credentials = new System.Net.NetworkCredential(emailAccount, Password);
            smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(emailAccount, "MyWeb Site");
            mail.To.Add(new MailAddress(receiver));
            mail.Body = body;
            mail.Subject = subject;
            //mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));

            //smtpClient.Send(mail);
            //await smtpClient.SendAsync(mail);
            await smtpClient.SendMailAsync(mail);
        }

    }
}
