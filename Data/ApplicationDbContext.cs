using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TitanBlog.Models;

namespace TitanBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TitanBlog.Models.Blog> Blog { get; set; }
        public DbSet<TitanBlog.Models.Post> Post { get; set; }
    }
}
