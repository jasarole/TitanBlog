using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBlog.Data;
using TitanBlog.Models;

namespace TitanBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> ContentSearch(string searchStr)
        {
            var posts = _context.Post.Where(p => p.Publish);

            if (!string.IsNullOrEmpty(searchStr))
            {
                searchStr = searchStr.ToLower();
                posts = posts.Where(p =>
                     p.Title.ToLower().Contains(searchStr) ||
                     p.Abstract.ToLower().Contains(searchStr) ||
                     p.Content.ToLower().Contains(searchStr) ||
                     p.Comments.Any(c=>
                        c.Body.ToLower().Contains(searchStr) ||
                        c.ModeratedBody.ToLower().Contains(searchStr) ||
                        c.Author.FirstName.ToLower().Contains(searchStr) ||
                        c.Author.LastName.ToLower().Contains(searchStr) ||
                        c.Author.Email.ToLower().Contains(searchStr))
                    );
            }
            return posts.OrderByDescending(p => p.Created);

        }

    }
}
