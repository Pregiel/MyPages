using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Models
{
    public class RegisterUserViewModel
    {
        private const string _fieldLengthMessage = "{0} length must be between {2} and {1}.";
        private const string _fieldNullMessage = "{0} is required.";
        private const string _fieldCompareMessage = "{0} and {1} do not match.";

        [Required(ErrorMessage = _fieldNullMessage)]
        [StringLength(32, ErrorMessage = _fieldLengthMessage, MinimumLength = 4)]
        public string Username { get; set; }

        [Required(ErrorMessage = _fieldNullMessage)]
        [StringLength(32, ErrorMessage = _fieldLengthMessage, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = _fieldNullMessage)]
        [StringLength(32, ErrorMessage = _fieldLengthMessage, MinimumLength = 6)]
        [Compare("Password", ErrorMessage = _fieldCompareMessage)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
