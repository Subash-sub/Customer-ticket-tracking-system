namespace ticket_tracking_system.DTOs
{
    public class TicketHistoryDto
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
    }
}
