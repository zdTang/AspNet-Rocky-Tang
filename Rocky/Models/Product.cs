using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }

        public string Discription { get; set; }

        [Range(1,int.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }

        [Display(Name="Category Type")]
        public int CategoryId { get; set; }
        // Define a foreign key
        // If only add this virtual Category, the foreign key has been created but not visiable
        // we can add a CategoryID explicitely and bind it with the virtual Category property
        [ForeignKey("CategoryId")]
        public virtual Category Category { set; get; }

        [Display(Name = "Application Type")]
        public int ApplicationId { get; set; }
        // Define a foreign key
        // If only add this virtual Category, the foreign key has been created but not visiable
        // we can add a CategoryID explicitely and bind it with the virtual Category property
        [ForeignKey("ApplicationId")]
        public virtual ApplicationType ApplicationType { set; get; }

    }
}
