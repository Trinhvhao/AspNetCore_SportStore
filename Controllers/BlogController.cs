using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_Project.Controllers;

public class BlogController : Controller
{
        private readonly SportStoreDb2Context _context;
        public BlogController(SportStoreDb2Context context)
        {
            _context = context;
        }
    // GET
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Create([Bind("Title, Content, Author, ImageUrl, CategoryId, Tags")] Blog blog)
    {
        if (ModelState.IsValid)
        {
            blog.CreatedAt = DateTime.Now; 

            _context.Blogs.Add(blog);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index)); 
        }
        return View(blog);
    }
}