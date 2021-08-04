using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Models
{
    public class Post
    {
        public int Id { get; set; }

        //Foreign Key (FK) - A combination of the Class Name (Blog) and the Primary Key (PK) property
        [Display(Name = "Blog Name")]
        public int BlogId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Abstract { get; set; }
    
        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Created")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Updated")]
        public DateTime? Updated { get; set; }

        public string Slug { get; set; }

        //Adding the properties for describing any images being used
        public string ImageType { get; set; }
        public byte[] ImageData { get; set; }

        [Display(Name = "Select Image")]
        [NotMapped]
        public IFormFile Image { get; set; }

        //Navigation Properties
        public virtual Blog Blog { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Tag> Tagss { get; set; } = new HashSet<Tag>();

    }
}
