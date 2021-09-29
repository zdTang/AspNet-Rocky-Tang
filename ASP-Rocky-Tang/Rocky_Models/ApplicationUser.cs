using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required]
        public string FullName { get; set; }
        [NotMapped]
        public string StreetAddress { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string PostalCode { get; set; }
    }
}
