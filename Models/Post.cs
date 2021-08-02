using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        //This will be a standardized verson of the Title
        //Title = "I am a Post"
        //Slug = i-am-a-post
        public string Slug { get; set; }

    }
}
