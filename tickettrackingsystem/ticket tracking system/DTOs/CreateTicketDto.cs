using System.ComponentModel.DataAnnotations;

namespace ticket_tracking_system.DTOs
{
    public class CreateTicketDto
    {
        
            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public string Priority { get; set; }
        

    }
}
