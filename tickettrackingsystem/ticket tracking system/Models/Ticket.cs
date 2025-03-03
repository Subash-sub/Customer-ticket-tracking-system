using System;
using System.ComponentModel.DataAnnotations;

namespace ticket_tracking_system.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; } // Open, Assigned, Resolved

        [Required]
        public string Priority { get; set; } // Low, Medium, High

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        // Foreign Keys
        [Required]
        public int CustomerId { get; set; }
        public int? AssignedSupportId { get; set; }

        // Navigation Properties
        public User Customer { get; set; }
        public User AssignedSupport { get; set; }
    }
}

