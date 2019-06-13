using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Dtos
{
    public class UserFormDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
