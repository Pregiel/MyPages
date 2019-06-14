using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Entities
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DataCreated { get; set; }
        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DataModified { get; set; }

        public int UserId { get; set; }
        private User _userId { get; set; }
        public virtual User User
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                UserId = value.Id;
            }
        }

        public int? ParentId { get; set; }
        private Folder _parent { get; set; }
        public virtual Folder Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                ParentId = value.Id;
            }
        }
        public virtual ICollection<Folder> Childs { get; set; }
        public virtual ICollection<Page> Pages { get; set; }
    }
}
