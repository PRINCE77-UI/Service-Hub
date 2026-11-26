using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.ViewModels;

namespace ServiceHub.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var service = await _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Category)
                .Include(s => s.Reviews)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (service == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var canBook = currentUser != null && User.IsInRole("Customer");

            var model = new ServiceDetailsViewModel
            {
                Service = service,
                Reviews = service.Reviews?.OrderByDescending(r => r.ReviewDate).ToList() ?? new(),
                AverageRating = service.Reviews?.Any() == true ? service.Reviews.Average(r => r.Rating) : 0,
                CanBook = canBook
            };

            return View(model);
        }

        // GET: Services (browse all)
        public async Task<IActionResult> Browse(string? keyword, string? city, int? categoryId, string? sort)
        {
            var query = _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Category)
                .Include(s => s.Reviews)
                .Where(s => s.IsActive);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(s => s.Title.Contains(keyword) || s.Description!.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(s => s.City!.Contains(city));

            if (categoryId.HasValue)
                query = query.Where(s => s.CategoryId == categoryId.Value);

            var services = await query.ToListAsync();

            // Sort
            services = sort switch
            {
                "price_asc" => services.OrderBy(s => s.Price).ToList(),
                "price_desc" => services.OrderByDescending(s => s.Price).ToList(),
                "rating" => services.OrderByDescending(s => s.AverageRating).ToList(),
                _ => services.OrderByDescending(s => s.CreatedAt).ToList()
            };

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.IndianCities = IndianCities.All;
            ViewBag.Keyword = keyword;
            ViewBag.City = city;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;

            return View(services);
        }
    }
}
