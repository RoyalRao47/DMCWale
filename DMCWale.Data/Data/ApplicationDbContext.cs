using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> UserProfiles => Set<User>();

    public DbSet<UserDetail> UserDetails => Set<UserDetail>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.AspNetUserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(user => user.Username)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(user => user.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(user => user.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(user => user.Mobile)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(user => user.IsActive)
                .HasDefaultValue(true);

            entity.Property(user => user.IsLeft)
                .HasDefaultValue(false);

            entity.Property(user => user.AddDate)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(user => user.AspNetUserId)
                .IsUnique();

            entity.HasOne(user => user.AspNetUser)
                .WithOne(applicationUser => applicationUser.UserProfile)
                .HasForeignKey<User>(user => user.AspNetUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserDetail>(entity =>
        {
            entity.ToTable("UserDetails");

            entity.HasKey(userDetail => userDetail.Id);

            entity.Property(userDetail => userDetail.AspNetUserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(userDetail => userDetail.Salutation)
                .HasMaxLength(20);

            entity.Property(userDetail => userDetail.CountryCode)
                .HasMaxLength(10);

            entity.Property(userDetail => userDetail.City)
                .HasMaxLength(100);

            entity.Property(userDetail => userDetail.Address1)
                .HasMaxLength(250);

            entity.Property(userDetail => userDetail.Address2)
                .HasMaxLength(250);

            entity.Property(userDetail => userDetail.Signature)
                .HasMaxLength(4000);

            entity.Property(userDetail => userDetail.ProfileImagePath)
                .HasMaxLength(500);

            entity.Property(userDetail => userDetail.AddDate)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(userDetail => userDetail.AspNetUserId)
                .IsUnique();

            entity.HasOne(userDetail => userDetail.AspNetUser)
                .WithOne(applicationUser => applicationUser.UserDetail)
                .HasForeignKey<UserDetail>(userDetail => userDetail.AspNetUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
