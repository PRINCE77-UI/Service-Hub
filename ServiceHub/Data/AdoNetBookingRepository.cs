using Microsoft.Data.SqlClient;
using ServiceHub.Models;

namespace ServiceHub.Data
{
    public interface IAdoNetBookingRepository
    {
        Task<(bool Success, string Message, int BookingId)> CreateBookingWithConflictCheckAsync(
            string customerId,
            int serviceId,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            decimal totalAmount,
            string? notes);
    }

    public class AdoNetBookingRepository : IAdoNetBookingRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<AdoNetBookingRepository> _logger;

        public AdoNetBookingRepository(IConfiguration configuration, ILogger<AdoNetBookingRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        public async Task<(bool Success, string Message, int BookingId)> CreateBookingWithConflictCheckAsync(
            string customerId,
            int serviceId,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            decimal totalAmount,
            string? notes)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);

            try
            {
                // Step 1: Check for booking conflicts
                var conflictCheckSql = @"
                    SELECT COUNT(1) 
                    FROM Bookings 
                    WHERE ServiceId = @ServiceId 
                      AND CAST(BookingDate AS DATE) = CAST(@BookingDate AS DATE)
                      AND Status IN ('Pending', 'Accepted')
                      AND @StartTime < EndTime 
                      AND @EndTime > StartTime";

                using var conflictCmd = new SqlCommand(conflictCheckSql, connection, transaction);
                conflictCmd.Parameters.AddWithValue("@ServiceId", serviceId);
                conflictCmd.Parameters.AddWithValue("@BookingDate", bookingDate.Date);
                conflictCmd.Parameters.AddWithValue("@StartTime", startTime);
                conflictCmd.Parameters.AddWithValue("@EndTime", endTime);

                var conflictCount = (int)(await conflictCmd.ExecuteScalarAsync() ?? 0);

                if (conflictCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (false, "This time slot is already booked. Please choose a different time.", -1);
                }

                // Step 2: Validate end time is after start time
                if (endTime <= startTime)
                {
                    await transaction.RollbackAsync();
                    return (false, "End time must be after start time.", -1);
                }

                // Step 3: Insert the booking
                var insertSql = @"
                    INSERT INTO Bookings 
                        (CustomerId, ServiceId, BookingDate, StartTime, EndTime, Status, TotalAmount, Notes, CreatedAt)
                    OUTPUT INSERTED.Id
                    VALUES 
                        (@CustomerId, @ServiceId, @BookingDate, @StartTime, @EndTime, 'Pending', @TotalAmount, @Notes, @CreatedAt)";

                using var insertCmd = new SqlCommand(insertSql, connection, transaction);
                insertCmd.Parameters.AddWithValue("@CustomerId", customerId);
                insertCmd.Parameters.AddWithValue("@ServiceId", serviceId);
                insertCmd.Parameters.AddWithValue("@BookingDate", bookingDate.Date);
                insertCmd.Parameters.AddWithValue("@StartTime", startTime);
                insertCmd.Parameters.AddWithValue("@EndTime", endTime);
                insertCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                insertCmd.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                var newId = (int)(await insertCmd.ExecuteScalarAsync() ?? throw new Exception("Insert failed."));

                await transaction.CommitAsync();
                _logger.LogInformation("Booking {BookingId} created successfully for Service {ServiceId}.", newId, serviceId);

                return (true, "Booking created successfully!", newId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating booking for Service {ServiceId}.", serviceId);
                return (false, $"An error occurred: {ex.Message}", -1);
            }
        }
    }
}
