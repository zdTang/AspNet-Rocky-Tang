using Microsoft.AspNetCore.Identity;

namespace Rocky_Models
{
    /// <summary>
    ///  Here we need to understand
    ///  as the ApplicationUser inherit IdentityUser
    ///  when we add new property into ApplicationUser model, it will be push to the default table
    ///  which is mapping the AspDotNetUser table
    ///  
    /// IdentityUser==>AspDotNetUser table in DB
    /// </summary>

    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
