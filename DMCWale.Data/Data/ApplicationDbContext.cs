using DMCWale.Data.Models;
using DMCWale.Data.Models.Crm;
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

    public DbSet<PageModule> PageModules => Set<PageModule>();
    public DbSet<PagePermission> PagePermissions => Set<PagePermission>();
    public DbSet<RolePagePermission> RolePagePermissions => Set<RolePagePermission>();
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<AgentDestinationMapping> AgentDestinationMappings => Set<AgentDestinationMapping>();
    public DbSet<MarkupRule> MarkupRules => Set<MarkupRule>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<LeadNote> LeadNotes => Set<LeadNote>();
    public DbSet<LeadAssignmentHistory> LeadAssignmentHistories => Set<LeadAssignmentHistory>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageDay> PackageDays => Set<PackageDay>();
    public DbSet<PackageService> PackageServices => Set<PackageService>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationVersion> QuotationVersions => Set<QuotationVersion>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<BookingExecution> BookingExecutions => Set<BookingExecution>();
    public DbSet<ApiProvider> ApiProviders => Set<ApiProvider>();
    public DbSet<ApiInventoryLog> ApiInventoryLogs => Set<ApiInventoryLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.AgentSupplierCode)
                .HasMaxLength(50);

            entity.HasIndex(user => user.AgentSupplierCode)
                .IsUnique()
                .HasFilter("[AgentSupplierCode] IS NOT NULL");
        });

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

            entity.Property(user => user.AgentSupplierCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(user => user.IsActive)
                .HasDefaultValue(true);

            entity.Property(user => user.IsLeft)
                .HasDefaultValue(false);

            entity.Property(user => user.AddDate)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(user => user.AspNetUserId)
                .IsUnique();

            entity.HasIndex(user => user.AgentSupplierCode)
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

        ConfigureCrmEntities(builder);
    }

    private static void ConfigureCrmEntities(ModelBuilder builder)
    {
        builder.Entity<PageModule>(entity =>
        {
            entity.ToTable("PageModules");
            entity.HasKey(pageModule => pageModule.Id);
            entity.Property(pageModule => pageModule.Name).IsRequired().HasMaxLength(150);
            entity.Property(pageModule => pageModule.Code).IsRequired().HasMaxLength(100);
            entity.Property(pageModule => pageModule.Area).HasMaxLength(100);
            entity.Property(pageModule => pageModule.Controller).IsRequired().HasMaxLength(100);
            entity.Property(pageModule => pageModule.Action).IsRequired().HasMaxLength(100);
            entity.Property(pageModule => pageModule.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(pageModule => pageModule.Code).IsUnique();
        });

        builder.Entity<PagePermission>(entity =>
        {
            entity.ToTable("PagePermissions");
            entity.HasKey(permission => permission.Id);
            entity.Property(permission => permission.PermissionCode).IsRequired().HasMaxLength(50);
            entity.Property(permission => permission.PermissionName).IsRequired().HasMaxLength(150);
            entity.Property(permission => permission.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(permission => new { permission.PageModuleId, permission.PermissionCode }).IsUnique();
            entity.HasOne(permission => permission.PageModule)
                .WithMany(pageModule => pageModule.Permissions)
                .HasForeignKey(permission => permission.PageModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RolePagePermission>(entity =>
        {
            entity.ToTable("RolePagePermissions");
            entity.HasKey(rolePermission => rolePermission.Id);
            entity.Property(rolePermission => rolePermission.RoleId).IsRequired().HasMaxLength(450);
            entity.Property(rolePermission => rolePermission.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(rolePermission => new { rolePermission.RoleId, rolePermission.PagePermissionId }).IsUnique();
            entity.HasOne(rolePermission => rolePermission.Role)
                .WithMany()
                .HasForeignKey(rolePermission => rolePermission.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(rolePermission => rolePermission.PagePermission)
                .WithMany(permission => permission.RolePagePermissions)
                .HasForeignKey(rolePermission => rolePermission.PagePermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Destination>(entity =>
        {
            entity.ToTable("Destinations");
            entity.HasKey(destination => destination.Id);
            entity.Property(destination => destination.Name).IsRequired().HasMaxLength(150);
            entity.Property(destination => destination.Code).IsRequired().HasMaxLength(50);
            entity.Property(destination => destination.Country).IsRequired().HasMaxLength(100);
            entity.Property(destination => destination.Description).HasMaxLength(1000);
            entity.Property(destination => destination.IsActive).HasDefaultValue(true);
            entity.Property(destination => destination.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(destination => destination.Code).IsUnique();
        });

        builder.Entity<AgentDestinationMapping>(entity =>
        {
            entity.ToTable("AgentDestinationMappings");
            entity.HasKey(mapping => mapping.Id);
            entity.Property(mapping => mapping.IsActive).HasDefaultValue(true);
            entity.Property(mapping => mapping.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(mapping => new { mapping.AgentUserId, mapping.DestinationId }).IsUnique();
            entity.HasOne(mapping => mapping.Destination).WithMany().HasForeignKey(mapping => mapping.DestinationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(mapping => mapping.AgentUser).WithMany().HasForeignKey(mapping => mapping.AgentUserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MarkupRule>(entity =>
        {
            entity.ToTable("MarkupRules");
            entity.HasKey(rule => rule.Id);
            entity.Property(rule => rule.MarkupType).IsRequired().HasMaxLength(50);
            entity.Property(rule => rule.MarkupPercentage).HasPrecision(10, 2);
            entity.Property(rule => rule.IsActive).HasDefaultValue(true);
            entity.Property(rule => rule.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(rule => rule.Destination).WithMany().HasForeignKey(rule => rule.DestinationId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currencies");
            entity.HasKey(currency => currency.Id);
            entity.Property(currency => currency.Code).IsRequired().HasMaxLength(10);
            entity.Property(currency => currency.Name).IsRequired().HasMaxLength(100);
            entity.Property(currency => currency.Symbol).HasMaxLength(10);
            entity.Property(currency => currency.IsActive).HasDefaultValue(true);
            entity.Property(currency => currency.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(currency => currency.Code).IsUnique();
        });

        builder.Entity<ExchangeRate>(entity =>
        {
            entity.ToTable("ExchangeRates");
            entity.HasKey(rate => rate.Id);
            entity.Property(rate => rate.Rate).HasPrecision(18, 6);
            entity.Property(rate => rate.Source).HasMaxLength(100);
            entity.Property(rate => rate.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(rate => rate.FromCurrency).WithMany().HasForeignKey(rate => rate.FromCurrencyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(rate => rate.ToCurrency).WithMany().HasForeignKey(rate => rate.ToCurrencyId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Lead>(entity =>
        {
            entity.ToTable("Leads");
            entity.HasKey(lead => lead.Id);
            entity.Property(lead => lead.LeadReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(lead => lead.Source).IsRequired().HasMaxLength(100);
            entity.Property(lead => lead.CustomerName).IsRequired().HasMaxLength(150);
            entity.Property(lead => lead.CustomerEmail).IsRequired().HasMaxLength(256);
            entity.Property(lead => lead.CustomerMobile).IsRequired().HasMaxLength(30);
            entity.Property(lead => lead.CustomerRequirement).HasMaxLength(2000);
            entity.Property(lead => lead.LeadStatus).IsRequired().HasMaxLength(50);
            entity.Property(lead => lead.Priority).IsRequired().HasMaxLength(50);
            entity.Property(lead => lead.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(lead => lead.LeadReferenceNo).IsUnique();
            entity.HasOne(lead => lead.Destination).WithMany().HasForeignKey(lead => lead.DestinationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(lead => lead.AssignedStaff).WithMany().HasForeignKey(lead => lead.AssignedStaffId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(lead => lead.AssignedAgent).WithMany().HasForeignKey(lead => lead.AssignedAgentId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<LeadNote>(entity =>
        {
            entity.ToTable("LeadNotes");
            entity.HasKey(note => note.Id);
            entity.Property(note => note.Note).IsRequired().HasMaxLength(2000);
            entity.Property(note => note.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(note => note.Lead).WithMany().HasForeignKey(note => note.LeadId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(note => note.AddedByUser).WithMany().HasForeignKey(note => note.AddedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<LeadAssignmentHistory>(entity =>
        {
            entity.ToTable("LeadAssignmentHistories");
            entity.HasKey(history => history.Id);
            entity.Property(history => history.Remarks).HasMaxLength(1000);
            entity.Property(history => history.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(history => history.Lead).WithMany().HasForeignKey(history => history.LeadId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(history => history.FromUser).WithMany().HasForeignKey(history => history.FromUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(history => history.ToUser).WithMany().HasForeignKey(history => history.ToUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(history => history.AssignedByUser).WithMany().HasForeignKey(history => history.AssignedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Package>(entity =>
        {
            entity.ToTable("Packages");
            entity.HasKey(package => package.Id);
            entity.Property(package => package.PackageReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(package => package.Title).IsRequired().HasMaxLength(200);
            entity.Property(package => package.Description).HasMaxLength(2000);
            entity.Property(package => package.TotalCost).HasPrecision(18, 2);
            entity.Property(package => package.MarkupPercentage).HasPrecision(10, 2);
            entity.Property(package => package.FinalPrice).HasPrecision(18, 2);
            entity.Property(package => package.PackageStatus).IsRequired().HasMaxLength(50);
            entity.Property(package => package.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(package => package.PackageReferenceNo).IsUnique();
            entity.HasOne(package => package.Lead).WithMany().HasForeignKey(package => package.LeadId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(package => package.Destination).WithMany().HasForeignKey(package => package.DestinationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(package => package.CreatedByAgent).WithMany().HasForeignKey(package => package.CreatedByAgentId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PackageDay>(entity =>
        {
            entity.ToTable("PackageDays");
            entity.HasKey(day => day.Id);
            entity.Property(day => day.Title).IsRequired().HasMaxLength(200);
            entity.Property(day => day.Description).HasMaxLength(2000);
            entity.Property(day => day.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(day => day.Package).WithMany(package => package.Days).HasForeignKey(day => day.PackageId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PackageService>(entity =>
        {
            entity.ToTable("PackageServices");
            entity.HasKey(service => service.Id);
            entity.Property(service => service.ServiceType).IsRequired().HasMaxLength(50);
            entity.Property(service => service.ServiceName).IsRequired().HasMaxLength(200);
            entity.Property(service => service.SupplierName).HasMaxLength(150);
            entity.Property(service => service.Description).HasMaxLength(2000);
            entity.Property(service => service.CostPrice).HasPrecision(18, 2);
            entity.Property(service => service.MarkupPercentage).HasPrecision(10, 2);
            entity.Property(service => service.SellingPrice).HasPrecision(18, 2);
            entity.Property(service => service.ApiProvider).HasMaxLength(100);
            entity.Property(service => service.ApiReference).HasMaxLength(200);
            entity.Property(service => service.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(service => service.PackageDay).WithMany(day => day.Services).HasForeignKey(service => service.PackageDayId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Quotation>(entity =>
        {
            entity.ToTable("Quotations");
            entity.HasKey(quotation => quotation.Id);
            entity.Property(quotation => quotation.QuotationReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(quotation => quotation.ItineraryText).HasMaxLength(4000);
            entity.Property(quotation => quotation.InclusionText).HasMaxLength(4000);
            entity.Property(quotation => quotation.ExclusionText).HasMaxLength(4000);
            entity.Property(quotation => quotation.TotalAmount).HasPrecision(18, 2);
            entity.Property(quotation => quotation.Status).IsRequired().HasMaxLength(50);
            entity.Property(quotation => quotation.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(quotation => quotation.QuotationReferenceNo).IsUnique();
            entity.HasOne(quotation => quotation.Package).WithMany().HasForeignKey(quotation => quotation.PackageId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<QuotationVersion>(entity =>
        {
            entity.ToTable("QuotationVersions");
            entity.HasKey(version => version.Id);
            entity.Property(version => version.SnapshotJson).HasMaxLength(4000);
            entity.Property(version => version.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(version => version.Quotation).WithMany().HasForeignKey(version => version.QuotationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(version => version.CreatedByUser).WithMany().HasForeignKey(version => version.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");
            entity.HasKey(booking => booking.Id);
            entity.Property(booking => booking.BookingReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(booking => booking.CustomerName).IsRequired().HasMaxLength(150);
            entity.Property(booking => booking.CustomerEmail).IsRequired().HasMaxLength(256);
            entity.Property(booking => booking.CustomerMobile).IsRequired().HasMaxLength(30);
            entity.Property(booking => booking.TotalAmount).HasPrecision(18, 2);
            entity.Property(booking => booking.PaidAmount).HasPrecision(18, 2);
            entity.Property(booking => booking.OutstandingAmount).HasPrecision(18, 2);
            entity.Property(booking => booking.BookingStatus).IsRequired().HasMaxLength(50);
            entity.Property(booking => booking.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(booking => booking.BookingReferenceNo).IsUnique();
            entity.HasOne(booking => booking.Quotation).WithMany().HasForeignKey(booking => booking.QuotationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(booking => booking.Lead).WithMany().HasForeignKey(booking => booking.LeadId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(booking => booking.Destination).WithMany().HasForeignKey(booking => booking.DestinationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(booking => booking.AssignedStaff).WithMany().HasForeignKey(booking => booking.AssignedStaffId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(booking => booking.AssignedSupplier).WithMany().HasForeignKey(booking => booking.AssignedSupplierId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<BookingStatusHistory>(entity =>
        {
            entity.ToTable("BookingStatusHistories");
            entity.HasKey(history => history.Id);
            entity.Property(history => history.FromStatus).HasMaxLength(50);
            entity.Property(history => history.ToStatus).IsRequired().HasMaxLength(50);
            entity.Property(history => history.Remarks).HasMaxLength(1000);
            entity.Property(history => history.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(history => history.Booking).WithMany().HasForeignKey(history => history.BookingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(history => history.ChangedByUser).WithMany().HasForeignKey(history => history.ChangedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(payment => payment.Id);
            entity.Property(payment => payment.PaymentReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(payment => payment.Amount).HasPrecision(18, 2);
            entity.Property(payment => payment.PaymentMode).IsRequired().HasMaxLength(50);
            entity.Property(payment => payment.PaymentStatus).IsRequired().HasMaxLength(50);
            entity.Property(payment => payment.TransactionId).HasMaxLength(150);
            entity.Property(payment => payment.PaymentLink).HasMaxLength(500);
            entity.Property(payment => payment.Remarks).HasMaxLength(1000);
            entity.Property(payment => payment.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(payment => payment.PaymentReferenceNo).IsUnique();
            entity.HasOne(payment => payment.Booking).WithMany().HasForeignKey(payment => payment.BookingId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(payment => payment.Currency).WithMany().HasForeignKey(payment => payment.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(payment => payment.RecordedByUser).WithMany().HasForeignKey(payment => payment.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<BookingExecution>(entity =>
        {
            entity.ToTable("BookingExecutions");
            entity.HasKey(execution => execution.Id);
            entity.Property(execution => execution.ExecutionStatus).IsRequired().HasMaxLength(50);
            entity.Property(execution => execution.ConfirmationReference).HasMaxLength(150);
            entity.Property(execution => execution.Remarks).HasMaxLength(1000);
            entity.Property(execution => execution.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(execution => execution.Booking).WithMany().HasForeignKey(execution => execution.BookingId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(execution => execution.Supplier).WithMany().HasForeignKey(execution => execution.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ApiProvider>(entity =>
        {
            entity.ToTable("ApiProviders");
            entity.HasKey(provider => provider.Id);
            entity.Property(provider => provider.Name).IsRequired().HasMaxLength(150);
            entity.Property(provider => provider.ProviderType).IsRequired().HasMaxLength(50);
            entity.Property(provider => provider.BaseUrl).HasMaxLength(500);
            entity.Property(provider => provider.IsActive).HasDefaultValue(true);
            entity.Property(provider => provider.AddDate).HasDefaultValueSql("GETUTCDATE()");
        });

        builder.Entity<ApiInventoryLog>(entity =>
        {
            entity.ToTable("ApiInventoryLogs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.RequestType).IsRequired().HasMaxLength(100);
            entity.Property(log => log.RequestJson).HasMaxLength(4000);
            entity.Property(log => log.ResponseJson).HasMaxLength(4000);
            entity.Property(log => log.Status).IsRequired().HasMaxLength(50);
            entity.Property(log => log.AddDate).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(log => log.Provider).WithMany().HasForeignKey(log => log.ProviderId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
