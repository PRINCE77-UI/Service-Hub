using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public int ServiceId { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
    }
}
