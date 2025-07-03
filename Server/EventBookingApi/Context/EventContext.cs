using System;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Context;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BookedSeat> BookedSeats { get; set; }
    public DbSet<EventImage> EventImages { get; set; }
    public DbSet<Cities> Cities { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cities>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CityName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.StateName).IsRequired().HasMaxLength(100);

            entity.HasMany(c => c.Events)
                  .WithOne(e => e.City)
                  .HasForeignKey(e => e.CityId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.IsDeleted).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();

            entity.HasMany(u => u.ManagedEvents)
                  .WithOne(e => e.Manager)
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Tickets)
                  .WithOne(t => t.User)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EventStatus).IsRequired();
            entity.Property(e => e.EventType).IsRequired();
            entity.Property(e => e.Category).IsRequired(); 
            entity.Property(e => e.CityId).IsRequired();   
            entity.Property(e => e.IsDeleted).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasMany(e => e.TicketTypes)
                  .WithOne(tt => tt.Event)
                  .HasForeignKey(tt => tt.EventId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Tickets)
                  .WithOne(t => t.Event)
                  .HasForeignKey(t => t.EventId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BookedSeats)
                  .WithOne(bs => bs.Event)
                  .HasForeignKey(bs => bs.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(tt => tt.Id);
            entity.Property(tt => tt.TypeName).IsRequired();
            entity.Property(tt => tt.Price).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(tt => tt.TotalQuantity).IsRequired();
            entity.Property(tt => tt.BookedQuantity).IsRequired();
            entity.Property(tt => tt.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Status).IsRequired();
            entity.Property(t => t.BookedAt).IsRequired();
            entity.Property(t => t.TotalPrice).HasColumnType("decimal(10,2)");

            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tickets)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Event)
                  .WithMany(e => e.Tickets)
                  .HasForeignKey(t => t.EventId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.TicketType)
                  .WithMany()
                  .HasForeignKey(t => t.TicketTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Payment)
                  .WithOne(p => p.Ticket)
                  .HasForeignKey<Payment>(p => p.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(p => p.PaymentType).IsRequired();
            entity.Property(p => p.PaymentStatus).IsRequired();
            entity.Property(p => p.PaidAt).IsRequired();
        });

        modelBuilder.Entity<BookedSeat>(entity =>
        {
            entity.HasKey(bs => bs.Id);
            entity.Property(bs => bs.SeatNumber).IsRequired();
            entity.Property(bs => bs.BookedSeatStatus).IsRequired();
            entity.Property(bs => bs.BookedAt).IsRequired();

            entity.HasOne(bs => bs.Event)
                  .WithMany(e => e.BookedSeats)
                  .HasForeignKey(bs => bs.EventId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(bs => bs.Ticket)
                  .WithMany(t => t.BookedSeats)
                  .HasForeignKey(bs => bs.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventImage>(entity =>
        {
            entity.HasKey(ei => ei.Id);
            entity.Property(ei => ei.FileName).IsRequired().HasMaxLength(255);
            entity.Property(ei => ei.FileType).IsRequired().HasMaxLength(100);
            entity.Property(ei => ei.FileContent).IsRequired();
            entity.Property(ei => ei.UploadedAt).IsRequired();

            entity.HasOne(ei => ei.Event)
                  .WithMany(e => e.Images)
                  .HasForeignKey(ei => ei.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
