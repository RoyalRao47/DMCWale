using System.Security.Claims;
using DMCWale.Data;
using DMCWale.Data.Constants;
using DMCWale.Data.Models;
using DMCWale.Data.Models.Crm;
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
        await SeedCrmPagePermissionsAsync(context, roleManager);

        var adminSection = configuration.GetSection("DefaultAdmin");
        var email = adminSection["Email"];
        var username = adminSection["Username"];
        var password = adminSection["Password"];
        var firstName = adminSection["FirstName"];
        var lastName = adminSection["LastName"];
        var mobile = adminSection["Mobile"];
        const string defaultAdminAgentSupplierCode = "DMC-1000";

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
                AgentSupplierCode = defaultAdminAgentSupplierCode,
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

        if (string.IsNullOrWhiteSpace(adminUser.AgentSupplierCode))
        {
            adminUser.AgentSupplierCode = defaultAdminAgentSupplierCode;
            await userManager.UpdateAsync(adminUser);
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
                AgentSupplierCode = adminUser.AgentSupplierCode ?? defaultAdminAgentSupplierCode,
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

    private static async Task SeedCrmPagePermissionsAsync(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
    {
        foreach (var moduleDefinition in CrmPagePermissionCatalog.Modules)
        {
            var pageModule = await context.PageModules
                .FirstOrDefaultAsync(module => module.Code == moduleDefinition.Code);

            if (pageModule is null)
            {
                pageModule = new PageModule
                {
                    Name = moduleDefinition.Name,
                    Code = moduleDefinition.Code,
                    Area = moduleDefinition.Area,
                    Controller = moduleDefinition.Controller,
                    Action = moduleDefinition.Action,
                    DisplayOrder = moduleDefinition.DisplayOrder,
                    IsActive = true,
                    AddDate = DateTime.UtcNow
                };

                await context.PageModules.AddAsync(pageModule);
                await context.SaveChangesAsync();
            }
            else
            {
                pageModule.Name = moduleDefinition.Name;
                pageModule.Area = moduleDefinition.Area;
                pageModule.Controller = moduleDefinition.Controller;
                pageModule.Action = moduleDefinition.Action;
                pageModule.DisplayOrder = moduleDefinition.DisplayOrder;
                pageModule.IsActive = true;
                pageModule.ModifyDate = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            foreach (var permissionCode in moduleDefinition.PermissionCodes)
            {
                var pagePermission = await context.PagePermissions
                    .FirstOrDefaultAsync(permission =>
                        permission.PageModuleId == pageModule.Id &&
                        permission.PermissionCode == permissionCode);

                if (pagePermission is null)
                {
                    await context.PagePermissions.AddAsync(new PagePermission
                    {
                        PageModuleId = pageModule.Id,
                        PermissionCode = permissionCode,
                        PermissionName = permissionCode == CrmPermissionActionConstants.RecordPayment
                            ? "Record Payment"
                            : permissionCode,
                        IsActive = true,
                        AddDate = DateTime.UtcNow
                    });
                }
                else
                {
                    pagePermission.PermissionName = permissionCode == CrmPermissionActionConstants.RecordPayment
                        ? "Record Payment"
                        : permissionCode;
                    pagePermission.IsActive = true;
                    pagePermission.ModifyDate = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync();
        }

        var permissions = await context.PagePermissions
            .Include(permission => permission.PageModule)
            .ToListAsync();

        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var roleClaims = await roleManager.GetClaimsAsync(role);

            foreach (var moduleDefinition in CrmPagePermissionCatalog.Modules)
            {
                var allowedPermissions = moduleDefinition.DefaultRolePermissions.TryGetValue(roleName, out var configuredPermissions)
                    ? configuredPermissions
                    : [];

                foreach (var permission in permissions.Where(permission => permission.PageModule.Code == moduleDefinition.Code))
                {
                    var isAllowed = roleName == RoleConstants.Admin ||
                        allowedPermissions.Contains(permission.PermissionCode, StringComparer.OrdinalIgnoreCase);

                    var rolePermission = await context.RolePagePermissions
                        .FirstOrDefaultAsync(x => x.RoleId == role.Id && x.PagePermissionId == permission.Id);

                    if (rolePermission is null)
                    {
                        await context.RolePagePermissions.AddAsync(new RolePagePermission
                        {
                            RoleId = role.Id,
                            PagePermissionId = permission.Id,
                            IsAllowed = isAllowed,
                            AddDate = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        rolePermission.IsAllowed = isAllowed;
                        rolePermission.ModifyDate = DateTime.UtcNow;
                    }

                    if (!isAllowed)
                    {
                        continue;
                    }

                    var claimValue = PagePermissionConstants.BuildClaimValue(moduleDefinition.Code, permission.PermissionCode);
                    var hasClaim = roleClaims.Any(claim =>
                        claim.Type == PagePermissionConstants.ClaimType &&
                        claim.Value == claimValue);

                    if (!hasClaim)
                    {
                        await roleManager.AddClaimAsync(
                            role,
                            new Claim(PagePermissionConstants.ClaimType, claimValue));
                    }
                }
            }
        }

        await context.SaveChangesAsync();
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
