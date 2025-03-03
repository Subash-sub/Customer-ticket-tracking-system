using Microsoft.EntityFrameworkCore;
using ticket_tracking_system.Models;

namespace ticket_tracking_system.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAssignment> TicketAssignments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure Ticket relationships
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Customer)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedSupport)
                .WithMany()
                .HasForeignKey(t => t.AssignedSupportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TicketAssignment relationships
            modelBuilder.Entity<TicketAssignment>()
                .HasOne(ta => ta.Ticket)
                .WithMany()
                .HasForeignKey(ta => ta.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(ta => ta.AssignedByAdmin)
                .WithMany()
                .HasForeignKey(ta => ta.AssignedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(ta => ta.AssignedToSupport)
                .WithMany()
                .HasForeignKey(ta => ta.AssignedToSupportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Comment relationships
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany()
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Attachment relationships
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Ticket)
                .WithMany()
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TicketHistory relationships
            modelBuilder.Entity<TicketHistory>()
                .HasOne(th => th.Ticket)
                .WithMany()
                .HasForeignKey(th => th.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketHistory>()
                .HasOne(th => th.User)
                .WithMany()
                .HasForeignKey(th => th.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
