using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class Category
    {
        // To specify this is the primary key 
        // This value will increase automatically without any mannual operation !

        /*=============     How to setup those detailed info. such as the length of Fields ???        ===========*/
        [Key]
        public int Id { get; set; }                // NO NULL
        public string Name { get; set; }           // allow null, nvarchar(MAX)
        public int DisplayOrder { get; set; }      //  NO NULL

    }
}
    