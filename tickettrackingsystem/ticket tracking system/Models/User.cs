using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ticket_tracking_system.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonIgnore]
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // Customer, Admin, Support

        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}

