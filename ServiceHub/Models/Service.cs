using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models
{
    public enum PriceType
    {
        Hourly,
        Fixed
    }

    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string ProviderId { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public PriceType PriceType { get; set; } = PriceType.Hourly;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? ServiceArea { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Work Schedule (JSON or comma-separated days)
        public string? WorkDays { get; set; }
        public TimeSpan? WorkStartTime { get; set; }
        public TimeSpan? WorkEndTime { get; set; }

        // Navigation
        [ForeignKey("ProviderId")]
        public ApplicationUser? Provider { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Computed
        [NotMapped]
        public double AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : 0;

        [NotMapped]
        public int ReviewCount => Reviews?.Count ?? 0;
    }
}
