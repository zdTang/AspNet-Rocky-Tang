using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Rocky.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : PageModel
    {

        private readonly ILogger<LoginModel> _logger;

        public LockoutModel(
            ILogger<LoginModel> logger
            )
        {
  
            _logger = logger;
        }



        public void OnGet()
        {
            _logger.LogWarning("Login--Instantiate");
        }
    }
}
