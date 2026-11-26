namespace ServiceHub.Models.ViewModels
{
    // Auth ViewModels
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Customer";
    }

    // Home / Discovery ViewModels
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new();
        public List<Service> FeaturedServices { get; set; } = new();
        public string? SearchKeyword { get; set; }
        public string? SearchCity { get; set; }
        public int? SearchCategoryId { get; set; }
        public List<string> IndianCities { get; set; } = new();
    }

    // Service ViewModels
    public class ServiceCreateEditViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public PriceType PriceType { get; set; } = PriceType.Hourly;
        public string? City { get; set; }
        public string? ServiceArea { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        public string? WorkDays { get; set; }
        public TimeSpan? WorkStartTime { get; set; }
        public TimeSpan? WorkEndTime { get; set; }
        public List<Category> Categories { get; set; } = new();
    }

    public class ServiceDetailsViewModel
    {
        public Service Service { get; set; } = null!;
        public List<Review> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public bool CanBook { get; set; }
    }

    // Booking ViewModels
    public class BookingCreateViewModel
    {
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Today.AddDays(1);
        public string StartTime { get; set; } = "09:00";
        public string EndTime { get; set; } = "10:00";
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // Customer Dashboard ViewModels
    public class CustomerDashboardViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<Booking> AllBookings { get; set; } = new();
        public List<Booking> ActiveBookings { get; set; } = new();
        public List<Booking> CompletedBookings { get; set; } = new();
        public List<Booking> CancelledOrRejectedBookings { get; set; } = new();
        public List<Booking> PendingReviewBookings { get; set; } = new();
        public List<Review> MyReviews { get; set; } = new();
    }

    // Provider Dashboard ViewModels
    public class ProviderDashboardViewModel
    {
        public List<Service> MyServices { get; set; } = new();
        public List<Booking> PendingBookings { get; set; } = new();
        public List<Booking> AcceptedBookings { get; set; } = new();
        public List<Booking> CompletedBookings { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
    }

    // Admin Dashboard ViewModels
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProviders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<MonthlyBookingStat> MonthlyStats { get; set; } = new();
        public List<CategoryBookingStat> CategoryStats { get; set; } = new();
        public List<Booking> RecentBookings { get; set; } = new();
    }

    public class MonthlyBookingStat
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CategoryBookingStat
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    // Review ViewModel
    public class ReviewCreateViewModel
    {
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public Booking? Booking { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
