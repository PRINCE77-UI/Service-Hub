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
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdoNetBookingRepository _bookingRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public BookingsController(
            ApplicationDbContext context,
            IAdoNetBookingRepository bookingRepo,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _bookingRepo = bookingRepo;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Bookings/Create/5 (5 = serviceId)
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(int serviceId)
        {
            var service = await _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive);

            if (service == null) return NotFound();

            var model = new BookingCreateViewModel
            {
                ServiceId = serviceId,
                Service = service,
                BookingDate = DateTime.Today.AddDays(1),
                StartTime = "09:00",
                EndTime = "10:00",
                TotalAmount = service.Price
            };

            return View(model);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            var service = await _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == model.ServiceId);

            if (service == null) return NotFound();
            model.Service = service;

            // Parse time strings
            if (!TimeSpan.TryParse(model.StartTime, out var startTime) ||
                !TimeSpan.TryParse(model.EndTime, out var endTime))
            {
                ModelState.AddModelError("", "Invalid time format.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Calculate total amount
            decimal totalAmount = service.PriceType == PriceType.Hourly
                ? service.Price * (decimal)(endTime - startTime).TotalHours
                : service.Price;

            // Use ADO.NET with conflict detection
            var (success, message, bookingId) = await _bookingRepo.CreateBookingWithConflictCheckAsync(
                user.Id, service.Id, model.BookingDate, startTime, endTime, totalAmount, model.Notes);

            if (!success)
            {
                TempData["Error"] = message;
                return View(model);
            }

            // Send confirmation email (non-critical)
            _ = _emailService.SendBookingConfirmationAsync(
                user.Email!, user.FullName, service.Title,
                model.BookingDate, startTime, endTime);

            TempData["Success"] = "Booking created successfully! The provider will confirm shortly.";
            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/MyBookings
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var bookings = await _context.Bookings
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Provider)
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Category)
                .Include(b => b.Review)
                .Where(b => b.CustomerId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var model = new CustomerDashboardViewModel
            {
                CustomerName = user.FullName,
                AllBookings = bookings,
                ActiveBookings = bookings.Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Accepted).ToList(),
                CompletedBookings = bookings.Where(b => b.Status == BookingStatus.Completed).ToList(),
                CancelledOrRejectedBookings = bookings.Where(b => b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Rejected).ToList(),
                PendingReviewBookings = bookings.Where(b => b.Status == BookingStatus.Completed && b.Review == null).ToList(),
                MyReviews = await _context.Reviews
                    .Include(r => r.Service)
                    .Where(r => r.CustomerId == user.Id)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id && b.CustomerId == user.Id);

            if (booking == null) return NotFound();

            if (booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Accepted)
            {
                booking.Status = BookingStatus.Cancelled;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking cancelled successfully.";
            }
            else
            {
                TempData["Error"] = "This booking cannot be cancelled.";
            }

            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/Review/5
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Review(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _context.Bookings
                .Include(b => b.Service)
                    .ThenInclude(s => s!.Provider)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.CustomerId == user!.Id);

            if (booking == null || booking.Status != BookingStatus.Completed || booking.Review != null)
                return NotFound();

            var model = new ReviewCreateViewModel
            {
                BookingId = booking.Id,
                ServiceId = booking.ServiceId,
                Booking = booking
            };

            return View(model);
        }

        // POST: Bookings/Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Review(ReviewCreateViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId && b.CustomerId == user!.Id);

            if (booking == null || booking.Review != null) return NotFound();

            var review = new Review
            {
                BookingId = model.BookingId,
                ServiceId = booking.ServiceId,
                CustomerId = user!.Id,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thank you for your review!";
            return RedirectToAction("MyBookings");
        }
    }
}
