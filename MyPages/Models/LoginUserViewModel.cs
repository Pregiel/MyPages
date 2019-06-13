using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Models
{
    public class LoginUserViewModel
    {
        private const string _fieldNullMessage = "{0} is required.";

        [Required(ErrorMessage = _fieldNullMessage)]
        public string Username { get; set; }

        [Required(ErrorMessage = _fieldNullMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
