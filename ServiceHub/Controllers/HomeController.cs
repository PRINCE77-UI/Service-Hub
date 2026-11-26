using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.ViewModels;

namespace ServiceHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword, string? city, int? categoryId)
        {
            var servicesQuery = _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Category)
                .Include(s => s.Reviews)
                .Where(s => s.IsActive);

            if (!string.IsNullOrWhiteSpace(keyword))
                servicesQuery = servicesQuery.Where(s =>
                    s.Title.Contains(keyword) ||
                    s.Description!.Contains(keyword) ||
                    s.Provider!.FullName.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(city))
                servicesQuery = servicesQuery.Where(s => s.City!.Contains(city));

            if (categoryId.HasValue)
                servicesQuery = servicesQuery.Where(s => s.CategoryId == categoryId.Value);

            var model = new HomeViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                FeaturedServices = await servicesQuery.Take(12).ToListAsync(),
                SearchKeyword = keyword,
                SearchCity = city,
                SearchCategoryId = categoryId,
                IndianCities = IndianCities.All
            };

            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
