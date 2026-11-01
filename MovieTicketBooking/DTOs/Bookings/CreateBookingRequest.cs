using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings
{
    public class CreateBookingRequest
    {
        [Required]
        public int ShowId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<int> SeatIds { get; set; } = new();
    }
}