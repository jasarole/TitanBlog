using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Models
{
    /// <summary>
    /// The Blog Post Model respresents an individual Post for a given Blog
    /// </summary>
    public class Post
    {
        /// <summary>
        /// The Primary Key
        /// </summary>
        public int Id { get; set; }

        //Foreign Key (FK) - A combination of the Class Name (Blog) and the Primary Key (PK) property
        /// <summary>
        /// The Foreign Key (Blog)
        /// </summary>
        [Display(Name = "Blog Name")]
        public int BlogId { get; set; }

        /// <summary>
        /// The Post Title
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        [Display(Name ="Post Title")]
        public string Title { get; set; }

        /// <summary>
        /// Intended to hook the reader without forcing them to read the entire Post
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Abstract { get; set; }
    
        /// <summary>
        /// The main body content of the Post
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Creation date of the Post
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Date Created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// If the Post was updated then this will show a date
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Date Updated")]
        public DateTime? Updated { get; set; }

        //Is the Post ready to be published to the Blog?
        /// <summary>
        /// Is the Post ready to be published to the Blog?
        /// </summary>
        [Display(Name ="Ready to publish?")]
        public bool Publish { get; set; }

        /// <summary>
        /// A shortened URL to the Post (known as a Slug)
        /// </summary>
        public string Slug { get; set; }

        //Adding the properties for describing any images being used
        /// <summary>
        /// The Type of Image File (.gif, .jpg, etc...)
        /// </summary>
        public string ImageType { get; set; }
        /// <summary>
        /// The Data from the Image File
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// The Image File itself...
        /// </summary>
        [Display(Name = "Select Image")]
        [NotMapped]
        public IFormFile Image { get; set; }

        //Navigation Properties
        public virtual Blog Blog { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

    }
}
