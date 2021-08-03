using System;
using System.ComponentModel.DataAnnotations;

namespace TitanBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Display(Name="Post Name")]
        public int PostId { get; set; }

        public string AuthorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }

        [Required]
        [Display(Name = "(500 characters or less)")]
        [StringLength(500, ErrorMessage ="The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string Body { get; set; }

        //Navigation Properties
        public virtual Post Post { get; set; }
        public virtual BlogUser Author { get; set; }
    }
}