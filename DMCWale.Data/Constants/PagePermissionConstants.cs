namespace DMCWale.Data.Constants;

public static class PagePermissionConstants
{
    public const string ClaimType = "PagePermission";

    public const string DashboardView = "Dashboard.View";

    public const string ProfileView = "Profile.View";

    public const string BookingsManageBookings = "Bookings.ManageBookings";
    public const string BookingsAddBooking = "Bookings.AddBooking";
    public const string BookingsManageEnquiry = "Bookings.ManageEnquiry";
    public const string BookingsReports = "Bookings.Reports";
    public const string BookingsQuery = "Bookings.Query";

    public const string HotelView = "Hotel.View";
    public const string HotelAdd = "Hotel.Add";

    public const string ActivityView = "Activity.View";
    public const string ActivityAdd = "Activity.Add";

    public const string TransferView = "Transfer.View";
    public const string TransferAdd = "Transfer.Add";

    public const string MealView = "Meal.View";
    public const string MealAdd = "Meal.Add";

    public const string VisaView = "Visa.View";
    public const string VisaAdd = "Visa.Add";

    public const string PackageView = "Package.View";
    public const string PackageAdd = "Package.Add";

    public const string BalanceSheetView = "BalanceSheet.View";
    public const string BalanceSheetAdd = "BalanceSheet.Add";

    public const string ComboPackagesView = "ComboPackages.View";
    public const string ComboPackagesAdd = "ComboPackages.Add";

    public const string PaymentsView = "Payments.View";
    public const string PaymentsAdd = "Payments.Add";

    public const string CouponView = "Coupon.View";
    public const string CouponAdd = "Coupon.Add";

    public const string MarketingView = "Marketing.View";
    public const string MarketingAdd = "Marketing.Add";

    public const string BlogView = "Blog.View";
    public const string BlogAdd = "Blog.Add";

    public static readonly IReadOnlyList<PagePermissionDefinition> All = BuildAll();

    public static string BuildClaimValue(string moduleCode, string permissionCode)
    {
        return $"{moduleCode}.{permissionCode}";
    }

