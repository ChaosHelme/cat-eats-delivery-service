using CatEats.Domain.ValueObjects;
using CatEats.UserService.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace CatEats.UserService.Infrastructure;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id)
                .HasConversion(id => id.Value, value => new UserId(value));

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(u => u.Role)
                .HasConversion<int>();

            entity.Property(u => u.Status)
                .HasConversion<int>();

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.Property(u => u.LastLoginAt);

            // Configure owned Address collection
            entity.OwnsMany(u => u.Addresses, address =>
            {
                address.ToTable("user_addresses");
                
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(255);

                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.Latitude)
                    .IsRequired()
                    .HasPrecision(10, 8);

                address.Property(a => a.Longitude)
                    .IsRequired()
                    .HasPrecision(11, 8);

                address.Property(a => a.IsDefault)
                    .IsRequired();
            });
        });
    }
}