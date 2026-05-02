using System.Globalization;
using System.Security.Claims;
using DMCWale.Data.Constants;
using DMCWale.Data.Models;
using DMCWale.Data.Models.Crm;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.DTOs.Crm;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DMCWale.Service.Services;

public class CrmModuleService : ICrmModuleService
{
    private const string DestinationLookup = "Destination";
    private const string CurrencyLookup = "Currency";
    private const string LeadLookup = "Lead";
    private const string PackageLookup = "Package";
    private const string QuotationLookup = "Quotation";
    private const string BookingLookup = "Booking";
    private const string AnyUserLookup = "AnyUser";
    private const string StaffUserLookup = "StaffUser";
    private const string AgentUserLookup = "AgentUser";
    private const string SupplierUserLookup = "SupplierUser";

    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<Destination> _destinationRepository;

    public CrmModuleService(IServiceProvider serviceProvider, IRepository<Destination> destinationRepository)
    {
        _serviceProvider = serviceProvider;
        _destinationRepository = destinationRepository;
    }

    public Task<bool> HasPermissionAsync(
        ClaimsPrincipal user,
        string moduleCode,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(false);
        }

        if (user.IsInRole(RoleConstants.Admin))
        {
            return Task.FromResult(true);
        }

        var claimValue = PagePermissionConstants.BuildClaimValue(moduleCode, permissionCode);
        return Task.FromResult(user.HasClaim(PagePermissionConstants.ClaimType, claimValue));
    }

    public async Task<IReadOnlyList<CrmModuleDto>> GetMenuModulesAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var modules = new List<CrmModuleDto>();

        foreach (var definition in CrmPagePermissionCatalog.Modules.OrderBy(module => module.DisplayOrder))
        {
            if (!await HasPermissionAsync(user, definition.Code, CrmPermissionActionConstants.View, cancellationToken))
            {
                continue;
            }

            modules.Add(new CrmModuleDto
            {
                ModuleCode = definition.Code,
                Title = definition.Name,
                Controller = definition.Controller,
                SupportedActions = definition.PermissionCodes.ToList()
            });
        }

        return modules;
    }

    public async Task<CrmModuleDto> GetListAsync(string moduleCode, CancellationToken cancellationToken = default)
    {
        var definition = GetDefinition(moduleCode);
        var rows = await GetRowsAsync(definition, cancellationToken);

        return new CrmModuleDto
        {
            ModuleCode = definition.ModuleCode,
            Title = definition.Title,
            Controller = definition.Controller,
            SupportedActions = definition.SupportedActions.ToList(),
            Fields = definition.Fields.Select(field => field.ToDto()).ToList(),
            Rows = rows
        };
    }

    public async Task<CrmModuleDto> GetFormAsync(
        string moduleCode,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        var definition = GetDefinition(moduleCode);
        object? entity = null;

        if (id.HasValue)
        {
            entity = await FindEntityAsync(definition.EntityType, id.Value, cancellationToken);
            if (entity is null)
            {
                throw new InvalidOperationException($"{definition.Title} record was not found.");
            }
        }

        var fields = new List<CrmFieldDto>();
        foreach (var field in definition.Fields)
        {
            var dto = field.ToDto();
            dto.Value = entity is null
                ? field.DefaultValue
                : ConvertToInputString(definition.EntityType.GetProperty(field.Name)?.GetValue(entity));

            dto.Options = await GetLookupOptionsAsync(field.LookupSource, cancellationToken);
            fields.Add(dto);
        }

        return new CrmModuleDto
        {
            ModuleCode = definition.ModuleCode,
            Title = definition.Title,
            Controller = definition.Controller,
            SupportedActions = definition.SupportedActions.ToList(),
            Fields = fields
        };
    }

    public async Task<CrmModuleDto> GetDetailsAsync(string moduleCode, int id, CancellationToken cancellationToken = default)
    {
        var definition = GetDefinition(moduleCode);
        var entity = await FindEntityAsync(definition.EntityType, id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"{definition.Title} record was not found.");
        }

        return new CrmModuleDto
        {
            ModuleCode = definition.ModuleCode,
            Title = definition.Title,
            Controller = definition.Controller,
            SupportedActions = definition.SupportedActions.ToList(),
            Rows =
            [
                new CrmRowDto
                {
                    Id = id,
                    Cells = definition.Fields
                        .Select(field => new CrmCellDto
                        {
                            Label = field.Label,
                            Value = ConvertToString(definition.EntityType.GetProperty(field.Name)?.GetValue(entity))
                        })
                        .ToList()
                }
            ]
        };
    }

    public async Task<ServiceResult> SaveAsync(CrmSaveRequestDto request, CancellationToken cancellationToken = default)
    {
        var definition = GetDefinition(request.ModuleCode);
        var entity = request.Id.HasValue
            ? await FindEntityAsync(definition.EntityType, request.Id.Value, cancellationToken)
            : Activator.CreateInstance(definition.EntityType);

        if (entity is null)
        {
            return ServiceResult.Failure($"{definition.Title} record was not found.");
        }

        foreach (var field in definition.Fields.Where(field => !field.IsReadOnly))
        {
            var property = definition.EntityType.GetProperty(field.Name);
            if (property is null)
            {
                continue;
            }

            request.Values.TryGetValue(field.Name, out var rawValue);
            if (field.IsRequired && string.IsNullOrWhiteSpace(rawValue))
            {
                return ServiceResult.Failure($"{field.Label} is required.");
            }

            var lookupValidation = await ValidateLookupAsync(field, rawValue, cancellationToken);
            if (!lookupValidation.Succeeded)
            {
                return lookupValidation;
            }

            try
            {
                property.SetValue(entity, ConvertFromString(rawValue, property.PropertyType));
            }
            catch (FormatException)
            {
                return ServiceResult.Failure($"{field.Label} has an invalid value.");
            }
            catch (OverflowException)
            {
                return ServiceResult.Failure($"{field.Label} has an invalid value.");
            }
        }

        SetPropertyIfExists(entity, definition.EntityType, "AddDate", request.Id.HasValue ? null : DateTime.UtcNow);
        SetPropertyIfExists(entity, definition.EntityType, "ModifyDate", request.Id.HasValue ? DateTime.UtcNow : null);
        SetPropertyIfExists(entity, definition.EntityType, "IsActive", request.Id.HasValue ? null : true);

        try
        {
            if (!request.Id.HasValue)
            {
                await InsertEntityAsync(definition.EntityType, entity, cancellationToken);
                return ServiceResult.Success($"{definition.Title} record created.");
            }

            await SaveChangesAsync(definition.EntityType, cancellationToken);
            return ServiceResult.Success($"{definition.Title} record updated.");
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Failure($"{definition.Title} could not be saved. Please verify all selected related records exist and try again.");
        }
    }

    public async Task<ServiceResult> SoftDeleteAsync(string moduleCode, int id, CancellationToken cancellationToken = default)
    {
        var definition = GetDefinition(moduleCode);
        var entity = await FindEntityAsync(definition.EntityType, id, cancellationToken);
        if (entity is null)
        {
            return ServiceResult.Failure($"{definition.Title} record was not found.");
        }

        var isActiveProperty = definition.EntityType.GetProperty("IsActive");
        if (isActiveProperty is not null && isActiveProperty.PropertyType == typeof(bool))
        {
            isActiveProperty.SetValue(entity, false);
            SetPropertyIfExists(entity, definition.EntityType, "ModifyDate", DateTime.UtcNow);
            await SaveChangesAsync(definition.EntityType, cancellationToken);
            return ServiceResult.Success($"{definition.Title} record deactivated.");
        }

        return ServiceResult.Failure($"{definition.Title} does not support safe deletion yet.");
    }

    private async Task<ServiceResult> ValidateLookupAsync(
        FieldDefinition field,
        string? rawValue,
        CancellationToken cancellationToken)
    {
        if (field.LookupSource is null || string.IsNullOrWhiteSpace(rawValue))
        {
            return ServiceResult.Success();
        }

        if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) || id <= 0)
        {
            return ServiceResult.Failure($"{field.Label} is invalid.");
        }

        var exists = field.LookupSource switch
        {
            DestinationLookup => await _destinationRepository.GetDbContext().Destinations
                .AsNoTracking()
                .AnyAsync(destination => destination.Id == id && destination.IsActive, cancellationToken),
            CurrencyLookup => await _destinationRepository.GetDbContext().Currencies
                .AsNoTracking()
                .AnyAsync(currency => currency.Id == id && currency.IsActive, cancellationToken),
            LeadLookup => await _destinationRepository.GetDbContext().Leads
                .AsNoTracking()
                .AnyAsync(lead => lead.Id == id, cancellationToken),
            PackageLookup => await _destinationRepository.GetDbContext().Packages
                .AsNoTracking()
                .AnyAsync(package => package.Id == id, cancellationToken),
            QuotationLookup => await _destinationRepository.GetDbContext().Quotations
                .AsNoTracking()
                .AnyAsync(quotation => quotation.Id == id, cancellationToken),
            BookingLookup => await _destinationRepository.GetDbContext().Bookings
                .AsNoTracking()
                .AnyAsync(booking => booking.Id == id, cancellationToken),
            StaffUserLookup => await UserExistsInRoleAsync(id, RoleConstants.Staff, cancellationToken),
            AgentUserLookup => await UserExistsInRoleAsync(id, RoleConstants.Agent, cancellationToken),
            SupplierUserLookup => await UserExistsInRoleAsync(id, RoleConstants.Supplier, cancellationToken),
            AnyUserLookup => await _destinationRepository.GetDbContext().UserProfiles
                .AsNoTracking()
                .AnyAsync(user => user.Id == id && user.IsActive && !user.IsLeft, cancellationToken),
            _ => true
        };

        return exists
            ? ServiceResult.Success()
            : ServiceResult.Failure($"{field.Label} does not exist or is not active.");
    }

    private async Task<List<CrmSelectOptionDto>> GetLookupOptionsAsync(
        string? lookupSource,
        CancellationToken cancellationToken)
    {
        if (lookupSource is null)
        {
            return [];
        }

        var context = _destinationRepository.GetDbContext();

        return lookupSource switch
        {
            DestinationLookup => await context.Destinations
                .AsNoTracking()
                .Where(destination => destination.IsActive)
                .OrderBy(destination => destination.Name)
                .Select(destination => new CrmSelectOptionDto
                {
                    Value = destination.Id.ToString(),
                    Text = destination.Name + " (" + destination.Code + ")"
                })
                .ToListAsync(cancellationToken),
            CurrencyLookup => await context.Currencies
                .AsNoTracking()
                .Where(currency => currency.IsActive)
                .OrderBy(currency => currency.Code)
                .Select(currency => new CrmSelectOptionDto
                {
                    Value = currency.Id.ToString(),
                    Text = currency.Code + " - " + currency.Name
                })
                .ToListAsync(cancellationToken),
            LeadLookup => await context.Leads
                .AsNoTracking()
                .OrderByDescending(lead => lead.AddDate)
                .Select(lead => new CrmSelectOptionDto
                {
                    Value = lead.Id.ToString(),
                    Text = lead.LeadReferenceNo + " - " + lead.CustomerName
                })
                .Take(200)
                .ToListAsync(cancellationToken),
            PackageLookup => await context.Packages
                .AsNoTracking()
                .OrderByDescending(package => package.AddDate)
                .Select(package => new CrmSelectOptionDto
                {
                    Value = package.Id.ToString(),
                    Text = package.PackageReferenceNo + " - " + package.Title
                })
                .Take(200)
                .ToListAsync(cancellationToken),
            QuotationLookup => await context.Quotations
                .AsNoTracking()
                .OrderByDescending(quotation => quotation.AddDate)
                .Select(quotation => new CrmSelectOptionDto
                {
                    Value = quotation.Id.ToString(),
                    Text = quotation.QuotationReferenceNo + " - " + quotation.Status
                })
                .Take(200)
                .ToListAsync(cancellationToken),
            BookingLookup => await context.Bookings
                .AsNoTracking()
                .OrderByDescending(booking => booking.AddDate)
                .Select(booking => new CrmSelectOptionDto
                {
                    Value = booking.Id.ToString(),
                    Text = booking.BookingReferenceNo + " - " + booking.CustomerName
                })
                .Take(200)
                .ToListAsync(cancellationToken),
            StaffUserLookup => await GetUserOptionsAsync(RoleConstants.Staff, cancellationToken),
            AgentUserLookup => await GetUserOptionsAsync(RoleConstants.Agent, cancellationToken),
            SupplierUserLookup => await GetUserOptionsAsync(RoleConstants.Supplier, cancellationToken),
            AnyUserLookup => await GetUserOptionsAsync(null, cancellationToken),
            _ => []
        };
    }

    private async Task<List<CrmSelectOptionDto>> GetUserOptionsAsync(
        string? roleName,
        CancellationToken cancellationToken)
    {
        var context = _destinationRepository.GetDbContext();

        var query = context.UserProfiles
            .AsNoTracking()
            .Where(user => user.IsActive && !user.IsLeft);

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            query = query
                .Join(
                    context.UserRoles.AsNoTracking(),
                    user => user.AspNetUserId,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                .Join(
                    context.Roles.AsNoTracking(),
                    x => x.userRole.RoleId,
                    role => role.Id,
                    (x, role) => new { x.user, role })
                .Where(x => x.role.Name == roleName)
                .Select(x => x.user);
        }

        return await query
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ThenBy(user => user.Username)
            .Select(user => new CrmSelectOptionDto
            {
                Value = user.Id.ToString(),
                Text = user.FirstName + " " + user.LastName + " (" + user.Username + ")"
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<bool> UserExistsInRoleAsync(
        int userId,
        string roleName,
        CancellationToken cancellationToken)
    {
        var context = _destinationRepository.GetDbContext();

        return await context.UserProfiles
            .AsNoTracking()
            .Where(user => user.Id == userId && user.IsActive && !user.IsLeft)
            .Join(
                context.UserRoles.AsNoTracking(),
                user => user.AspNetUserId,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole })
            .Join(
                context.Roles.AsNoTracking(),
                x => x.userRole.RoleId,
                role => role.Id,
                (x, role) => new { x.user, role })
            .AnyAsync(x => x.role.Name == roleName, cancellationToken);
    }

    public async Task<CrmDashboardDto> GetDashboardAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var context = _destinationRepository.GetDbContext();

        var totalLeads = await context.Leads.AsNoTracking().CountAsync(cancellationToken);
        var processingLeads = await context.Leads.AsNoTracking()
            .CountAsync(lead => lead.LeadStatus == "New" || lead.LeadStatus == "InProgress" || lead.LeadStatus == "Assigned", cancellationToken);
        var activeBookings = await context.Bookings.AsNoTracking()
            .CountAsync(booking => booking.BookingStatus != "Cancelled" && booking.BookingStatus != "Executed", cancellationToken);
        var paymentsReceived = await context.Payments.AsNoTracking()
            .Where(payment => payment.PaymentStatus == "Completed")
            .SumAsync(payment => (decimal?)payment.Amount, cancellationToken) ?? 0;
        var paymentsOutstanding = await context.Bookings.AsNoTracking()
            .SumAsync(booking => (decimal?)booking.OutstandingAmount, cancellationToken) ?? 0;
        var pendingExecution = await context.BookingExecutions.AsNoTracking()
            .CountAsync(execution => execution.ExecutionStatus == "Pending" || execution.ExecutionStatus == "InProgress", cancellationToken);

        return new CrmDashboardDto
        {
            Title = $"{roleName} Dashboard",
            Metrics =
            [
                new() { Label = "Total leads received", Value = totalLeads.ToString(CultureInfo.InvariantCulture) },
                new() { Label = "Leads under processing", Value = processingLeads.ToString(CultureInfo.InvariantCulture) },
                new() { Label = "Active bookings", Value = activeBookings.ToString(CultureInfo.InvariantCulture) },
                new() { Label = "Payments received", Value = paymentsReceived.ToString("N2", CultureInfo.InvariantCulture) },
                new() { Label = "Payments outstanding", Value = paymentsOutstanding.ToString("N2", CultureInfo.InvariantCulture) },
                new() { Label = "Bookings pending execution", Value = pendingExecution.ToString(CultureInfo.InvariantCulture) }
            ]
        };
    }

    private async Task<List<CrmRowDto>> GetRowsAsync(ModuleDefinition definition, CancellationToken cancellationToken)
    {
        var repository = GetRepository(definition.EntityType);
        var asQueryableMethod = repository.GetType().GetMethod("AsQueryable", [typeof(bool)]);
        var query = (IQueryable?)asQueryableMethod?.Invoke(repository, [false]);

        if (query is null)
        {
            return [];
        }

        var entities = await ToObjectListAsync(query, definition.EntityType, cancellationToken);

        return entities.Select(entity =>
        {
            var id = (int)(definition.EntityType.GetProperty("Id")?.GetValue(entity) ?? 0);
            return new CrmRowDto
            {
                Id = id,
                Cells = definition.ListFields.Select(field => new CrmCellDto
                {
                    Label = field.Label,
                    Value = ConvertToString(definition.EntityType.GetProperty(field.Name)?.GetValue(entity))
                }).ToList()
            };
        }).ToList();
    }

    private static async Task<List<object>> ToObjectListAsync(
        IQueryable query,
        Type entityType,
        CancellationToken cancellationToken)
    {
        var takeMethod = typeof(Queryable)
            .GetMethods()
            .FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return method.Name == nameof(Queryable.Take) &&
                    method.IsGenericMethodDefinition &&
                    parameters.Length == 2 &&
                    parameters[0].ParameterType.IsGenericType &&
                    parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    parameters[1].ParameterType == typeof(int);
            })?.MakeGenericMethod(entityType);

        if (takeMethod is null)
        {
            throw new InvalidOperationException("Could not resolve Queryable.Take<T>(IQueryable<T>, int).");
        }

        var limitedQuery = (IQueryable)takeMethod.Invoke(null, [query, 100])!;

        var toListMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods()
            .FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return method.Name == nameof(EntityFrameworkQueryableExtensions.ToListAsync) &&
                    method.IsGenericMethodDefinition &&
                    parameters.Length == 2 &&
                    parameters[0].ParameterType.IsGenericType &&
                    parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    parameters[1].ParameterType == typeof(CancellationToken);
            })?.MakeGenericMethod(entityType);

        if (toListMethod is null)
        {
            throw new InvalidOperationException("Could not resolve EntityFrameworkQueryableExtensions.ToListAsync<T>(IQueryable<T>, CancellationToken).");
        }

        var task = (Task)toListMethod.Invoke(null, [limitedQuery, cancellationToken])!;
        await task.ConfigureAwait(false);

        var result = (System.Collections.IEnumerable)(task.GetType().GetProperty("Result")?.GetValue(task) ?? Array.Empty<object>());
        return result.Cast<object>().ToList();
    }

    private async Task<object?> FindEntityAsync(Type entityType, int id, CancellationToken cancellationToken)
    {
        var repository = GetRepository(entityType);
        var method = repository.GetType().GetMethod("FindByIdAsync", [typeof(object), typeof(CancellationToken)]);
        var task = (Task?)method?.Invoke(repository, [id, cancellationToken]);
        if (task is null)
        {
            return null;
        }

        await task.ConfigureAwait(false);
        return task.GetType().GetProperty("Result")?.GetValue(task);
    }

    private async Task InsertEntityAsync(Type entityType, object entity, CancellationToken cancellationToken)
    {
        var repository = GetRepository(entityType);
        var method = repository.GetType().GetMethod("InsertAsync", [entityType, typeof(bool), typeof(CancellationToken)]);
        var task = (Task?)method?.Invoke(repository, [entity, true, cancellationToken]);
        if (task is not null)
        {
            await task.ConfigureAwait(false);
        }
    }

    private async Task SaveChangesAsync(Type entityType, CancellationToken cancellationToken)
    {
        var repository = GetRepository(entityType);
        var method = repository.GetType().GetMethod("SaveChangesAsync", [typeof(CancellationToken)]);
        var task = (Task?)method?.Invoke(repository, [cancellationToken]);
        if (task is not null)
        {
            await task.ConfigureAwait(false);
        }
    }

    private object GetRepository(Type entityType)
    {
        var repositoryType = typeof(IRepository<>).MakeGenericType(entityType);
        return _serviceProvider.GetRequiredService(repositoryType);
    }

    private static void SetPropertyIfExists(object entity, Type entityType, string propertyName, object? value)
    {
        if (value is null)
        {
            return;
        }

        var property = entityType.GetProperty(propertyName);
        if (property is not null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
    }

    private static object? ConvertFromString(string? value, Type targetType)
    {
        var nullableType = Nullable.GetUnderlyingType(targetType);
        var effectiveType = nullableType ?? targetType;

        if (string.IsNullOrWhiteSpace(value))
        {
            return nullableType is not null ? null : effectiveType == typeof(string) ? string.Empty : Activator.CreateInstance(effectiveType);
        }

        if (effectiveType == typeof(string))
        {
            return value.Trim();
        }

        if (effectiveType == typeof(int))
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(decimal))
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(DateTime))
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(bool))
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("1", StringComparison.OrdinalIgnoreCase);
        }

        return value;
    }

    private static string ConvertToString(object? value)
    {
        return value switch
        {
            null => string.Empty,
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            decimal decimalValue => decimalValue.ToString("0.##", CultureInfo.InvariantCulture),
            bool boolValue => boolValue ? "Yes" : "No",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    private static string ConvertToInputString(object? value)
    {
        return value switch
        {
            null => string.Empty,
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            bool boolValue => boolValue ? "true" : "false",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    private static ModuleDefinition GetDefinition(string moduleCode)
    {
        return Definitions.TryGetValue(moduleCode, out var definition)
            ? definition
            : throw new InvalidOperationException($"CRM module '{moduleCode}' is not configured.");
    }

    private static readonly IReadOnlyDictionary<string, ModuleDefinition> Definitions = BuildDefinitions();

    private static IReadOnlyDictionary<string, ModuleDefinition> BuildDefinitions()
    {
        var definitions = new[]
        {
            Definition(CrmModuleConstants.Destination, "Destinations", "Destination", typeof(Destination), [Text("Name", true), Text("Code", true), Text("Country", true), Text("Description"), Bool("IsActive", "Active", "true")]),
            Definition(CrmModuleConstants.Markup, "Markup Rules", "Markup", typeof(MarkupRule), [Lookup("DestinationId", "Destination", DestinationLookup), Text("MarkupType", true, "Global"), Decimal("MarkupPercentage", "Markup %", true), Bool("IsActive", "Active", "true")]),
            Definition(CrmModuleConstants.Currency, "Currencies", "Currency", typeof(Currency), [Text("Code", true), Text("Name", true), Text("Symbol"), Bool("IsBaseCurrency", "Base Currency"), Bool("IsActive", "Active", "true")]),
            Definition(CrmModuleConstants.ExchangeRate, "Exchange Rates", "ExchangeRate", typeof(ExchangeRate), [Lookup("FromCurrencyId", "From Currency", CurrencyLookup, true), Lookup("ToCurrencyId", "To Currency", CurrencyLookup, true), Decimal("Rate", "Rate", true), Date("RateDate", "Rate Date", true), Bool("IsLocked", "Locked"), Text("Source")]),
            Definition(CrmModuleConstants.Lead, "Leads", "Lead", typeof(Lead), [Text("LeadReferenceNo", true, "AUTO"), Text("Source", true), Lookup("DestinationId", "Destination", DestinationLookup, true), Date("TravelStartDate", "Travel Start", true), Date("TravelEndDate", "Travel End", true), Text("CustomerName", true), Text("CustomerEmail", true), Text("CustomerMobile", true), Text("CustomerRequirement"), Text("LeadStatus", true, "New"), Text("Priority", true, "Normal"), Lookup("AssignedStaffId", "Assigned Staff", StaffUserLookup), Lookup("AssignedAgentId", "Assigned Agent", AgentUserLookup)]),
            Definition(CrmModuleConstants.Package, "Packages", "Package", typeof(Package), [Lookup("LeadId", "Lead", LeadLookup, true), Text("PackageReferenceNo", true, "AUTO"), Lookup("DestinationId", "Destination", DestinationLookup, true), Lookup("CreatedByAgentId", "Agent", AgentUserLookup, true), Text("Title", true), Text("Description"), Decimal("TotalCost", "Total Cost"), Decimal("MarkupPercentage", "Markup %"), Decimal("FinalPrice", "Final Price"), Text("PackageStatus", true, "Draft")]),
            Definition(CrmModuleConstants.Quotation, "Quotations", "Quotation", typeof(Quotation), [Lookup("PackageId", "Package", PackageLookup, true), Text("QuotationReferenceNo", true, "AUTO"), Number("VersionNo", "Version No", true, "1"), Text("ItineraryText"), Text("InclusionText"), Text("ExclusionText"), Decimal("TotalAmount", "Total Amount"), Text("Status", true, "Draft"), Date("SentDate", "Sent Date")]),
            Definition(CrmModuleConstants.Booking, "Bookings", "Booking", typeof(Booking), [Text("BookingReferenceNo", true, "AUTO"), Lookup("QuotationId", "Quotation", QuotationLookup, true), Lookup("LeadId", "Lead", LeadLookup, true), Lookup("DestinationId", "Destination", DestinationLookup, true), Text("CustomerName", true), Text("CustomerEmail", true), Text("CustomerMobile", true), Date("TravelStartDate", "Travel Start", true), Date("TravelEndDate", "Travel End", true), Decimal("TotalAmount", "Total Amount"), Decimal("PaidAmount", "Paid Amount"), Decimal("OutstandingAmount", "Outstanding Amount"), Text("BookingStatus", true, "Pending"), Lookup("AssignedStaffId", "Assigned Staff", StaffUserLookup), Lookup("AssignedSupplierId", "Assigned Supplier", SupplierUserLookup), Bool("IsHighPriority", "High Priority")]),
            Definition(CrmModuleConstants.Payment, "Payments", "Payment", typeof(Payment), [Lookup("BookingId", "Booking", BookingLookup, true), Text("PaymentReferenceNo", true, "AUTO"), Decimal("Amount", "Amount", true), Lookup("CurrencyId", "Currency", CurrencyLookup, true), Text("PaymentMode", true, "Manual"), Text("PaymentStatus", true, "Pending"), Text("TransactionId"), Text("PaymentLink"), Date("PaymentDate", "Payment Date"), Lookup("RecordedByUserId", "Recorded By", AnyUserLookup), Text("Remarks")]),
            Definition(CrmModuleConstants.Execution, "Booking Executions", "Execution", typeof(BookingExecution), [Lookup("BookingId", "Booking", BookingLookup, true), Lookup("SupplierId", "Supplier", SupplierUserLookup, true), Text("ExecutionStatus", true, "Pending"), Text("ConfirmationReference"), Text("Remarks")]),
            Definition(CrmModuleConstants.ApiProvider, "API Providers", "ApiProvider", typeof(ApiProvider), [Text("Name", true), Text("ProviderType", true), Text("BaseUrl"), Bool("IsActive", "Active", "true")])
        };

        return definitions.ToDictionary(definition => definition.ModuleCode, StringComparer.OrdinalIgnoreCase);
    }

    private static ModuleDefinition Definition(string moduleCode, string title, string controller, Type entityType, IReadOnlyList<FieldDefinition> fields)
    {
        return new ModuleDefinition(moduleCode, title, controller, entityType, fields);
    }

    private static FieldDefinition Text(string name, bool required = false, string defaultValue = "")
    {
        return new FieldDefinition(name, SplitName(name), "text", required, false, defaultValue, null);
    }

    private static FieldDefinition Number(string name, string label, bool required = false, string defaultValue = "")
    {
        return new FieldDefinition(name, label, "number", required, false, defaultValue, null);
    }

    private static FieldDefinition Lookup(
        string name,
        string label,
        string lookupSource,
        bool required = false,
        string defaultValue = "")
    {
        return new FieldDefinition(name, label, "select", required, false, defaultValue, lookupSource);
    }

    private static FieldDefinition Decimal(string name, string label, bool required = false, string defaultValue = "0")
    {
        return new FieldDefinition(name, label, "number", required, false, defaultValue, null);
    }

    private static FieldDefinition Date(string name, string label, bool required = false)
    {
        return new FieldDefinition(name, label, "date", required, false, string.Empty, null);
    }

    private static FieldDefinition Bool(string name, string label, string defaultValue = "false")
    {
        return new FieldDefinition(name, label, "checkbox", false, false, defaultValue, null);
    }

    private static string SplitName(string name)
    {
        return string.Concat(name.Select((character, index) =>
            index > 0 && char.IsUpper(character) ? " " + character : character.ToString()));
    }

    private sealed record ModuleDefinition(
        string ModuleCode,
        string Title,
        string Controller,
        Type EntityType,
        IReadOnlyList<FieldDefinition> Fields)
    {
        public IReadOnlyList<FieldDefinition> ListFields => Fields.Take(6).ToList();

        public IReadOnlyList<string> SupportedActions =>
        [
            CrmPermissionActionConstants.View,
            CrmPermissionActionConstants.Add,
            CrmPermissionActionConstants.Edit,
            CrmPermissionActionConstants.Delete
        ];
    }

    private sealed record FieldDefinition(
        string Name,
        string Label,
        string FieldType,
        bool IsRequired,
        bool IsReadOnly,
        string DefaultValue,
        string? LookupSource)
    {
        public CrmFieldDto ToDto()
        {
            return new CrmFieldDto
            {
                Name = Name,
                Label = Label,
                FieldType = FieldType,
                IsRequired = IsRequired,
                IsReadOnly = IsReadOnly,
                Value = DefaultValue
            };
        }
    }
}
