using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Shows
{
    public class UpdateShowRequest
    {
        [Range(1, int.MaxValue)]
        public int MovieId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(typeof(decimal), "0", "99999999")]
        public decimal TicketPrice { get; set; }
    }
}