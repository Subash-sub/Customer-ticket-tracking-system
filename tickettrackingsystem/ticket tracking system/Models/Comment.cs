using System;
using System.ComponentModel.DataAnnotations;

namespace ticket_tracking_system.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int UserId { get; set; }

        // Navigation Properties
        public Ticket Ticket { get; set; }
        public User User { get; set; }
    }
}

