using System.Security.Claims;
using DMCWale.Data.Constants;
using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DMCWale.Service.Infrastructure;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var userRepository = services.GetRequiredService<IRepository<User>>();
        var roleRepository = services.GetRequiredService<IRepository<ApplicationRole>>();
        var context = userRepository.GetDbContext();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        if (!context.Database.GetMigrations().Any())
        {
            logger.LogInformation("No EF Core migrations were found. Skipping database seed until migrations are created.");
            return;
        }

        await context.Database.MigrateAsync();

        await RenameLegacyRolesAsync(roleRepository);

        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                if (!roleResult.Succeeded)
                {
                    logger.LogWarning("Could not create role {RoleName}: {Errors}", roleName, string.Join(", ", roleResult.Errors.Select(error => error.Description)));
                }
            }
        }

        await GrantProfileClaimToAllRolesAsync(roleManager);

        var adminSection = configuration.GetSection("DefaultAdmin");
        var email = adminSection["Email"];
        var username = adminSection["Username"];
        var password = adminSection["Password"];
        var firstName = adminSection["FirstName"];
        var lastName = adminSection["LastName"];
        var mobile = adminSection["Mobile"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(mobile))
        {
            logger.LogWarning("DefaultAdmin configuration is incomplete. Default admin user was not seeded.");
            return;
        }

        var adminUser = await userManager.FindByNameAsync(username);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = username,
                Email = email,
                PhoneNumber = mobile,
                EmailConfirmed = true
            };

            var createUserResult = await userManager.CreateAsync(adminUser, password);
            if (!createUserResult.Succeeded)
            {
                logger.LogWarning("Could not create default admin: {Errors}", string.Join(", ", createUserResult.Errors.Select(error => error.Description)));
                return;
            }
        }

        if (!await userRepository.AsQueryable().AnyAsync(user => user.AspNetUserId == adminUser.Id))
        {
            await userRepository.InsertAsync(new User
            {
                AspNetUserId = adminUser.Id,
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Mobile = mobile,
                IsActive = true,
                IsLeft = false,
                AddDate = DateTime.UtcNow
            });
        }

        if (!await userManager.IsInRoleAsync(adminUser, RoleConstants.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
        }
    }

    private static async Task GrantProfileClaimToAllRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = await roleManager.Roles.ToListAsync();

        foreach (var role in roles)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            var hasProfileClaim = claims.Any(claim =>
                claim.Type == PagePermissionConstants.ClaimType &&
                claim.Value == PagePermissionConstants.ProfileView);

            if (!hasProfileClaim)
            {
                await roleManager.AddClaimAsync(
                    role,
                    new Claim(PagePermissionConstants.ClaimType, PagePermissionConstants.ProfileView));
            }
        }
    }

    private static async Task RenameLegacyRolesAsync(IRepository<ApplicationRole> roleRepository)
    {
        var legacyRoleMappings = new Dictionary<string, string>
        {
            ["Manager"] = RoleConstants.Agent,
            ["Editor"] = RoleConstants.Staff,
            ["User"] = RoleConstants.Supplier
        };

        foreach (var mapping in legacyRoleMappings)
        {
            var legacyRole = await roleRepository
                .Query()
                .AsTracking()
                .Filter(role => role.Name == mapping.Key)
                .GetQueryable()
                .FirstOrDefaultAsync();

            var currentRoleExists = await roleRepository
                .AsQueryable()
                .AnyAsync(role => role.Name == mapping.Value);

            if (legacyRole is null || currentRoleExists)
            {
                continue;
            }

            legacyRole.Name = mapping.Value;
            legacyRole.NormalizedName = mapping.Value.ToUpperInvariant();
        }

        await roleRepository.SaveChangesAsync();
    }
}
