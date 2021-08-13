using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TitanBlog.Data;
using TitanBlog.Models;

namespace TitanBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Moderator, Administrator")]
        public async Task<IActionResult> UnmoderatedIndex()
        {
            var comments = await _context.Comment.Where(c => c.Moderated == null && c.Deleted == null).ToListAsync();
            return View("Index", comments);
        }

        [Authorize(Roles = "Moderator, Administrator")]
        public async Task<IActionResult> ModeratedIndex()
        {
            var comments = await _context.Comment.Where(c => c.Moderated != null && c.Deleted == null).ToListAsync();
            return View("Index", comments);
        }

        [Authorize(Roles = "Moderator, Administrator")]
        public async Task<IActionResult> DeletedIndex()
        {
            var comments = await _context.Comment.Where(c => c.Deleted != null).ToListAsync();
            return View("Index", comments);
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comment.Include(c => c.Author).Include(c => c.Post);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        //public IActionResult Create()
        //{
        //    ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["PostId"] = new SelectList(_context.Post, "Id", "Abstract");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Body")] Comment comment, string slug)
        {
            if (ModelState.IsValid)
            {
                comment.Created = DateTime.Now;
                comment.AuthorId = _userManager.GetUserId(User);


                _context.Add(comment);
                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", "Posts", new { slug }, "postComments");
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,AuthorId,Created,Body")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.Updated = DateTime.Now;

                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Moderate(int id, [Bind("Id,ModeratedBody,ModerationReason")] Comment comment, string slug)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            try
            {
                comment.Moderated = DateTime.Now;
                comment.ModeratorId = _userManager.GetUserId(User);

                _context.Comment.Attach(comment);

                _context.Entry(comment).Property(x => x.ModeratedBody).IsModified = true;
                _context.Entry(comment).Property(x => x.ModerationReason).IsModified = true;
                _context.Entry(comment).Property(x => x.Moderated).IsModified = true;
                _context.Entry(comment).Property(x => x.ModeratorId).IsModified = true;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;                      
                }
            }

                return RedirectToAction("Details", "Posts", new { slug }, "postComments");

            }


        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string slug)
        {
            var comment = await _context.Comment.FindAsync(id);
            _context.Comment.Remove(comment);
            comment.Deleted = DateTime.Now;
            _context.Update(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Post", new { slug }, "postComments");

        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
