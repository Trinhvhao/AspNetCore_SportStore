using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_Project.Areas.Admin.Controllers;

[Area("Admin")]
public class UserADController : Controller
{
    private readonly SportStoreDb2Context _context;

    public UserADController(SportStoreDb2Context context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(user);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public IActionResult Edit(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(user);
    }

    public IActionResult Detail(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}