using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Entities
{
    public class Page
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DataCreated { get; set; }
        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DataModified { get; set; }

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
