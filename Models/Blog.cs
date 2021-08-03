using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Models
{
    public class Blog
    {

        public int Id { get; set; }

        [Required]
        [Display(Name="Name (Must be at least 2 characters, 100 max)")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Display(Name="Description (500 characters or less)")]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Description { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name="Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }

        //Adding the properties for describing any images being used
        public string ImageType { get; set; }
        public byte[] ImageData { get; set; }

        [Display(Name="Select Image")]
        [NotMapped]
        public IFormFile Image { get; set; }

        //Navigational Properties - These properties allow us to move from one object to another related object
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
}
