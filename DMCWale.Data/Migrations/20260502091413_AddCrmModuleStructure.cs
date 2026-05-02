using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMCWale.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCrmModuleStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsBaseCurrency = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiInventoryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ResponseJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiInventoryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiInventoryLogs_ApiProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ApiProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyId = table.Column<int>(type: "int", nullable: false),
                    ToCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    RateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentDestinationMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    AgentUserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentDestinationMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentDestinationMappings_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgentDestinationMappings_Users_AgentUserId",
                        column: x => x.AgentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    TravelStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TravelEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CustomerMobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CustomerRequirement = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LeadStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedStaffId = table.Column<int>(type: "int", nullable: true),
                    AssignedAgentId = table.Column<int>(type: "int", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Users_AssignedAgentId",
                        column: x => x.AssignedAgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Users_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkupRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationId = table.Column<int>(type: "int", nullable: true),
                    MarkupType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarkupPercentage = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkupRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarkupRules_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageModuleId = table.Column<int>(type: "int", nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagePermissions_PageModules_PageModuleId",
                        column: x => x.PageModuleId,
                        principalTable: "PageModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<int>(type: "int", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    AssignedByUserId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadAssignmentHistories_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadAssignmentHistories_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeadAssignmentHistories_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeadAssignmentHistories_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadNotes_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadNotes_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    PackageReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    CreatedByAgentId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MarkupPercentage = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PackageStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Users_CreatedByAgentId",
                        column: x => x.CreatedByAgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePagePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PagePermissionId = table.Column<int>(type: "int", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePagePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePagePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePagePermissions_PagePermissions_PagePermissionId",
                        column: x => x.PagePermissionId,
                        principalTable: "PagePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    DayNo = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageDays_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    QuotationReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    ItineraryText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    InclusionText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExclusionText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotations_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackageServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageDayId = table.Column<int>(type: "int", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MarkupPercentage = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsApiInventory = table.Column<bool>(type: "bit", nullable: false),
                    ApiProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApiReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageServices_PackageDays_PackageDayId",
                        column: x => x.PackageDayId,
                        principalTable: "PackageDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuotationId = table.Column<int>(type: "int", nullable: false),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CustomerMobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TravelStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TravelEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BookingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedStaffId = table.Column<int>(type: "int", nullable: true),
                    AssignedSupplierId = table.Column<int>(type: "int", nullable: true),
                    IsHighPriority = table.Column<bool>(type: "bit", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_AssignedSupplierId",
                        column: x => x.AssignedSupplierId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationId = table.Column<int>(type: "int", nullable: false),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationVersions_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationVersions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ExecutionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConfirmationReference = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingExecutions_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingExecutions_Users_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistories_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    PaymentReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PaymentLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordedByUserId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentDestinationMappings_AgentUserId_DestinationId",
                table: "AgentDestinationMappings",
                columns: new[] { "AgentUserId", "DestinationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentDestinationMappings_DestinationId",
                table: "AgentDestinationMappings",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiInventoryLogs_ProviderId",
                table: "ApiInventoryLogs",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingExecutions_BookingId",
                table: "BookingExecutions",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingExecutions_SupplierId",
                table: "BookingExecutions",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AssignedStaffId",
                table: "Bookings",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AssignedSupplierId",
                table: "Bookings",
                column: "AssignedSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingReferenceNo",
                table: "Bookings",
                column: "BookingReferenceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DestinationId",
                table: "Bookings",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LeadId",
                table: "Bookings",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_QuotationId",
                table: "Bookings",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistories_BookingId",
                table: "BookingStatusHistories",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistories_ChangedByUserId",
                table: "BookingStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_Code",
                table: "Destinations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_FromCurrencyId",
                table: "ExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ToCurrencyId",
                table: "ExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentHistories_AssignedByUserId",
                table: "LeadAssignmentHistories",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentHistories_FromUserId",
                table: "LeadAssignmentHistories",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentHistories_LeadId",
                table: "LeadAssignmentHistories",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentHistories_ToUserId",
                table: "LeadAssignmentHistories",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadNotes_AddedByUserId",
                table: "LeadNotes",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadNotes_LeadId",
                table: "LeadNotes",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_AssignedAgentId",
                table: "Leads",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_AssignedStaffId",
                table: "Leads",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_DestinationId",
                table: "Leads",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_LeadReferenceNo",
                table: "Leads",
                column: "LeadReferenceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarkupRules_DestinationId",
                table: "MarkupRules",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDays_PackageId",
                table: "PackageDays",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CreatedByAgentId",
                table: "Packages",
                column: "CreatedByAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DestinationId",
                table: "Packages",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_LeadId",
                table: "Packages",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PackageReferenceNo",
                table: "Packages",
                column: "PackageReferenceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageServices_PackageDayId",
                table: "PackageServices",
                column: "PackageDayId");

            migrationBuilder.CreateIndex(
                name: "IX_PageModules_Code",
                table: "PageModules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PagePermissions_PageModuleId_PermissionCode",
                table: "PagePermissions",
                columns: new[] { "PageModuleId", "PermissionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CurrencyId",
                table: "Payments",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentReferenceNo",
                table: "Payments",
                column: "PaymentReferenceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RecordedByUserId",
                table: "Payments",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_PackageId",
                table: "Quotations",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_QuotationReferenceNo",
                table: "Quotations",
                column: "QuotationReferenceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationVersions_CreatedByUserId",
                table: "QuotationVersions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationVersions_QuotationId",
                table: "QuotationVersions",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePagePermissions_PagePermissionId",
                table: "RolePagePermissions",
                column: "PagePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePagePermissions_RoleId_PagePermissionId",
                table: "RolePagePermissions",
                columns: new[] { "RoleId", "PagePermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentDestinationMappings");

            migrationBuilder.DropTable(
                name: "ApiInventoryLogs");

            migrationBuilder.DropTable(
                name: "BookingExecutions");

            migrationBuilder.DropTable(
                name: "BookingStatusHistories");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "LeadAssignmentHistories");

            migrationBuilder.DropTable(
                name: "LeadNotes");

            migrationBuilder.DropTable(
                name: "MarkupRules");

            migrationBuilder.DropTable(
                name: "PackageServices");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "QuotationVersions");

            migrationBuilder.DropTable(
                name: "RolePagePermissions");

            migrationBuilder.DropTable(
                name: "ApiProviders");

            migrationBuilder.DropTable(
                name: "PackageDays");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "PagePermissions");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "PageModules");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Destinations");
        }
    }
}
