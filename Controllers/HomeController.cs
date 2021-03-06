using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBlog.Data;
using TitanBlog.Models;
using X.PagedList;

namespace TitanBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //get latest 3 blog posts to send to view
            var allPosts = await _context.Post.Include(p => p.Comments).OrderByDescending(p => p.Created).ToListAsync();
            var latestPosts = allPosts.Take(3);
            ViewData["LatestPosts"] = await latestPosts.ToListAsync();
            var blogs =  await _context.Blog.ToListAsync();
            return View(blogs);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("emailName, emailAddress, emailSubject, emailBody")] Contact contact)
        {
            await _emailSender.SendEmailAsync(_configuration["MailSettings:Email"], contact.emailSubject,
                $"You have received an email from {contact.emailName}. <br /> <br /> <hr> <br />{contact.emailBody} <br /> <br /> <hr> <br /> Please contact at {contact.emailAddress}");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
