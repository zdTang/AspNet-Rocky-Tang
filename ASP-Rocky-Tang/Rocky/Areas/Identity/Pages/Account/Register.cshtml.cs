using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rocky_Models;
using Rocky_Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Rocky.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;    // added

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)    // Tang added
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;               // Tang added
            _logger.LogWarning("Register--Instantiation");
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        //TODO:  how these fields bind AspNetCourUser table?  - TANG
        //       how this Model add a field, then the framework will map it to AspNetCourUser table

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            // Add two more properties  --- Tang
            public string FullName { get; set; }             // tang added    
            public string PhoneNumber { get; set; }          // Identity package build-in
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            //Tang Added
            _logger.LogWarning("Register--Get");
            _logger.LogWarning(User.Identity.Name);
            if (!await _roleManager.RoleExistsAsync(WC.AdminRole))
            {
                // Create two roles,  role is just a string !!!! like a label

                await _roleManager.CreateAsync(new IdentityRole(WC.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(WC.CustomerRole));
            }
            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            _logger.LogWarning("Register--Post");
            _logger.LogWarning(User.Identity.Name);
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                // you can see, the asp.net use Email use userName by default
                
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, PhoneNumber=Input.PhoneNumber,FullName=Input.FullName};

                var result = await _userManager.CreateAsync(user, Input.Password); // add to AspNetUsers Table
                
                if (result.Succeeded)
                {
                    //this line  will be run for createing the first USER - Administrator !

                    //await _userManager.AddToRoleAsync(user, WC.AdminRole);
                    
                    if (User.IsInRole(WC.AdminRole))       // User object -- represent the user logged in
                    {
                        // An admin has logged in 
                        await _userManager.AddToRoleAsync(user, WC.AdminRole);   // Tang added

                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, WC.CustomerRole);   // Tang added
                    }
                    

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        //  if it is not Admin, means it is a new normal user, then sign in  after registeration
                        //  if it is Admin, mean the Admin is creating another Admin user, do not need sign in automatically
                        if (!User.IsInRole(WC.AdminRole))   
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
