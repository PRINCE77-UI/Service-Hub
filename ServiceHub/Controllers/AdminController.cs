using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.ViewModels;

namespace ServiceHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var allUsers = _userManager.Users.ToList();
            var providerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "ServiceProvider");
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");

            var providerUserIds = providerRole != null
                ? await _context.UserRoles.Where(ur => ur.RoleId == providerRole.Id).Select(ur => ur.UserId).ToListAsync()
                : new List<string>();

            var customerUserIds = customerRole != null
                ? await _context.UserRoles.Where(ur => ur.RoleId == customerRole.Id).Select(ur => ur.UserId).ToListAsync()
                : new List<string>();

            var allBookings = await _context.Bookings
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Category)
                .Include(b => b.Customer)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            // Monthly stats for the past 6 months
            var monthlyStats = new List<MonthlyBookingStat>();
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.UtcNow.AddMonths(-i);
                var monthBookings = allBookings.Where(b =>
                    b.BookingDate.Year == month.Year && b.BookingDate.Month == month.Month).ToList();

                monthlyStats.Add(new MonthlyBookingStat
                {
                    Month = month.ToString("MMM yyyy"),
                    Count = monthBookings.Count,
                    Revenue = monthBookings.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalAmount)
                });
            }

            // Category stats
            var categoryStats = allBookings
                .GroupBy(b => b.Service?.Category?.Name ?? "Unknown")
                .Select(g => new CategoryBookingStat { CategoryName = g.Key, Count = g.Count() })
                .OrderByDescending(c => c.Count)
                .ToList();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = allUsers.Count,
                TotalProviders = providerUserIds.Count,
                TotalCustomers = customerUserIds.Count,
                TotalBookings = allBookings.Count,
                ActiveBookings = allBookings.Count(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Accepted),
                TotalRevenue = allBookings.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalAmount),
                MonthlyStats = monthlyStats,
                CategoryStats = categoryStats,
                RecentBookings = allBookings.Take(10).ToList()
            };

            return View(model);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.OrderByDescending(u => u.CreatedAt).ToList();
            var usersWithRoles = new List<(ApplicationUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add((user, roles));
            }

            return View(usersWithRoles);
        }

        // POST: Admin/ToggleLock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"{user.FullName} has been unlocked.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["Success"] = $"{user.FullName} has been locked.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/VerifyProvider
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyProvider(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsVerified = !user.IsVerified;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.IsVerified
                ? $"{user.FullName} has been verified."
                : $"{user.FullName}'s verification has been revoked.";

            return RedirectToAction("Users");
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Services)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        // POST: Admin/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully!";
            }
            return RedirectToAction("Categories");
        }

        // POST: Admin/DeleteCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deactivated.";
            }
            return RedirectToAction("Categories");
        }

        // GET: Admin/Bookings
        public async Task<IActionResult> Bookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Provider)
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Category)
                .Include(b => b.Review)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Service)
                    .ThenInclude(s => s!.Provider)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View(reviews);
        }
    }
}
