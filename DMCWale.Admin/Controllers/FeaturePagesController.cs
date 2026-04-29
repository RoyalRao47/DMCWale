using DMCWale.Admin.Filters;
using DMCWale.Admin.ViewModels;
using DMCWale.Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize]
public class FeaturePagesController : Controller
{
    [HttpGet("/Bookings/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.BookingsManageBookings)]
    public IActionResult ManageBookings()
    {
        return FeaturePage("Bookings", "Manage Bookings");
    }

    [HttpGet("/Bookings/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.BookingsAddBooking)]
    public IActionResult AddBooking()
    {
        return FeaturePage("Bookings", "Add Booking");
    }

    [HttpGet("/Bookings/Enquiry")]
    [PagePermissionAuthorize(PagePermissionConstants.BookingsManageEnquiry)]
    public IActionResult ManageEnquiry()
    {
        return FeaturePage("Bookings", "Manage Enquiry");
    }

    [HttpGet("/Bookings/Reports")]
    [PagePermissionAuthorize(PagePermissionConstants.BookingsReports)]
    public IActionResult BookingReports()
    {
        return FeaturePage("Bookings", "Reports");
    }

    [HttpGet("/Bookings/Query")]
    [PagePermissionAuthorize(PagePermissionConstants.BookingsQuery)]
    public IActionResult BookingQuery()
    {
        return FeaturePage("Bookings", "Query");
    }

    [HttpGet("/Hotel/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.HotelView)]
    public IActionResult ManageHotel()
    {
        return FeaturePage("Hotel", "Manage Hotel");
    }

    [HttpGet("/Hotel/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.HotelAdd)]
    public IActionResult AddHotel()
    {
        return FeaturePage("Hotel", "Add Hotel");
    }

    [HttpGet("/Activity/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.ActivityView)]
    public IActionResult ManageActivity()
    {
        return FeaturePage("Activity", "Manage Activity");
    }

    [HttpGet("/Activity/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.ActivityAdd)]
    public IActionResult AddActivity()
    {
        return FeaturePage("Activity", "Add Activity");
    }

    [HttpGet("/Transfer/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.TransferView)]
    public IActionResult ManageTransfer()
    {
        return FeaturePage("Transfer", "Manage Transfer");
    }

    [HttpGet("/Transfer/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.TransferAdd)]
    public IActionResult AddTransfer()
    {
        return FeaturePage("Transfer", "Add Transfer");
    }

    [HttpGet("/Meal/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.MealView)]
    public IActionResult ManageMeal()
    {
        return FeaturePage("Meal", "Manage Meal");
    }

    [HttpGet("/Meal/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.MealAdd)]
    public IActionResult AddMeal()
    {
        return FeaturePage("Meal", "Add Meal");
    }

    [HttpGet("/Visa/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.VisaView)]
    public IActionResult ManageVisa()
    {
        return FeaturePage("Visa", "Manage Visa");
    }

    [HttpGet("/Visa/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.VisaAdd)]
    public IActionResult AddVisa()
    {
        return FeaturePage("Visa", "Add Visa");
    }

    [HttpGet("/Package/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.PackageView)]
    public IActionResult ManagePackage()
    {
        return FeaturePage("Package", "Manage Package");
    }

    [HttpGet("/Package/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.PackageAdd)]
    public IActionResult AddPackage()
    {
        return FeaturePage("Package", "Add Package");
    }

    [HttpGet("/BalanceSheet/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.BalanceSheetView)]
    public IActionResult ManageBalanceSheet()
    {
        return FeaturePage("Balance Sheet", "View Balance Sheet");
    }

    [HttpGet("/BalanceSheet/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.BalanceSheetAdd)]
    public IActionResult AddBalanceSheet()
    {
        return FeaturePage("Balance Sheet", "Add Balance Sheet");
    }

    [HttpGet("/ComboPackages/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.ComboPackagesView)]
    public IActionResult ManageComboPackages()
    {
        return FeaturePage("Combo Packages", "Manage Combo Packages");
    }

    [HttpGet("/ComboPackages/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.ComboPackagesAdd)]
    public IActionResult AddComboPackage()
    {
        return FeaturePage("Combo Packages", "Add Combo Package");
    }

    [HttpGet("/Payments/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.PaymentsView)]
    public IActionResult ManagePayments()
    {
        return FeaturePage("Payments", "Manage Payments");
    }

    [HttpGet("/Payments/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.PaymentsAdd)]
    public IActionResult AddPayment()
    {
        return FeaturePage("Payments", "Add Payment");
    }

    [HttpGet("/Coupon/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.CouponView)]
    public IActionResult ManageCoupon()
    {
        return FeaturePage("Coupon", "Manage Coupon");
    }

    [HttpGet("/Coupon/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.CouponAdd)]
    public IActionResult AddCoupon()
    {
        return FeaturePage("Coupon", "Add Coupon");
    }

    [HttpGet("/Marketing/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.MarketingView)]
    public IActionResult ManageMarketing()
    {
        return FeaturePage("Marketing", "Manage Marketing");
    }

    [HttpGet("/Marketing/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.MarketingAdd)]
    public IActionResult AddMarketing()
    {
        return FeaturePage("Marketing", "Add Marketing");
    }

    [HttpGet("/Blog/Manage")]
    [PagePermissionAuthorize(PagePermissionConstants.BlogView)]
    public IActionResult ManageBlog()
    {
        return FeaturePage("Blog", "Manage Blog");
    }

    [HttpGet("/Blog/Add")]
    [PagePermissionAuthorize(PagePermissionConstants.BlogAdd)]
    public IActionResult AddBlog()
    {
        return FeaturePage("Blog", "Add Blog");
    }

    private IActionResult FeaturePage(string module, string title)
    {
        return View("FeaturePage", new FeaturePageViewModel
        {
            Module = module,
            Title = title
        });
    }
}
