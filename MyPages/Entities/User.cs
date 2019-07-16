using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public int MainPageId { get; set; }
        private Page _mainPage { get; set; }
        public virtual Page MainPage
        {
            get
            {
                return _mainPage;
            }
            set
            {
                _mainPage = value;
                MainPageId = value.Id;
            }
        }
    }
}
