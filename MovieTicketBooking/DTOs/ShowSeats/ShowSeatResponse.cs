namespace MovieTicketBooking.DTOs.ShowSeats
{
    public class ShowSeatResponse
    {
        public int SeatId { get; set; }

        public string RowName { get; set; } = string.Empty;

        public int SeatNumber { get; set; }

        public string SeatLabel { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}