using System;
using System.ComponentModel.DataAnnotations;

namespace ticket_tracking_system.Models
{
    public class TicketAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public int AssignedByAdminId { get; set; } // Admin who assigned the ticket

        [Required]
        public int AssignedToSupportId { get; set; } // Support person

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Ticket Ticket { get; set; }
        public User AssignedByAdmin { get; set; }
        public User AssignedToSupport { get; set; }
    }
}

