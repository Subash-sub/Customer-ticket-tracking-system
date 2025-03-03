namespace ticket_tracking_system.DTOs
{
    public class AddCommentDto
    {
        public string Text { get; set; }
        public int TicketId { get; set; } // Add this property to fix the error
    }
}
