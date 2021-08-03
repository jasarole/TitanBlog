using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBlog.Models
{
    //IdentityUser
    public class BlogUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public string ImageType { get; set; }
        public byte[] ImageData { get; set; }

        //Navigational properties
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
