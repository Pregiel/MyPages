using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Models
{
    public class CreateNewPageModel
    {
        private const string _fieldNullMessage = "{0} is required.";

        [Required(ErrorMessage = _fieldNullMessage)]
        [StringLength(32)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
