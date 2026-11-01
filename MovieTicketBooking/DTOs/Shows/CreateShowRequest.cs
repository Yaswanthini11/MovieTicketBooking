using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Shows
{
    public class CreateShowRequest
    {
        [Range(1, int.MaxValue)]
        public int MovieId { get; set; }

        [Range(1, int.MaxValue)]
        public int ScreenId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(typeof(decimal), "0", "99999999")]
        public decimal TicketPrice { get; set; }
    }
}