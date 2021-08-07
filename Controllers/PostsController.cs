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

namespace TitanBlog.Controllers
{
    public class PostsController : Controller 
    {
        private readonly ApplicationDbContext _context;
        private readonly BasicSlugService _slugService;

        public PostsController(ApplicationDbContext context, BasicSlugService slugService)
        {
            _context = context;
            _slugService = slugService;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Post.Include(p => p.Blog);
            var applicationDbContext = _context.Post.Where(p => p.Publish).Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,Publish")] Post post)
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

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,Slug,Publish")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);

                    if (newSlug != post.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            post.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "The Title you entered cannot be used, please try again...");
                            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
                            return View(post);
                        }
                    }

                    post.Updated = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
