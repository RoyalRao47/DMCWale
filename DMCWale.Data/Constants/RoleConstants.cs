namespace DMCWale.Data.Constants;

public static class RoleConstants
{
    public const string Admin = "Admin";
    public const string Agent = "Agent";
    public const string Staff = "Staff";
    public const string Supplier = "Supplier";

    public static readonly IReadOnlyList<string> DefaultRoles =
    [
        Admin,
        Agent,
        Staff,
        Supplier
    ];
}
