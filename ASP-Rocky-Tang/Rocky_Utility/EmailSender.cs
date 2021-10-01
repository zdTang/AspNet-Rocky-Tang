using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Utility
{
    public class EmailSender : IEmailSender
    {
        
        // this configration is used to access appsettings.json
        // we saved MailJet keys at appsetting.json so that we need fetch those values
        
        private readonly IConfiguration _configration;

        public MailJetSettings _mailJetSettings { set; get; }


        //  Configuration has been injected by the Framework
        /// <summary>
        /// Here try to make clear how the Dependency Injection works !!!
        /// </summary>
        /// <param name="configration"></param>
        public EmailSender(IConfiguration configration) {
            _configration = configration;
        }
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            // Access appsetting.json to get MailJet keys

            _mailJetSettings = _configration.GetSection("MailJet").Get<MailJetSettings>();
            
                      
            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey)
            {
                Version = ApiVersion.V3_1    //   mailjetclientapi== version 1.2.2
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
           // Here to use the Sender email, which must exist in your Mailjet account !!!
        {"Email", "miketangdemo@gmail.com"},
        //{"Email", "mikesoftware@protonmail.com"},
        {"Name", "Mike"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }, {
          "Name",
          "DotNetMastery"
         }
        }
       }
      }, {
       "Subject",
       subject
      }, {
       "HTMLPart",
       body
      }
     }
             });
            await client.PostAsync(request);
        }
    }
}

 