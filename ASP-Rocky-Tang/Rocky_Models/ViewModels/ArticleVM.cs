using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Rocky_Models.ViewModels
{
    public class ArticleVM
    {
        public Article Article { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
