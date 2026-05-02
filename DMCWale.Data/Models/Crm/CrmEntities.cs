using DMCWale.Data.Models.Identity;

namespace DMCWale.Data.Models.Crm;

public class PageModule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public ICollection<PagePermission> Permissions { get; set; } = [];
}

public class PagePermission
{
    public int Id { get; set; }
    public int PageModuleId { get; set; }
    public PageModule PageModule { get; set; } = null!;
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public ICollection<RolePagePermission> RolePagePermissions { get; set; } = [];
}

public class RolePagePermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public ApplicationRole Role { get; set; } = null!;
    public int PagePermissionId { get; set; }
    public PagePermission PagePermission { get; set; } = null!;
    public bool IsAllowed { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class Destination
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class AgentDestinationMapping
{
    public int Id { get; set; }
    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;
    public int AgentUserId { get; set; }
    public User AgentUser { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class MarkupRule
{
    public int Id { get; set; }
    public int? DestinationId { get; set; }
    public Destination? Destination { get; set; }
    public string MarkupType { get; set; } = string.Empty;
    public decimal MarkupPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class Currency
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public bool IsBaseCurrency { get; set; }
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class ExchangeRate
{
    public int Id { get; set; }
    public int FromCurrencyId { get; set; }
    public Currency FromCurrency { get; set; } = null!;
    public int ToCurrencyId { get; set; }
    public Currency ToCurrency { get; set; } = null!;
    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; }
    public bool IsLocked { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class Lead
{
    public int Id { get; set; }
    public string LeadReferenceNo { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;
    public DateTime TravelStartDate { get; set; }
    public DateTime TravelEndDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerMobile { get; set; } = string.Empty;
    public string CustomerRequirement { get; set; } = string.Empty;
    public string LeadStatus { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int? AssignedStaffId { get; set; }
    public User? AssignedStaff { get; set; }
    public int? AssignedAgentId { get; set; }
    public User? AssignedAgent { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class LeadNote
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    public string Note { get; set; } = string.Empty;
    public int AddedByUserId { get; set; }
    public User AddedByUser { get; set; } = null!;
    public DateTime AddDate { get; set; }
}

public class LeadAssignmentHistory
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    public int? FromUserId { get; set; }
    public User? FromUser { get; set; }
    public int ToUserId { get; set; }
    public User ToUser { get; set; } = null!;
    public int AssignedByUserId { get; set; }
    public User AssignedByUser { get; set; } = null!;
    public string Remarks { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
}

public class Package
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    public string PackageReferenceNo { get; set; } = string.Empty;
    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;
    public int CreatedByAgentId { get; set; }
    public User CreatedByAgent { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public decimal MarkupPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public string PackageStatus { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public ICollection<PackageDay> Days { get; set; } = [];
}

public class PackageDay
{
    public int Id { get; set; }
    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;
    public int DayNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public ICollection<PackageService> Services { get; set; } = [];
}

public class PackageService
{
    public int Id { get; set; }
    public int PackageDayId { get; set; }
    public PackageDay PackageDay { get; set; } = null!;
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal MarkupPercentage { get; set; }
    public decimal SellingPrice { get; set; }
    public bool IsApiInventory { get; set; }
    public string ApiProvider { get; set; } = string.Empty;
    public string ApiReference { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class Quotation
{
    public int Id { get; set; }
    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;
    public string QuotationReferenceNo { get; set; } = string.Empty;
    public int VersionNo { get; set; }
    public string ItineraryText { get; set; } = string.Empty;
    public string InclusionText { get; set; } = string.Empty;
    public string ExclusionText { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SentDate { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class QuotationVersion
{
    public int Id { get; set; }
    public int QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;
    public int VersionNo { get; set; }
    public string SnapshotJson { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public DateTime AddDate { get; set; }
}

public class Booking
{
    public int Id { get; set; }
    public string BookingReferenceNo { get; set; } = string.Empty;
    public int QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerMobile { get; set; } = string.Empty;
    public DateTime TravelStartDate { get; set; }
    public DateTime TravelEndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public int? AssignedStaffId { get; set; }
    public User? AssignedStaff { get; set; }
    public int? AssignedSupplierId { get; set; }
    public User? AssignedSupplier { get; set; }
    public bool IsHighPriority { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class BookingStatusHistory
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public int ChangedByUserId { get; set; }
    public User ChangedByUser { get; set; } = null!;
    public string Remarks { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public string PaymentReferenceNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
    public string PaymentMode { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? PaymentLink { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int? RecordedByUserId { get; set; }
    public User? RecordedByUser { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class BookingExecution
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int SupplierId { get; set; }
    public User Supplier { get; set; } = null!;
    public string ExecutionStatus { get; set; } = string.Empty;
    public string ConfirmationReference { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class ApiProvider
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProviderType { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime AddDate { get; set; }
    public DateTime? ModifyDate { get; set; }
}

public class ApiInventoryLog
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public ApiProvider Provider { get; set; } = null!;
    public string RequestType { get; set; } = string.Empty;
    public string RequestJson { get; set; } = string.Empty;
    public string ResponseJson { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AddDate { get; set; }
}
