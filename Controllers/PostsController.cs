using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TitanBlog.Data;
using TitanBlog.Models;
using TitanBlog.Services;
using TitanBlog.Services.Interfaces;
using X.PagedList;

namespace TitanBlog.Controllers
{
    public class PostsController : Controller 
    {
        private readonly ApplicationDbContext _context;
        private readonly BasicSlugService _slugService;
        private readonly IImageService _imageService;
        private readonly SearchService _searchService;

        public PostsController(ApplicationDbContext context, BasicSlugService slugService, IImageService imageService, SearchService searchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _searchService = searchService;
        }

        public async Task<IActionResult> PostsWithTag (string text, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 4;
            var postsWithTag = await _context.Post.Where(p => p.Tags.Any(t => t.Text.ToUpper() == text)).ToPagedListAsync(pageNumber, pageSize);
            return View("Index", postsWithTag);
        }

        public async Task<IActionResult> BlogPostIndex(int? blogId)
        {
            if(blogId is null)
            {
                return NotFound();
            }

            var posts = await _context.Post.Where(posts => posts.BlogId == blogId && posts.Publish).ToListAsync();

            //get latest 3 blog posts to send to view
            var allPosts = await _context.Post.Include(p=>p.Comments).OrderByDescending(p => p.Created).ToListAsync();
            var latestPosts = allPosts.Take(3);
            ViewData["LatestPosts"] = await latestPosts.ToListAsync();

            return View(posts);
        }

        // GET: Posts
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 4;

            var posts = await _context.Post.Where(p => p.Publish)
                                           .Include(p => p.Blog)
                                           .OrderByDescending(p =>p.Created)
                                           .ToPagedListAsync(pageNumber, pageSize);
            
            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> SearchIndex(string searchStr)
        {
            //get latest 3 blog posts to send to view
            var allPosts = await _context.Post.Include(p=>p.Comments).OrderByDescending(p => p.Created).ToListAsync();
            var latestPosts = allPosts.Take(3);
            ViewData["LatestPosts"] = await latestPosts.ToListAsync();

            var posts = _searchService.ContentSearch(searchStr);
            return View("BlogPostIndex", posts);
        }

        //// GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            //Check for null or empty string
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            //eager loading
            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.Comments.Where(p=>p.Deleted == null))
                .ThenInclude(c => c.Author)
                .Include(p=>p.Tags)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            //get all tags to send to view
            var allTags = await _context.Tag.Select(t => t.Text.ToUpper()).Distinct().ToListAsync();
            ViewData["AllTags"] = allTags;
                
            //get latest 3 blog posts to send to view
            var allPosts = await _context.Post.Include(p=>p.Comments).OrderByDescending(p=>p.Created).ToListAsync();
            var latestPosts = allPosts.Take(3);
            ViewData["LatestPosts"] = await latestPosts.ToListAsync();

            //get prev and next post for same blogId
            var created = post.Created;
            var nextPost = _context.Post.Where(b => b.BlogId == post.BlogId)
                                        .Where(post => post.Publish)
                                        .Include(post => post.Tags)
                                        .OrderBy(post => post.Created)
                                        .ToList()
                                        .SkipWhile(p => p.Created < created)
                                        .Skip(1)
                                        .Take(1)
                                        .FirstOrDefault();
            if(post != null)
            {
                post.NextPost.Title = nextPost.Title;
                post.NextPost.Slug = nextPost.Slug;
                post.NextPost.Tags = nextPost.Tags;
            }
            else
            {
                post.PrevPost = null;
            }
            // end prev and next post section //

            
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,Publish,Image")] Post post, List<string> TagValues)
        {
            if (ModelState.IsValid)
            {
                var slug = _slugService.UrlFriendly(post.Title);

                if(_slugService.IsUnique(slug))
                {
                    post.Slug = slug;
                }
                else
                {
                    //I have determinded that the title is a duplicate slug...
                    ModelState.AddModelError("Title", "The Title you entered cannot be used, please try again...");
                    ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
                    return View(post);
                }

                post.Created = DateTime.Now;

                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ImageType = _imageService.ContentType(post.Image);

                foreach (var tag in TagValues)
                {
                    post.Tags.Add(new Tag()
                    {
                        Text = tag.ToUpper()
                    }) ;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            //eager loading
            var post = await _context.Post.Include(p=>p.Tags).FirstOrDefaultAsync(p => p.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,Slug,Publish,Image")] Post post, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var originalPost = await _context.Post.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);

                    if (newSlug != originalPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            originalPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "The Title you entered cannot be used, please try again...");
                            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
                            return View(post);
                        }
                    }

                    //Tags Section Start
                    //Get the Post and Include the Tags
                    _context.Tag.RemoveRange(originalPost.Tags);

                    foreach (var tag in tagValues)
                    {
                        originalPost.Tags.Add(new Tag()
                        {
                            Text = tag.ToUpper()
                        });
                    }
                    //Tags Section End

                    originalPost.Updated = DateTime.Now;

                    post.ImageType = _imageService.ContentType(post.Image);
                    post.ImageData = await _imageService.EncodeImageAsync(post.Image);

                    //_context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(originalPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Unpublished()
        {
            var applicationDbContext = _context.Post.Where(p => !p.Publish).Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}
