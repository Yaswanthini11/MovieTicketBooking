using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;
using System.Reflection.Emit;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }

        public DbSet<Screen> Screens { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<Show> Shows { get; set; }

        public DbSet<ShowSeat> ShowSeats { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.MovieId);

                entity.Property(m => m.Title)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(m => m.Description)
                    .HasMaxLength(500);

                entity.Property(m => m.Language)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(m => m.Genre)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.ToTable("Theatres");

                entity.HasKey(t => t.TheatreId);

                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(t => t.Location)
                      .IsRequired()
                      .HasMaxLength(200);
            });

            modelBuilder.Entity<Screen>(entity =>
            {
                entity.ToTable("Screens");

                entity.HasKey(s => s.ScreenId);

                entity.Property(s => s.ScreenName)
                    .IsRequired()
                    .HasMaxLength(50);

                // Screen belongs to a Theatre
                entity.HasOne<Theatre>()
                    .WithMany()
                    .HasForeignKey(s => s.TheatreId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Screen names must be unique within a theatre
                entity.HasIndex(s => new
                {
                    s.TheatreId,
                    s.ScreenName
                }).IsUnique();
            });

            // Seat configuration
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.ToTable("Seats");

                entity.HasKey(s => s.SeatId);

                entity.Property(s => s.RowName)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(s => s.SeatNumber)
                    .IsRequired();

                entity.Property(s => s.SeatType)
                    .IsRequired()
                    .HasMaxLength(30);

                // Seat belongs to one Screen
                entity.HasOne<Screen>()
                    .WithMany()
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate seat positions within a screen
                entity.HasIndex(s => new
                {
                    s.ScreenId,
                    s.RowName,
                    s.SeatNumber
                }).IsUnique();

                // Seat number must be greater than zero
                entity.ToTable(table =>
                    table.HasCheckConstraint(
                        "CK_Seats_SeatNumber",
                        "[SeatNumber] > 0"));
            });

            modelBuilder.Entity<Show>(entity =>
            {
                entity.ToTable("Shows", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Shows_EndTime",
                        "[EndTime] > [StartTime]");

                    table.HasCheckConstraint(
                        "CK_Shows_TicketPrice",
                        "[TicketPrice] >= 0");
                });

                entity.HasKey(s => s.ShowId);

                entity.Property(s => s.StartTime)
                    .IsRequired();

                entity.Property(s => s.EndTime)
                    .IsRequired();

                entity.Property(s => s.TicketPrice)
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.HasOne<Movie>()
                    .WithMany()
                    .HasForeignKey(s => s.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Screen>()
                    .WithMany()
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => new { s.ScreenId, s.StartTime });
            });

            modelBuilder.Entity<ShowSeat>(entity =>
            {
                entity.ToTable("ShowSeats");

                entity.HasKey(s => s.ShowSeatId);

                entity.Property(s => s.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne<Show>()
                    .WithMany()
                    .HasForeignKey(s => s.ShowId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Seat>()
                    .WithMany()
                    .HasForeignKey(s => s.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => new { s.ShowId, s.SeatId })
                    .IsUnique();
            });
        }

    }
}