    private static IReadOnlyList<PagePermissionDefinition> BuildAll()
    {
        var permissions = new List<PagePermissionDefinition>
        {
            new("Dashboard", "Dashboard", "View Dashboard", DashboardView),
            new("Profile", "Profile", "View Profile", ProfileView),

            new("Bookings", "Bookings", "Manage Bookings", BookingsManageBookings),
            new("Bookings", "Bookings", "Add Booking", BookingsAddBooking),
            new("Bookings", "Bookings", "Manage Enquiry", BookingsManageEnquiry),
            new("Bookings", "Bookings", "Reports", BookingsReports),
            new("Bookings", "Bookings", "Query", BookingsQuery),

            new("Hotel", "Hotel", "Manage Hotel", HotelView),
            new("Hotel", "Hotel", "Add Hotel", HotelAdd),

            new("Activity", "Activity", "Manage Activity", ActivityView),
            new("Activity", "Activity", "Add Activity", ActivityAdd),

            new("Transfer", "Transfer", "Manage Transfer", TransferView),
            new("Transfer", "Transfer", "Add Transfer", TransferAdd),

            new("Meal", "Meal", "Manage Meal", MealView),
            new("Meal", "Meal", "Add Meal", MealAdd),

            new("Visa", "Visa", "Manage Visa", VisaView),
            new("Visa", "Visa", "Add Visa", VisaAdd),

            new("Package", "Package", "Manage Package", PackageView),
            new("Package", "Package", "Add Package", PackageAdd),

            new("Balance Sheet", "BalanceSheet", "View Balance Sheet", BalanceSheetView),
            new("Balance Sheet", "BalanceSheet", "Add Balance Sheet", BalanceSheetAdd),

            new("Combo Packages", "ComboPackages", "Manage Combo Packages", ComboPackagesView),
            new("Combo Packages", "ComboPackages", "Add Combo Package", ComboPackagesAdd),

            new("Payments", "Payments", "Manage Payments", PaymentsView),
            new("Payments", "Payments", "Add Payment", PaymentsAdd),

            new("Coupon", "Coupon", "Manage Coupon", CouponView),
            new("Coupon", "Coupon", "Add Coupon", CouponAdd),

            new("Marketing", "Marketing", "Manage Marketing", MarketingView),
            new("Marketing", "Marketing", "Add Marketing", MarketingAdd),

            new("Blog", "Blog", "Manage Blog", BlogView),
            new("Blog", "Blog", "Add Blog", BlogAdd)
        };

        foreach (var definition in CrmPagePermissionCatalog.Modules)
        {
            permissions.AddRange(definition.PermissionCodes.Select(permissionCode =>
                new PagePermissionDefinition(
                    definition.Name,
                    definition.Code,
                    $"{GetPermissionDisplayName(permissionCode)} {definition.Name}",
                    BuildClaimValue(definition.Code, permissionCode))));
        }

        return permissions
            .GroupBy(permission => permission.ClaimValue, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    private static string GetPermissionDisplayName(string permissionCode)
    {
        return permissionCode switch
        {
            CrmPermissionActionConstants.RecordPayment => "Record Payment",
            _ => permissionCode
        };
    }
}

public sealed record PagePermissionDefinition(
    string Module,
    string Key,
    string Name,
    string ClaimValue);

public sealed record CrmPageModuleDefinition(
    string Name,
    string Code,
    string Area,
    string Controller,
    string Action,
    int DisplayOrder,
    IReadOnlyList<string> PermissionCodes,
    IReadOnlyDictionary<string, IReadOnlyList<string>> DefaultRolePermissions);

public static class CrmPagePermissionCatalog
{
    public static readonly IReadOnlyList<CrmPageModuleDefinition> Modules =
    [
        Module("Admin Dashboard", CrmModuleConstants.AdminDashboard, "Dashboard", "Admin", 10, [CrmPermissionActionConstants.View], AdminOnly()),
        Module("Staff Dashboard", CrmModuleConstants.StaffDashboard, "Dashboard", "Staff", 20, [CrmPermissionActionConstants.View], Roles(RoleConstants.Staff)),
        Module("Agent Dashboard", CrmModuleConstants.AgentDashboard, "Dashboard", "Agent", 30, [CrmPermissionActionConstants.View], Roles(RoleConstants.Agent)),
        Module("Supplier Dashboard", CrmModuleConstants.SupplierDashboard, "Dashboard", "Supplier", 40, [CrmPermissionActionConstants.View], Roles(RoleConstants.Supplier)),
        Module("User Management", CrmModuleConstants.UserManagement, "User", "Index", 50, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Delete], AdminOnly()),
        Module("Role Permissions", CrmModuleConstants.RolePermission, "RolePermission", "Index", 60, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Edit], AdminOnly()),
        Module("Destinations", CrmModuleConstants.Destination, "Destination", "Index", 70, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Delete], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent)),
        Module("Markup", CrmModuleConstants.Markup, "Markup", "Index", 80, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit], AdminOnly()),
        Module("Currencies", CrmModuleConstants.Currency, "Currency", "Index", 90, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit], AdminOnly()),
        Module("Exchange Rates", CrmModuleConstants.ExchangeRate, "ExchangeRate", "Index", 100, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Override], AdminOnly()),
        Module("Leads", CrmModuleConstants.Lead, "Lead", "Index", 110, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Approve, CrmPermissionActionConstants.Reject, CrmPermissionActionConstants.Assign, CrmPermissionActionConstants.Reassign], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent)),
        Module("Packages", CrmModuleConstants.Package, "Package", "Index", 120, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Delete], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent)),
        Module("Quotations", CrmModuleConstants.Quotation, "Quotation", "Index", 130, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Export], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent)),
        Module("Bookings", CrmModuleConstants.Booking, "Booking", "Index", 140, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Assign, CrmPermissionActionConstants.Reassign, CrmPermissionActionConstants.Override], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent, RoleConstants.Supplier)),
        Module("Payments", CrmModuleConstants.Payment, "Payment", "Index", 150, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.RecordPayment, CrmPermissionActionConstants.Export], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent)),
        Module("Execution", CrmModuleConstants.Execution, "Execution", "Index", 160, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Edit, CrmPermissionActionConstants.Execute], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Supplier)),
        Module("Reports", CrmModuleConstants.Report, "Report", "Index", 170, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Export], Roles(RoleConstants.Admin, RoleConstants.Staff)),
        Module("API Providers", CrmModuleConstants.ApiProvider, "ApiProvider", "Index", 180, [CrmPermissionActionConstants.View, CrmPermissionActionConstants.Add, CrmPermissionActionConstants.Edit], Roles(RoleConstants.Admin)),
        Module("Inventory Search", CrmModuleConstants.InventorySearch, "InventorySearch", "Hotel", 190, [CrmPermissionActionConstants.View], Roles(RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Agent, RoleConstants.Supplier))
    ];

    private static CrmPageModuleDefinition Module(
        string name,
        string code,
        string controller,
        string action,
        int displayOrder,
        IReadOnlyList<string> permissionCodes,
        IReadOnlyDictionary<string, IReadOnlyList<string>> defaultRolePermissions)
    {
        return new CrmPageModuleDefinition(name, code, string.Empty, controller, action, displayOrder, permissionCodes, defaultRolePermissions);
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<string>> AdminOnly()
    {
        return Roles(RoleConstants.Admin);
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<string>> Roles(params string[] roleNames)
    {
        return roleNames.ToDictionary(
            roleName => roleName,
            roleName => roleName == RoleConstants.Admin
                ? CrmPermissionActionConstants.All
                : (IReadOnlyList<string>)[CrmPermissionActionConstants.View],
            StringComparer.OrdinalIgnoreCase);
    }
}
