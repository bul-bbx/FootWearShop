// Controllers/HomeController.cs
using FootFetish.Data;
using FootFetish.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FootFetish.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            ViewBag.Categories = await _context.Categories.ToListAsync(); 
            var products = await query.ToListAsync(); 

            return View(products);
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

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products) 
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View("~/Views/Categories/Details.cshtml", category);
        }

        public IActionResult SearchResults(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return View(new List<Product>()); 
            }

            var searchTerm = search.ToLower(); 

            var filteredProducts = _context.Products
                                           .Where(p => p.Name.ToLower().Contains(searchTerm)) 
                                           .Take(5) 
                                           .ToList();

            ViewBag.SearchTerm = search; 
            return View(filteredProducts); 
        }
    }
}
