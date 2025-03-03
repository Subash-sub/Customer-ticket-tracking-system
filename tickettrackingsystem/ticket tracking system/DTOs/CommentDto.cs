namespace ticket_tracking_system.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
    }
}
