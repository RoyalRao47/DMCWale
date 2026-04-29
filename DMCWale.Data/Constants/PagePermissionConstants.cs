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

    public static readonly IReadOnlyList<PagePermissionDefinition> All =
    [
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
    ];
}

public sealed record PagePermissionDefinition(
    string Module,
    string Key,
    string Name,
    string ClaimValue);
