//namespace ticket_tracking_system.DTOs
//{
//    public class AttachmentDto
//    {
//        public int Id { get; set; }
//        public string FileName { get; set; }
//        public string FilePath { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public int TicketId { get; set; }
//        public int UserId { get; set; }
//    }
//}


using System.ComponentModel.DataAnnotations;
using ticket_tracking_system.Models;

public class Attachment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string FileName { get; set; }
    [Required]
    public string FilePath { get; set; }
    public DateTime CreatedAt { get; set; }
    // Foreign Keys
    [Required]
    public int TicketId { get; set; }
    [Required]
    public int UserId { get; set; }
    // Navigation Properties
    public Ticket Ticket { get; set; }
    public User User { get; set; }
    public bool IsScreenshot { get; set; } // New property to indicate if the attachment is a screenshot
}
