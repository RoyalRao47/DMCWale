namespace DMCWale.Data.Constants;

public static class CrmPermissionActionConstants
{
    public const string View = "View";
    public const string Add = "Add";
    public const string Edit = "Edit";
    public const string Delete = "Delete";
    public const string Approve = "Approve";
    public const string Reject = "Reject";
    public const string Assign = "Assign";
    public const string Reassign = "Reassign";
    public const string Override = "Override";
    public const string Export = "Export";
    public const string Execute = "Execute";
    public const string RecordPayment = "RecordPayment";

    public static readonly IReadOnlyList<string> All =
    [
        View,
        Add,
        Edit,
        Delete,
        Approve,
        Reject,
        Assign,
        Reassign,
        Override,
        Export,
        Execute,
        RecordPayment
    ];
}
