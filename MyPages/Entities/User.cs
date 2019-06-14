﻿using System;
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

        public int FolderId { get; set; }
        private Folder _folder { get; set; }
        public virtual Folder Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
                FolderId = value.Id;
            }
        }
    }
}
