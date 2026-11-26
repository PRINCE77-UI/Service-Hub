using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models
{
    public enum BookingStatus
    {
        Pending,
        Accepted,
        Rejected,
        Completed,
        Cancelled
    }

    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        public Review? Review { get; set; }
    }
}
