using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.ViewModels;
using ServiceHub.Services;

namespace ServiceHub.Controllers
{
    [Authorize(Roles = "ServiceProvider")]
    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ProviderController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Provider/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var services = await _context.Services
                .Include(s => s.Category)
                .Where(s => s.ProviderId == user.Id)
                .ToListAsync();

            var serviceIds = services.Select(s => s.Id).ToList();

            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Category)
                .Where(b => serviceIds.Contains(b.ServiceId))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var model = new ProviderDashboardViewModel
            {
                MyServices = services,
                PendingBookings = bookings.Where(b => b.Status == BookingStatus.Pending).ToList(),
                AcceptedBookings = bookings.Where(b => b.Status == BookingStatus.Accepted).ToList(),
                CompletedBookings = bookings.Where(b => b.Status == BookingStatus.Completed).ToList(),
                TotalRevenue = bookings.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalAmount),
                TotalBookings = bookings.Count
            };

            return View(model);
        }

        // GET: Provider/CreateService
        public async Task<IActionResult> CreateService()
        {
            var model = new ServiceCreateEditViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync()
            };
            return View(model);
        }

        // POST: Provider/CreateService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(ServiceCreateEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            model.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var service = new Service
            {
                ProviderId = user.Id,
                CategoryId = model.CategoryId,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                PriceType = model.PriceType,
                City = model.City,
                ServiceArea = model.ServiceArea,
                IsActive = model.IsActive,
                ImageUrl = model.ImageUrl,
                WorkDays = model.WorkDays,
                WorkStartTime = model.WorkStartTime,
                WorkEndTime = model.WorkEndTime,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Service listing created successfully!";
            return RedirectToAction("Dashboard");
        }

        // GET: Provider/EditService/5
        public async Task<IActionResult> EditService(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == user!.Id);

            if (service == null) return NotFound();

            var model = new ServiceCreateEditViewModel
            {
                Id = service.Id,
                CategoryId = service.CategoryId,
                Title = service.Title,
                Description = service.Description,
                Price = service.Price,
                PriceType = service.PriceType,
                City = service.City,
                ServiceArea = service.ServiceArea,
                IsActive = service.IsActive,
                ImageUrl = service.ImageUrl,
                WorkDays = service.WorkDays,
                WorkStartTime = service.WorkStartTime,
                WorkEndTime = service.WorkEndTime,
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync()
            };

            return View(model);
        }

        // POST: Provider/EditService/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, ServiceCreateEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == user!.Id);

            if (service == null) return NotFound();

            model.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            if (!ModelState.IsValid) return View(model);

            service.CategoryId = model.CategoryId;
            service.Title = model.Title;
            service.Description = model.Description;
            service.Price = model.Price;
            service.PriceType = model.PriceType;
            service.City = model.City;
            service.ServiceArea = model.ServiceArea;
            service.IsActive = model.IsActive;
            service.ImageUrl = model.ImageUrl;
            service.WorkDays = model.WorkDays;
            service.WorkStartTime = model.WorkStartTime;
            service.WorkEndTime = model.WorkEndTime;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Service updated successfully!";
            return RedirectToAction("Dashboard");
        }

        // POST: Provider/AcceptBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptBooking(int id)
        {
            await UpdateBookingStatus(id, BookingStatus.Accepted, "Booking accepted.");
            return RedirectToAction("Dashboard");
        }

        // POST: Provider/RejectBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBooking(int id)
        {
            await UpdateBookingStatus(id, BookingStatus.Rejected, "Booking rejected.");
            return RedirectToAction("Dashboard");
        }

        // POST: Provider/CompleteBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            await UpdateBookingStatus(id, BookingStatus.Completed, "Booking marked as completed.");
            return RedirectToAction("Dashboard");
        }

        private async Task UpdateBookingStatus(int bookingId, BookingStatus newStatus, string successMessage)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.Service!.ProviderId == user!.Id);

            if (booking == null) return;

            booking.Status = newStatus;
            await _context.SaveChangesAsync();
            TempData["Success"] = successMessage;

            // Send email notification
            if (booking.Customer?.Email != null)
            {
                _ = _emailService.SendBookingStatusUpdateAsync(
                    booking.Customer.Email,
                    booking.Customer.FullName,
                    booking.Service?.Title ?? "Service",
                    newStatus.ToString());
            }
        }

        // GET: Provider/Calendar
        public async Task<IActionResult> Calendar()
        {
            var user = await _userManager.GetUserAsync(User);
            var serviceIds = await _context.Services
                .Where(s => s.ProviderId == user!.Id)
                .Select(s => s.Id)
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .Where(b => serviceIds.Contains(b.ServiceId)
                    && (b.Status == BookingStatus.Accepted || b.Status == BookingStatus.Pending)
                    && b.BookingDate >= DateTime.Today)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }
    }
}
