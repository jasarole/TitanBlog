using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Tag Text")]
        public string Text { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
}
