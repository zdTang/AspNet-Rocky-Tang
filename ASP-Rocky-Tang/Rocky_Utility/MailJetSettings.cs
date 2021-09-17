using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Utility
{
    public class MailJetSettings
    {

        // Attention: those fields must match with Appsetting.json totally !! or will have error
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }              
    }
}
