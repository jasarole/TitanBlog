using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Models
{
    public class Contact
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name ="Enter Your Name")]
        public string emailName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name ="Email Address")]
        public string emailAddress { get; set; }
        
        [Required]
        [StringLength(120, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "Subject")]
        public string emailSubject { get; set; }
        
        [Required]
        [Display(Name = "Email Message")]
        public string emailBody { get; set; }

    }
}
