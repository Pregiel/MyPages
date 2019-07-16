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

        public int? ParentId { get; set; }
        private Page _parent { get; set; }
        public virtual Page Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                if (value != null)
                    ParentId = value.Id;
            }
        }
        public virtual ICollection<Page> Childs { get; set; }
    }
}
