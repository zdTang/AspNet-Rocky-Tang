using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class ApplicationType
    {
        // To specify this is the primary key 
        // This value will increase automatically without any mannual operation !

        /*=============     How to setup those detailed info. such as the length of Fields ???        ===========*/
        [Key]
        public int Id { get; set; }                // NO NULL
        [Required(ErrorMessage ="Need a name"), MaxLength(30)]
        public string Name { get; set; }           // before it was allow null, nvarchar(MAX)
                                                   // After adding [Refqured] it cannot be null
                                                   // Need migrate Database
        /* This annotation will be used for " asp-for " to display on webpage*/
        [DisplayName("Display Order")]
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Only positive value is OK")]
        public int DisplayOrder { get; set; }      //  NO NULL

    }
}
    