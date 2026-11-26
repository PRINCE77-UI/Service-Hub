using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Models;

namespace ServiceHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser
            builder.Entity<ApplicationUser>(e =>
            {
                e.Property(u => u.FullName).HasMaxLength(200);
                e.Property(u => u.City).HasMaxLength(100);
                e.Property(u => u.Address).HasMaxLength(500);
            });

            // Service
            builder.Entity<Service>(e =>
            {
                e.HasOne(s => s.Provider)
                    .WithMany(u => u.Services)
                    .HasForeignKey(s => s.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(s => s.Category)
                    .WithMany(c => c.Services)
                    .HasForeignKey(s => s.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(s => s.PriceType)
                    .HasConversion<string>();
            });

            // Booking
            builder.Entity<Booking>(e =>
            {
                e.HasOne(b => b.Customer)
                    .WithMany(u => u.CustomerBookings)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.Service)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(b => b.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(b => b.Status)
                    .HasConversion<string>();
            });

            // Review
            builder.Entity<Review>(e =>
            {
                e.HasOne(r => r.Booking)
                    .WithOne(b => b.Review)
                    .HasForeignKey<Review>(r => r.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Customer)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Service)
                    .WithMany(s => s.Reviews)
                    .HasForeignKey(r => r.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Indexes for performance
            builder.Entity<Booking>()
                .HasIndex(b => new { b.ServiceId, b.BookingDate, b.Status });

            builder.Entity<Service>()
                .HasIndex(s => new { s.CategoryId, s.IsActive });
        }
    }
}
