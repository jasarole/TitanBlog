using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBlog.Data;
using TitanBlog.Models;

namespace TitanBlog.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        //This will be where I write the GetTopXPosts
        //Provide an endpoint to the user (localhost:5001/api/PostsService/GetTopXPosts/4)

        /// <summary>
        /// Allow a consumer to request the latest X number of Blog Posts
        /// </summary>
        /// <remarks>
        /// This is what it looks like when you use the remarks tag
        /// </remarks>
        /// <param name="num">The number of Posts you want</param>
        /// <returns></returns>
        [HttpGet("/GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetTopXPosts(int num)
        {
            return await _context.Post.OrderByDescending(p => p.Created).Take(num).ToListAsync();
        }



    }
}
