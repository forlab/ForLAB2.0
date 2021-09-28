using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForLab.Data.Migrations
{
    public partial class AddTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.EnsureSchema(
                name: "CMS");

            migrationBuilder.EnsureSchema(
                name: "Configuration");

            migrationBuilder.EnsureSchema(
                name: "DiseaseProgram");

            migrationBuilder.EnsureSchema(
                name: "Disease");

            migrationBuilder.EnsureSchema(
                name: "Forecasting");

            migrationBuilder.EnsureSchema(
                name: "Laboratory");

            migrationBuilder.EnsureSchema(
                name: "Lookup");

            migrationBuilder.EnsureSchema(
                name: "lookup");

            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.EnsureSchema(
                name: "Testing");

            migrationBuilder.EnsureSchema(
                name: "Vendor");

            migrationBuilder.CreateTable(
                name: "Configurations",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumOfDaysToChangePassword = table.Column<int>(nullable: false),
                    AccountLoginAttempts = table.Column<int>(nullable: false),
                    PasswordExpiryTime = table.Column<int>(nullable: false),
                    UserPhotosize = table.Column<double>(nullable: false),
                    AttachmentsMaxSize = table.Column<double>(nullable: false),
                    TimesCountBeforePasswordReuse = table.Column<int>(nullable: false),
                    TimeToSessionTimeOut = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTransactionTypes",
                schema: "lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalculationPeriods",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Continents",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true),
                    ShortCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlRequirementUnits",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlRequirementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryPeriods",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForecastInfoLevels",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastInfoLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForecastMethodologies",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastMethodologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductBasicUnits",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBasicUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReagentSystems",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReagentSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScopeOfTheForecasts",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeOfTheForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThroughPutUnits",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThroughPutUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptionLevels",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptionLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VariableTypes",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariableTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalculationPeriodMonths",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Name = table.Column<string>(nullable: true),
                    CalculationPeriodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationPeriodMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculationPeriodMonths_CalculationPeriods_CalculationPeriodId",
                        column: x => x.CalculationPeriodId,
                        principalSchema: "Lookup",
                        principalTable: "CalculationPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false, defaultValue: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    UserSubscriptionLevelId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    PersonalImagePath = table.Column<string>(nullable: true),
                    IP = table.Column<string>(nullable: true),
                    ChangePassword = table.Column<bool>(nullable: false, defaultValue: false),
                    CallingCode = table.Column<string>(nullable: true),
                    JobTitle = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    NextPasswordExpiryDate = table.Column<DateTime>(nullable: false),
                    EmailVerifiedDate = table.Column<DateTime>(nullable: true),
                    ContinentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Continents_ContinentId",
                        column: x => x.ContinentId,
                        principalSchema: "Lookup",
                        principalTable: "Continents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_UserSubscriptionLevels_UserSubscriptionLevelId",
                        column: x => x.UserSubscriptionLevelId,
                        principalSchema: "Lookup",
                        principalTable: "UserSubscriptionLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Security",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ProvidedBy = table.Column<string>(nullable: true),
                    ProvidedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChannelVideos",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AttachmentUrl = table.Column<string>(nullable: true),
                    AttachmentSize = table.Column<float>(nullable: true),
                    AttachmentName = table.Column<string>(nullable: true),
                    ExtensionFormat = table.Column<string>(nullable: true),
                    IsExternalResource = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelVideos_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChannelVideos_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactInfos",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    LinkedIn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactInfos_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactInfos_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LogoPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Features_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FrequentlyAskedQuestions",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequentlyAskedQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrequentlyAskedQuestions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FrequentlyAskedQuestions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InquiryQuestions",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ReplyProvided = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryQuestions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryQuestions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsefulResources",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    AttachmentUrl = table.Column<string>(nullable: true),
                    AttachmentSize = table.Column<float>(nullable: true),
                    AttachmentName = table.Column<string>(nullable: true),
                    ExtensionFormat = table.Column<string>(nullable: true),
                    IsExternalResource = table.Column<bool>(nullable: false, defaultValue: false),
                    DownloadCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsefulResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsefulResources_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsefulResources_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationAudits",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfAction = table.Column<DateTime>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: false),
                    ConfigurationId = table.Column<int>(nullable: false),
                    NumOfDaysToChangePassword = table.Column<int>(nullable: false),
                    AccountLoginAttempts = table.Column<int>(nullable: false),
                    PasswordExpiryTime = table.Column<int>(nullable: false),
                    UserPhotosize = table.Column<double>(nullable: false),
                    AttachmentsMaxSize = table.Column<double>(nullable: false),
                    TimesCountBeforePasswordReuse = table.Column<int>(nullable: false),
                    TimeToSessionTimeOut = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationAudits_Configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalSchema: "Configuration",
                        principalTable: "Configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationAudits_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diseases",
                schema: "Disease",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diseases_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diseases_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ContinentId = table.Column<int>(nullable: false),
                    CountryPeriodId = table.Column<int>(nullable: false),
                    ShortCode = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    NativeName = table.Column<string>(nullable: true),
                    Flag = table.Column<string>(nullable: true),
                    CurrencyCode = table.Column<string>(nullable: true),
                    CallingCode = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Population = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Continents_ContinentId",
                        column: x => x.ContinentId,
                        principalSchema: "Lookup",
                        principalTable: "Continents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Countries_CountryPeriods_CountryPeriodId",
                        column: x => x.CountryPeriodId,
                        principalSchema: "Lookup",
                        principalTable: "CountryPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Countries_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Countries_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryCategories",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryCategories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryCategories_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryLevels",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryLevels_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryLevels_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientGroups",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientGroups_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientGroups_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestingAreas",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestingAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestingAreas_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingAreas_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Security",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    RoleId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.UniqueConstraint("AK_UserRoles_UserId_RoleId", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Security",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId1",
                        column: x => x.RoleId1,
                        principalSchema: "Security",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Security",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTransactionHistories",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    UserTransactionTypeId = table.Column<int>(nullable: false),
                    Location_IP = table.Column<string>(nullable: true),
                    Location_City = table.Column<string>(nullable: true),
                    Location_Region = table.Column<string>(nullable: true),
                    Location_RegionCode = table.Column<string>(nullable: true),
                    Location_CountryName = table.Column<string>(nullable: true),
                    Location_CountryCode = table.Column<string>(nullable: true),
                    Location_ContinentName = table.Column<string>(nullable: true),
                    Location_ContinentCode = table.Column<string>(nullable: true),
                    Location_Latitude = table.Column<float>(nullable: true),
                    Location_Longitude = table.Column<float>(nullable: true),
                    Location_ASN = table.Column<string>(nullable: true),
                    Location_Flag = table.Column<string>(nullable: true),
                    Location_Postal = table.Column<string>(nullable: true),
                    Location_CallingCode = table.Column<string>(nullable: true),
                    Location_RTL = table.Column<bool>(nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTransactionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTransactionHistories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTransactionHistories_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTransactionHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTransactionHistories_UserTransactionTypes_UserTransactionTypeId",
                        column: x => x.UserTransactionTypeId,
                        principalSchema: "lookup",
                        principalTable: "UserTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                schema: "Vendor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendors_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleImages",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ArticleId = table.Column<int>(nullable: false),
                    AttachmentUrl = table.Column<string>(nullable: true),
                    AttachmentSize = table.Column<float>(nullable: true),
                    AttachmentName = table.Column<string>(nullable: true),
                    ExtensionFormat = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false, defaultValue: false),
                    IsExternalResource = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleImages_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "CMS",
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticleImages_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticleImages_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InquiryQuestionReplies",
                schema: "CMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InquiryQuestionId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryQuestionReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryQuestionReplies_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryQuestionReplies_InquiryQuestions_InquiryQuestionId",
                        column: x => x.InquiryQuestionId,
                        principalSchema: "CMS",
                        principalTable: "InquiryQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryQuestionReplies_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                schema: "DiseaseProgram",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    DiseaseId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NumberOfYears = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programs_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalSchema: "Disease",
                        principalTable: "Diseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programs_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CountryDiseaseIncidents",
                schema: "Disease",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Incidence = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    IncidencePer1kPopulation = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    IncidencePer100kPopulation = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PrevalenceRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PrevalenceRatePer1kPopulation = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PrevalenceRatePer100kPopulation = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryDiseaseIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CountryDiseaseIncidents_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Lookup",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CountryDiseaseIncidents_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CountryDiseaseIncidents_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalSchema: "Disease",
                        principalTable: "Diseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CountryDiseaseIncidents_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    ShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Lookup",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCountrySubscriptions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ApplicationUserId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCountrySubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCountrySubscriptions_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCountrySubscriptions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Lookup",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCountrySubscriptions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCountrySubscriptions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                schema: "Testing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    TestingAreaId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tests_TestingAreas_TestingAreaId",
                        column: x => x.TestingAreaId,
                        principalSchema: "Lookup",
                        principalTable: "TestingAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tests_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    VendorId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MaxThroughPut = table.Column<int>(nullable: false),
                    ThroughPutUnitId = table.Column<int>(nullable: false),
                    ReagentSystemId = table.Column<int>(nullable: false),
                    ControlRequirement = table.Column<int>(nullable: false),
                    ControlRequirementUnitId = table.Column<int>(nullable: false),
                    TestingAreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instruments_ControlRequirementUnits_ControlRequirementUnitId",
                        column: x => x.ControlRequirementUnitId,
                        principalSchema: "Lookup",
                        principalTable: "ControlRequirementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Instruments_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Instruments_ReagentSystems_ReagentSystemId",
                        column: x => x.ReagentSystemId,
                        principalSchema: "Lookup",
                        principalTable: "ReagentSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Instruments_TestingAreas_TestingAreaId",
                        column: x => x.TestingAreaId,
                        principalSchema: "Lookup",
                        principalTable: "TestingAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Instruments_ThroughPutUnits_ThroughPutUnitId",
                        column: x => x.ThroughPutUnitId,
                        principalSchema: "Lookup",
                        principalTable: "ThroughPutUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Instruments_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Instruments_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Vendor",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    VendorId = table.Column<int>(nullable: false),
                    ManufacturerPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    ProductTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CatalogNo = table.Column<string>(nullable: true),
                    ProductBasicUnitId = table.Column<int>(nullable: false),
                    PackSize = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_ProductBasicUnits_ProductBasicUnitId",
                        column: x => x.ProductBasicUnitId,
                        principalSchema: "Lookup",
                        principalTable: "ProductBasicUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalSchema: "Lookup",
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Vendor",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorContacts",
                schema: "Vendor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    VendorId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorContacts_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorContacts_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorContacts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Vendor",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientAssumptionParameters",
                schema: "DiseaseProgram",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProgramId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsNumeric = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPercentage = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPositive = table.Column<bool>(nullable: false, defaultValue: false),
                    IsNegative = table.Column<bool>(nullable: false, defaultValue: false),
                    EntityTypeId = table.Column<int>(nullable: true),
                    VariableTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAssumptionParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAssumptionParameters_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssumptionParameters_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalSchema: "Lookup",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssumptionParameters_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssumptionParameters_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssumptionParameters_VariableTypes_VariableTypeId",
                        column: x => x.VariableTypeId,
                        principalSchema: "Lookup",
                        principalTable: "VariableTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAssumptionParameters",
                schema: "DiseaseProgram",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProgramId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsNumeric = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPercentage = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPositive = table.Column<bool>(nullable: false, defaultValue: false),
                    IsNegative = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAssumptionParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAssumptionParameters_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAssumptionParameters_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAssumptionParameters_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestingAssumptionParameters",
                schema: "DiseaseProgram",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProgramId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsNumeric = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPercentage = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPositive = table.Column<bool>(nullable: false, defaultValue: false),
                    IsNegative = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestingAssumptionParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestingAssumptionParameters_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingAssumptionParameters_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingAssumptionParameters_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Laboratories",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    RegionId = table.Column<int>(nullable: false),
                    LaboratoryCategoryId = table.Column<int>(nullable: false),
                    LaboratoryLevelId = table.Column<int>(nullable: false),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laboratories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Laboratories_LaboratoryCategories_LaboratoryCategoryId",
                        column: x => x.LaboratoryCategoryId,
                        principalSchema: "Lookup",
                        principalTable: "LaboratoryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Laboratories_LaboratoryLevels_LaboratoryLevelId",
                        column: x => x.LaboratoryLevelId,
                        principalSchema: "Lookup",
                        principalTable: "LaboratoryLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Laboratories_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "Lookup",
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Laboratories_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRegionSubscriptions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ApplicationUserId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRegionSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRegionSubscriptions_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRegionSubscriptions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRegionSubscriptions_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "Lookup",
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRegionSubscriptions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestingProtocols",
                schema: "Testing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TestId = table.Column<int>(nullable: false),
                    PatientGroupId = table.Column<int>(nullable: false),
                    CalculationPeriodId = table.Column<int>(nullable: false),
                    BaseLine = table.Column<int>(nullable: false),
                    TestAfterFirstYear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestingProtocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestingProtocols_CalculationPeriods_CalculationPeriodId",
                        column: x => x.CalculationPeriodId,
                        principalSchema: "Lookup",
                        principalTable: "CalculationPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocols_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocols_PatientGroups_PatientGroupId",
                        column: x => x.PatientGroupId,
                        principalSchema: "Lookup",
                        principalTable: "PatientGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocols_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocols_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CountryProductPrices",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PackSize = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    FromDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CountryProductPrices_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Lookup",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CountryProductPrices_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CountryProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CountryProductPrices_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductUsage",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    TestId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    IsForControl = table.Column<bool>(nullable: false, defaultValue: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PerPeriod = table.Column<bool>(nullable: false, defaultValue: false),
                    PerPeriodPerInstrument = table.Column<bool>(nullable: false, defaultValue: false),
                    CountryPeriodId = table.Column<int>(nullable: true),
                    InstrumentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductUsage_CountryPeriods_CountryPeriodId",
                        column: x => x.CountryPeriodId,
                        principalSchema: "Lookup",
                        principalTable: "CountryPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductUsage_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductUsage_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalSchema: "Product",
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductUsage_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductUsage_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductUsage_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionProductPrices",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PackSize = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    FromDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionProductPrices_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionProductPrices_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "Lookup",
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionProductPrices_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastInfos",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoLevelId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    ForecastMethodologyId = table.Column<int>(nullable: false),
                    ScopeOfTheForecastId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    IsAggregate = table.Column<bool>(nullable: false, defaultValue: false),
                    IsSiteBySite = table.Column<bool>(nullable: false, defaultValue: false),
                    IsWorldHealthOrganization = table.Column<bool>(nullable: false, defaultValue: false),
                    IsTargetBased = table.Column<bool>(nullable: false, defaultValue: false),
                    WastageRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    LaboratoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Lookup",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_ForecastInfoLevels_ForecastInfoLevelId",
                        column: x => x.ForecastInfoLevelId,
                        principalSchema: "Lookup",
                        principalTable: "ForecastInfoLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_ForecastMethodologies_ForecastMethodologyId",
                        column: x => x.ForecastMethodologyId,
                        principalSchema: "Lookup",
                        principalTable: "ForecastMethodologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_ScopeOfTheForecasts_ScopeOfTheForecastId",
                        column: x => x.ScopeOfTheForecastId,
                        principalSchema: "Lookup",
                        principalTable: "ScopeOfTheForecasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInfos_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryConsumptions",
                schema: "Laboratory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LaboratoryId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ConsumptionDuration = table.Column<DateTime>(nullable: false),
                    AmountUsed = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryConsumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryConsumptions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryConsumptions_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryConsumptions_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryConsumptions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryInstruments",
                schema: "Laboratory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InstrumentId = table.Column<int>(nullable: false),
                    LaboratoryId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TestRunPercentage = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryInstruments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryInstruments_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryInstruments_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalSchema: "Product",
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryInstruments_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryInstruments_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryPatientStatistics",
                schema: "Laboratory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LaboratoryId = table.Column<int>(nullable: false),
                    Period = table.Column<DateTime>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryPatientStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryPatientStatistics_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryPatientStatistics_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryPatientStatistics_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestServices",
                schema: "Laboratory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LaboratoryId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false),
                    ServiceDuration = table.Column<DateTime>(nullable: false),
                    TestPerformed = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestServices_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestServices_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestServices_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestServices_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryWorkingDays",
                schema: "Laboratory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LaboratoryId = table.Column<int>(nullable: false),
                    Day = table.Column<string>(nullable: true),
                    FromTime = table.Column<TimeSpan>(nullable: false),
                    ToTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryWorkingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryWorkingDays_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryWorkingDays_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryWorkingDays_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryProductPrices",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    LaboratoryId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PackSize = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    FromDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryProductPrices_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryProductPrices_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboratoryProductPrices_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLaboratorySubscriptions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ApplicationUserId = table.Column<int>(nullable: false),
                    LaboratoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLaboratorySubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLaboratorySubscriptions_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLaboratorySubscriptions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLaboratorySubscriptions_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLaboratorySubscriptions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiseaseTestingProtocols",
                schema: "Disease",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    DiseaseId = table.Column<int>(nullable: false),
                    TestingProtocolId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseTestingProtocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseTestingProtocols_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiseaseTestingProtocols_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalSchema: "Disease",
                        principalTable: "Diseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiseaseTestingProtocols_TestingProtocols_TestingProtocolId",
                        column: x => x.TestingProtocolId,
                        principalSchema: "Testing",
                        principalTable: "TestingProtocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiseaseTestingProtocols_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramTests",
                schema: "DiseaseProgram",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProgramId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false),
                    TestingProtocolId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramTests_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramTests_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramTests_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramTests_TestingProtocols_TestingProtocolId",
                        column: x => x.TestingProtocolId,
                        principalSchema: "Testing",
                        principalTable: "TestingProtocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramTests_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestingProtocolCalculationPeriodMonths",
                schema: "Testing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    TestingProtocolId = table.Column<int>(nullable: false),
                    CalculationPeriodMonthId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestingProtocolCalculationPeriodMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestingProtocolCalculationPeriodMonths_CalculationPeriodMonths_CalculationPeriodMonthId",
                        column: x => x.CalculationPeriodMonthId,
                        principalSchema: "Lookup",
                        principalTable: "CalculationPeriodMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocolCalculationPeriodMonths_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocolCalculationPeriodMonths_TestingProtocols_TestingProtocolId",
                        column: x => x.TestingProtocolId,
                        principalSchema: "Testing",
                        principalTable: "TestingProtocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestingProtocolCalculationPeriodMonths_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastCategories",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastCategories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastCategories_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastCategories_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastInstruments",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InstrumentId = table.Column<int>(nullable: false),
                    ForecastInfoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastInstruments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastInstruments_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInstruments_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInstruments_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalSchema: "Product",
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastInstruments_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastLaboratoryConsumptions",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    LaboratoryId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Period = table.Column<string>(nullable: true),
                    AmountForecasted = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastLaboratoryConsumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryConsumptions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryConsumptions_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryConsumptions_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryConsumptions_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryConsumptions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastLaboratoryTestServices",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    LaboratoryId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false),
                    Period = table.Column<string>(nullable: true),
                    AmountForecasted = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastLaboratoryTestServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryTestServices_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryTestServices_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryTestServices_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryTestServices_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratoryTestServices_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastMorbidityPrograms",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    ProgramId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastMorbidityPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityPrograms_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityPrograms_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityPrograms_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityPrograms_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastMorbidityTestingProtocolMonths",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    ProgramId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false),
                    PatientGroupId = table.Column<int>(nullable: false),
                    TestingProtocolId = table.Column<int>(nullable: false),
                    CalculationPeriodMonthId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastMorbidityTestingProtocolMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_CalculationPeriodMonths_CalculationPeriodMonthId",
                        column: x => x.CalculationPeriodMonthId,
                        principalSchema: "Lookup",
                        principalTable: "CalculationPeriodMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_PatientGroups_PatientGroupId",
                        column: x => x.PatientGroupId,
                        principalSchema: "Lookup",
                        principalTable: "PatientGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_TestingProtocols_TestingProtocolId",
                        column: x => x.TestingProtocolId,
                        principalSchema: "Testing",
                        principalTable: "TestingProtocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTestingProtocolMonths_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastMorbidityWhoBases",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    DiseaseId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    Period = table.Column<string>(nullable: true),
                    Count = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastMorbidityWhoBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityWhoBases_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "Lookup",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityWhoBases_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityWhoBases_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalSchema: "Disease",
                        principalTable: "Diseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityWhoBases_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityWhoBases_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastPatientAssumptionValues",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    PatientAssumptionParameterId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastPatientAssumptionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastPatientAssumptionValues_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientAssumptionValues_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientAssumptionValues_PatientAssumptionParameters_PatientAssumptionParameterId",
                        column: x => x.PatientAssumptionParameterId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "PatientAssumptionParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientAssumptionValues_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastPatientGroups",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    PatientGroupId = table.Column<int>(nullable: false),
                    ProgramId = table.Column<int>(nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastPatientGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastPatientGroups_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientGroups_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientGroups_PatientGroups_PatientGroupId",
                        column: x => x.PatientGroupId,
                        principalSchema: "Lookup",
                        principalTable: "PatientGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientGroups_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastPatientGroups_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastProductAssumptionValues",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ProductAssumptionParameterId = table.Column<int>(nullable: false),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastProductAssumptionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastProductAssumptionValues_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastProductAssumptionValues_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastProductAssumptionValues_ProductAssumptionParameters_ProductAssumptionParameterId",
                        column: x => x.ProductAssumptionParameterId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "ProductAssumptionParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastProductAssumptionValues_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastResults",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    LaboratoryId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    AmountForecasted = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    TotalValue = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    DurationDateTime = table.Column<DateTime>(nullable: false),
                    Period = table.Column<string>(nullable: true),
                    PackSize = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    PackQty = table.Column<int>(nullable: false),
                    PackPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    TotalPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    ProductTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastResults_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastResults_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastResults_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastResults_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastResults_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalSchema: "Lookup",
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastResults_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastResults_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastTestingAssumptionValues",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    TestingAssumptionParameterId = table.Column<int>(nullable: false),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastTestingAssumptionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastTestingAssumptionValues_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastTestingAssumptionValues_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastTestingAssumptionValues_TestingAssumptionParameters_TestingAssumptionParameterId",
                        column: x => x.TestingAssumptionParameterId,
                        principalSchema: "DiseaseProgram",
                        principalTable: "TestingAssumptionParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastTestingAssumptionValues_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastTests",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastTests_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastTests_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastTests_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "Testing",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastTests_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastLaboratories",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    ForecastCategoryId = table.Column<int>(nullable: true),
                    LaboratoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastLaboratories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratories_ForecastCategories_ForecastCategoryId",
                        column: x => x.ForecastCategoryId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratories_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratories_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "Lookup",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastLaboratories_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForecastMorbidityTargetBases",
                schema: "Forecasting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    ForecastInfoId = table.Column<int>(nullable: false),
                    ForecastLaboratoryId = table.Column<int>(nullable: false),
                    ForecastMorbidityProgramId = table.Column<int>(nullable: false),
                    CurrentPatient = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m),
                    TargetPatient = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastMorbidityTargetBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTargetBases_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTargetBases_ForecastInfos_ForecastInfoId",
                        column: x => x.ForecastInfoId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTargetBases_ForecastLaboratories_ForecastLaboratoryId",
                        column: x => x.ForecastLaboratoryId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastLaboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTargetBases_ForecastMorbidityPrograms_ForecastMorbidityProgramId",
                        column: x => x.ForecastMorbidityProgramId,
                        principalSchema: "Forecasting",
                        principalTable: "ForecastMorbidityPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastMorbidityTargetBases_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleImages_ArticleId",
                schema: "CMS",
                table: "ArticleImages",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleImages_CreatedBy",
                schema: "CMS",
                table: "ArticleImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleImages_UpdatedBy",
                schema: "CMS",
                table: "ArticleImages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedBy",
                schema: "CMS",
                table: "Articles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UpdatedBy",
                schema: "CMS",
                table: "Articles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelVideos_CreatedBy",
                schema: "CMS",
                table: "ChannelVideos",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelVideos_UpdatedBy",
                schema: "CMS",
                table: "ChannelVideos",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfos_CreatedBy",
                schema: "CMS",
                table: "ContactInfos",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfos_UpdatedBy",
                schema: "CMS",
                table: "ContactInfos",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Features_CreatedBy",
                schema: "CMS",
                table: "Features",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Features_UpdatedBy",
                schema: "CMS",
                table: "Features",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FrequentlyAskedQuestions_CreatedBy",
                schema: "CMS",
                table: "FrequentlyAskedQuestions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FrequentlyAskedQuestions_UpdatedBy",
                schema: "CMS",
                table: "FrequentlyAskedQuestions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryQuestionReplies_CreatedBy",
                schema: "CMS",
                table: "InquiryQuestionReplies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryQuestionReplies_InquiryQuestionId",
                schema: "CMS",
                table: "InquiryQuestionReplies",
                column: "InquiryQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryQuestionReplies_UpdatedBy",
                schema: "CMS",
                table: "InquiryQuestionReplies",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryQuestions_CreatedBy",
                schema: "CMS",
                table: "InquiryQuestions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryQuestions_UpdatedBy",
                schema: "CMS",
                table: "InquiryQuestions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UsefulResources_CreatedBy",
                schema: "CMS",
                table: "UsefulResources",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UsefulResources_UpdatedBy",
                schema: "CMS",
                table: "UsefulResources",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationAudits_ConfigurationId",
                schema: "Configuration",
                table: "ConfigurationAudits",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationAudits_CreatedBy",
                schema: "Configuration",
                table: "ConfigurationAudits",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CountryDiseaseIncidents_CountryId",
                schema: "Disease",
                table: "CountryDiseaseIncidents",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryDiseaseIncidents_CreatedBy",
                schema: "Disease",
                table: "CountryDiseaseIncidents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CountryDiseaseIncidents_DiseaseId",
                schema: "Disease",
                table: "CountryDiseaseIncidents",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryDiseaseIncidents_UpdatedBy",
                schema: "Disease",
                table: "CountryDiseaseIncidents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Diseases_CreatedBy",
                schema: "Disease",
                table: "Diseases",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Diseases_UpdatedBy",
                schema: "Disease",
                table: "Diseases",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseTestingProtocols_CreatedBy",
                schema: "Disease",
                table: "DiseaseTestingProtocols",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseTestingProtocols_DiseaseId",
                schema: "Disease",
                table: "DiseaseTestingProtocols",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseTestingProtocols_TestingProtocolId",
                schema: "Disease",
                table: "DiseaseTestingProtocols",
                column: "TestingProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseTestingProtocols_UpdatedBy",
                schema: "Disease",
                table: "DiseaseTestingProtocols",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssumptionParameters_CreatedBy",
                schema: "DiseaseProgram",
                table: "PatientAssumptionParameters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssumptionParameters_EntityTypeId",
                schema: "DiseaseProgram",
                table: "PatientAssumptionParameters",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssumptionParameters_ProgramId",
                schema: "DiseaseProgram",
                table: "PatientAssumptionParameters",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssumptionParameters_UpdatedBy",
                schema: "DiseaseProgram",
                table: "PatientAssumptionParameters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssumptionParameters_VariableTypeId",
                schema: "DiseaseProgram",
                table: "PatientAssumptionParameters",
                column: "VariableTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAssumptionParameters_CreatedBy",
                schema: "DiseaseProgram",
                table: "ProductAssumptionParameters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAssumptionParameters_ProgramId",
                schema: "DiseaseProgram",
                table: "ProductAssumptionParameters",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAssumptionParameters_UpdatedBy",
                schema: "DiseaseProgram",
                table: "ProductAssumptionParameters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_CreatedBy",
                schema: "DiseaseProgram",
                table: "Programs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DiseaseId",
                schema: "DiseaseProgram",
                table: "Programs",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_UpdatedBy",
                schema: "DiseaseProgram",
                table: "Programs",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTests_CreatedBy",
                schema: "DiseaseProgram",
                table: "ProgramTests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTests_ProgramId",
                schema: "DiseaseProgram",
                table: "ProgramTests",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTests_TestId",
                schema: "DiseaseProgram",
                table: "ProgramTests",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTests_TestingProtocolId",
                schema: "DiseaseProgram",
                table: "ProgramTests",
                column: "TestingProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTests_UpdatedBy",
                schema: "DiseaseProgram",
                table: "ProgramTests",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingAssumptionParameters_CreatedBy",
                schema: "DiseaseProgram",
                table: "TestingAssumptionParameters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingAssumptionParameters_ProgramId",
                schema: "DiseaseProgram",
                table: "TestingAssumptionParameters",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingAssumptionParameters_UpdatedBy",
                schema: "DiseaseProgram",
                table: "TestingAssumptionParameters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastCategories_CreatedBy",
                schema: "Forecasting",
                table: "ForecastCategories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastCategories_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastCategories",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastCategories_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastCategories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_CountryId",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_CreatedBy",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_ForecastInfoLevelId",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "ForecastInfoLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_ForecastMethodologyId",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "ForecastMethodologyId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_LaboratoryId",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_ScopeOfTheForecastId",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "ScopeOfTheForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInfos_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastInfos",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInstruments_CreatedBy",
                schema: "Forecasting",
                table: "ForecastInstruments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInstruments_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastInstruments",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInstruments_InstrumentId",
                schema: "Forecasting",
                table: "ForecastInstruments",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastInstruments_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastInstruments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratories_CreatedBy",
                schema: "Forecasting",
                table: "ForecastLaboratories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratories_ForecastCategoryId",
                schema: "Forecasting",
                table: "ForecastLaboratories",
                column: "ForecastCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratories_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastLaboratories",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratories_LaboratoryId",
                schema: "Forecasting",
                table: "ForecastLaboratories",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratories_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastLaboratories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryConsumptions_CreatedBy",
                schema: "Forecasting",
                table: "ForecastLaboratoryConsumptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryConsumptions_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastLaboratoryConsumptions",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryConsumptions_LaboratoryId",
                schema: "Forecasting",
                table: "ForecastLaboratoryConsumptions",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryConsumptions_ProductId",
                schema: "Forecasting",
                table: "ForecastLaboratoryConsumptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryConsumptions_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastLaboratoryConsumptions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryTestServices_CreatedBy",
                schema: "Forecasting",
                table: "ForecastLaboratoryTestServices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryTestServices_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastLaboratoryTestServices",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryTestServices_LaboratoryId",
                schema: "Forecasting",
                table: "ForecastLaboratoryTestServices",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryTestServices_TestId",
                schema: "Forecasting",
                table: "ForecastLaboratoryTestServices",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastLaboratoryTestServices_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastLaboratoryTestServices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityPrograms_CreatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityPrograms",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityPrograms_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastMorbidityPrograms",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityPrograms_ProgramId",
                schema: "Forecasting",
                table: "ForecastMorbidityPrograms",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityPrograms_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityPrograms",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTargetBases_CreatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityTargetBases",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTargetBases_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastMorbidityTargetBases",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTargetBases_ForecastLaboratoryId",
                schema: "Forecasting",
                table: "ForecastMorbidityTargetBases",
                column: "ForecastLaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTargetBases_ForecastMorbidityProgramId",
                schema: "Forecasting",
                table: "ForecastMorbidityTargetBases",
                column: "ForecastMorbidityProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTargetBases_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityTargetBases",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_CalculationPeriodMonthId",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "CalculationPeriodMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_CreatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_PatientGroupId",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "PatientGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_ProgramId",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_TestId",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_TestingProtocolId",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "TestingProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityTestingProtocolMonths_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityTestingProtocolMonths",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityWhoBases_CountryId",
                schema: "Forecasting",
                table: "ForecastMorbidityWhoBases",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityWhoBases_CreatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityWhoBases",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityWhoBases_DiseaseId",
                schema: "Forecasting",
                table: "ForecastMorbidityWhoBases",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityWhoBases_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastMorbidityWhoBases",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastMorbidityWhoBases_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastMorbidityWhoBases",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientAssumptionValues_CreatedBy",
                schema: "Forecasting",
                table: "ForecastPatientAssumptionValues",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientAssumptionValues_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastPatientAssumptionValues",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientAssumptionValues_PatientAssumptionParameterId",
                schema: "Forecasting",
                table: "ForecastPatientAssumptionValues",
                column: "PatientAssumptionParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientAssumptionValues_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastPatientAssumptionValues",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientGroups_CreatedBy",
                schema: "Forecasting",
                table: "ForecastPatientGroups",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientGroups_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastPatientGroups",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientGroups_PatientGroupId",
                schema: "Forecasting",
                table: "ForecastPatientGroups",
                column: "PatientGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientGroups_ProgramId",
                schema: "Forecasting",
                table: "ForecastPatientGroups",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastPatientGroups_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastPatientGroups",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastProductAssumptionValues_CreatedBy",
                schema: "Forecasting",
                table: "ForecastProductAssumptionValues",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastProductAssumptionValues_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastProductAssumptionValues",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastProductAssumptionValues_ProductAssumptionParameterId",
                schema: "Forecasting",
                table: "ForecastProductAssumptionValues",
                column: "ProductAssumptionParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastProductAssumptionValues_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastProductAssumptionValues",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_CreatedBy",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_LaboratoryId",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_ProductId",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_ProductTypeId",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_TestId",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastResults_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastResults",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTestingAssumptionValues_CreatedBy",
                schema: "Forecasting",
                table: "ForecastTestingAssumptionValues",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTestingAssumptionValues_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastTestingAssumptionValues",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTestingAssumptionValues_TestingAssumptionParameterId",
                schema: "Forecasting",
                table: "ForecastTestingAssumptionValues",
                column: "TestingAssumptionParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTestingAssumptionValues_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastTestingAssumptionValues",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTests_CreatedBy",
                schema: "Forecasting",
                table: "ForecastTests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTests_ForecastInfoId",
                schema: "Forecasting",
                table: "ForecastTests",
                column: "ForecastInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTests_TestId",
                schema: "Forecasting",
                table: "ForecastTests",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastTests_UpdatedBy",
                schema: "Forecasting",
                table: "ForecastTests",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryConsumptions_CreatedBy",
                schema: "Laboratory",
                table: "LaboratoryConsumptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryConsumptions_LaboratoryId",
                schema: "Laboratory",
                table: "LaboratoryConsumptions",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryConsumptions_ProductId",
                schema: "Laboratory",
                table: "LaboratoryConsumptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryConsumptions_UpdatedBy",
                schema: "Laboratory",
                table: "LaboratoryConsumptions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryInstruments_CreatedBy",
                schema: "Laboratory",
                table: "LaboratoryInstruments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryInstruments_InstrumentId",
                schema: "Laboratory",
                table: "LaboratoryInstruments",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryInstruments_LaboratoryId",
                schema: "Laboratory",
                table: "LaboratoryInstruments",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryInstruments_UpdatedBy",
                schema: "Laboratory",
                table: "LaboratoryInstruments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryPatientStatistics_CreatedBy",
                schema: "Laboratory",
                table: "LaboratoryPatientStatistics",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryPatientStatistics_LaboratoryId",
                schema: "Laboratory",
                table: "LaboratoryPatientStatistics",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryPatientStatistics_UpdatedBy",
                schema: "Laboratory",
                table: "LaboratoryPatientStatistics",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestServices_CreatedBy",
                schema: "Laboratory",
                table: "LaboratoryTestServices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestServices_LaboratoryId",
                schema: "Laboratory",
                table: "LaboratoryTestServices",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestServices_TestId",
                schema: "Laboratory",
                table: "LaboratoryTestServices",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestServices_UpdatedBy",
                schema: "Laboratory",
                table: "LaboratoryTestServices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryWorkingDays_CreatedBy",
                schema: "Laboratory",
                table: "LaboratoryWorkingDays",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryWorkingDays_LaboratoryId",
                schema: "Laboratory",
                table: "LaboratoryWorkingDays",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryWorkingDays_UpdatedBy",
                schema: "Laboratory",
                table: "LaboratoryWorkingDays",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CalculationPeriodMonths_CalculationPeriodId",
                schema: "Lookup",
                table: "CalculationPeriodMonths",
                column: "CalculationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ContinentId",
                schema: "Lookup",
                table: "Countries",
                column: "ContinentId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CountryPeriodId",
                schema: "Lookup",
                table: "Countries",
                column: "CountryPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CreatedBy",
                schema: "Lookup",
                table: "Countries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_UpdatedBy",
                schema: "Lookup",
                table: "Countries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_CreatedBy",
                schema: "Lookup",
                table: "Laboratories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_LaboratoryCategoryId",
                schema: "Lookup",
                table: "Laboratories",
                column: "LaboratoryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_LaboratoryLevelId",
                schema: "Lookup",
                table: "Laboratories",
                column: "LaboratoryLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_RegionId",
                schema: "Lookup",
                table: "Laboratories",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_UpdatedBy",
                schema: "Lookup",
                table: "Laboratories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryCategories_CreatedBy",
                schema: "Lookup",
                table: "LaboratoryCategories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryCategories_UpdatedBy",
                schema: "Lookup",
                table: "LaboratoryCategories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryLevels_CreatedBy",
                schema: "Lookup",
                table: "LaboratoryLevels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryLevels_UpdatedBy",
                schema: "Lookup",
                table: "LaboratoryLevels",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PatientGroups_CreatedBy",
                schema: "Lookup",
                table: "PatientGroups",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PatientGroups_UpdatedBy",
                schema: "Lookup",
                table: "PatientGroups",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CountryId",
                schema: "Lookup",
                table: "Regions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CreatedBy",
                schema: "Lookup",
                table: "Regions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_UpdatedBy",
                schema: "Lookup",
                table: "Regions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingAreas_CreatedBy",
                schema: "Lookup",
                table: "TestingAreas",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingAreas_UpdatedBy",
                schema: "Lookup",
                table: "TestingAreas",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CountryProductPrices_CountryId",
                schema: "Product",
                table: "CountryProductPrices",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryProductPrices_CreatedBy",
                schema: "Product",
                table: "CountryProductPrices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CountryProductPrices_ProductId",
                schema: "Product",
                table: "CountryProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryProductPrices_UpdatedBy",
                schema: "Product",
                table: "CountryProductPrices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_ControlRequirementUnitId",
                schema: "Product",
                table: "Instruments",
                column: "ControlRequirementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_CreatedBy",
                schema: "Product",
                table: "Instruments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_ReagentSystemId",
                schema: "Product",
                table: "Instruments",
                column: "ReagentSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_TestingAreaId",
                schema: "Product",
                table: "Instruments",
                column: "TestingAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_ThroughPutUnitId",
                schema: "Product",
                table: "Instruments",
                column: "ThroughPutUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_UpdatedBy",
                schema: "Product",
                table: "Instruments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_VendorId",
                schema: "Product",
                table: "Instruments",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryProductPrices_CreatedBy",
                schema: "Product",
                table: "LaboratoryProductPrices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryProductPrices_LaboratoryId",
                schema: "Product",
                table: "LaboratoryProductPrices",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryProductPrices_ProductId",
                schema: "Product",
                table: "LaboratoryProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryProductPrices_UpdatedBy",
                schema: "Product",
                table: "LaboratoryProductPrices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedBy",
                schema: "Product",
                table: "Products",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductBasicUnitId",
                schema: "Product",
                table: "Products",
                column: "ProductBasicUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                schema: "Product",
                table: "Products",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedBy",
                schema: "Product",
                table: "Products",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendorId",
                schema: "Product",
                table: "Products",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUsage_CountryPeriodId",
                schema: "Product",
                table: "ProductUsage",
                column: "CountryPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUsage_CreatedBy",
                schema: "Product",
                table: "ProductUsage",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUsage_InstrumentId",
                schema: "Product",
                table: "ProductUsage",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUsage_ProductId",
                schema: "Product",
                table: "ProductUsage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUsage_TestId",
                schema: "Product",
                table: "ProductUsage",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUsage_UpdatedBy",
                schema: "Product",
                table: "ProductUsage",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RegionProductPrices_CreatedBy",
                schema: "Product",
                table: "RegionProductPrices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RegionProductPrices_ProductId",
                schema: "Product",
                table: "RegionProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionProductPrices_RegionId",
                schema: "Product",
                table: "RegionProductPrices",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionProductPrices_UpdatedBy",
                schema: "Product",
                table: "RegionProductPrices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Security",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Security",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Security",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCountrySubscriptions_ApplicationUserId",
                schema: "Security",
                table: "UserCountrySubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCountrySubscriptions_CountryId",
                schema: "Security",
                table: "UserCountrySubscriptions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCountrySubscriptions_CreatedBy",
                schema: "Security",
                table: "UserCountrySubscriptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserCountrySubscriptions_UpdatedBy",
                schema: "Security",
                table: "UserCountrySubscriptions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserLaboratorySubscriptions_ApplicationUserId",
                schema: "Security",
                table: "UserLaboratorySubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLaboratorySubscriptions_CreatedBy",
                schema: "Security",
                table: "UserLaboratorySubscriptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserLaboratorySubscriptions_LaboratoryId",
                schema: "Security",
                table: "UserLaboratorySubscriptions",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLaboratorySubscriptions_UpdatedBy",
                schema: "Security",
                table: "UserLaboratorySubscriptions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Security",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRegionSubscriptions_ApplicationUserId",
                schema: "Security",
                table: "UserRegionSubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRegionSubscriptions_CreatedBy",
                schema: "Security",
                table: "UserRegionSubscriptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRegionSubscriptions_RegionId",
                schema: "Security",
                table: "UserRegionSubscriptions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRegionSubscriptions_UpdatedBy",
                schema: "Security",
                table: "UserRegionSubscriptions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Security",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId1",
                schema: "Security",
                table: "UserRoles",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ContinentId",
                schema: "Security",
                table: "Users",
                column: "ContinentId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Security",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Security",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserSubscriptionLevelId",
                schema: "Security",
                table: "Users",
                column: "UserSubscriptionLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactionHistories_CreatedBy",
                schema: "Security",
                table: "UserTransactionHistories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactionHistories_UpdatedBy",
                schema: "Security",
                table: "UserTransactionHistories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactionHistories_UserId",
                schema: "Security",
                table: "UserTransactionHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactionHistories_UserTransactionTypeId",
                schema: "Security",
                table: "UserTransactionHistories",
                column: "UserTransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocolCalculationPeriodMonths_CalculationPeriodMonthId",
                schema: "Testing",
                table: "TestingProtocolCalculationPeriodMonths",
                column: "CalculationPeriodMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocolCalculationPeriodMonths_CreatedBy",
                schema: "Testing",
                table: "TestingProtocolCalculationPeriodMonths",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocolCalculationPeriodMonths_TestingProtocolId",
                schema: "Testing",
                table: "TestingProtocolCalculationPeriodMonths",
                column: "TestingProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocolCalculationPeriodMonths_UpdatedBy",
                schema: "Testing",
                table: "TestingProtocolCalculationPeriodMonths",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocols_CalculationPeriodId",
                schema: "Testing",
                table: "TestingProtocols",
                column: "CalculationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocols_CreatedBy",
                schema: "Testing",
                table: "TestingProtocols",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocols_PatientGroupId",
                schema: "Testing",
                table: "TestingProtocols",
                column: "PatientGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocols_TestId",
                schema: "Testing",
                table: "TestingProtocols",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingProtocols_UpdatedBy",
                schema: "Testing",
                table: "TestingProtocols",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CreatedBy",
                schema: "Testing",
                table: "Tests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_TestingAreaId",
                schema: "Testing",
                table: "Tests",
                column: "TestingAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_UpdatedBy",
                schema: "Testing",
                table: "Tests",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_CreatedBy",
                schema: "Vendor",
                table: "VendorContacts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_UpdatedBy",
                schema: "Vendor",
                table: "VendorContacts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_VendorId",
                schema: "Vendor",
                table: "VendorContacts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CreatedBy",
                schema: "Vendor",
                table: "Vendors",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UpdatedBy",
                schema: "Vendor",
                table: "Vendors",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleImages",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "ChannelVideos",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "ContactInfos",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "FrequentlyAskedQuestions",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "InquiryQuestionReplies",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "UsefulResources",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "ConfigurationAudits",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "CountryDiseaseIncidents",
                schema: "Disease");

            migrationBuilder.DropTable(
                name: "DiseaseTestingProtocols",
                schema: "Disease");

            migrationBuilder.DropTable(
                name: "ProgramTests",
                schema: "DiseaseProgram");

            migrationBuilder.DropTable(
                name: "ForecastInstruments",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastLaboratoryConsumptions",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastLaboratoryTestServices",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastMorbidityTargetBases",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastMorbidityTestingProtocolMonths",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastMorbidityWhoBases",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastPatientAssumptionValues",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastPatientGroups",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastProductAssumptionValues",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastResults",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastTestingAssumptionValues",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastTests",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "LaboratoryConsumptions",
                schema: "Laboratory");

            migrationBuilder.DropTable(
                name: "LaboratoryInstruments",
                schema: "Laboratory");

            migrationBuilder.DropTable(
                name: "LaboratoryPatientStatistics",
                schema: "Laboratory");

            migrationBuilder.DropTable(
                name: "LaboratoryTestServices",
                schema: "Laboratory");

            migrationBuilder.DropTable(
                name: "LaboratoryWorkingDays",
                schema: "Laboratory");

            migrationBuilder.DropTable(
                name: "CountryProductPrices",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "LaboratoryProductPrices",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "ProductUsage",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "RegionProductPrices",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserCountrySubscriptions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserLaboratorySubscriptions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserRegionSubscriptions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserTransactionHistories",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "TestingProtocolCalculationPeriodMonths",
                schema: "Testing");

            migrationBuilder.DropTable(
                name: "VendorContacts",
                schema: "Vendor");

            migrationBuilder.DropTable(
                name: "Articles",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "InquiryQuestions",
                schema: "CMS");

            migrationBuilder.DropTable(
                name: "Configurations",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "ForecastLaboratories",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "ForecastMorbidityPrograms",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "PatientAssumptionParameters",
                schema: "DiseaseProgram");

            migrationBuilder.DropTable(
                name: "ProductAssumptionParameters",
                schema: "DiseaseProgram");

            migrationBuilder.DropTable(
                name: "TestingAssumptionParameters",
                schema: "DiseaseProgram");

            migrationBuilder.DropTable(
                name: "Instruments",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserTransactionTypes",
                schema: "lookup");

            migrationBuilder.DropTable(
                name: "CalculationPeriodMonths",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "TestingProtocols",
                schema: "Testing");

            migrationBuilder.DropTable(
                name: "ForecastCategories",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "VariableTypes",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Programs",
                schema: "DiseaseProgram");

            migrationBuilder.DropTable(
                name: "ControlRequirementUnits",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ReagentSystems",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ThroughPutUnits",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ProductBasicUnits",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ProductTypes",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Vendors",
                schema: "Vendor");

            migrationBuilder.DropTable(
                name: "CalculationPeriods",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "PatientGroups",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Tests",
                schema: "Testing");

            migrationBuilder.DropTable(
                name: "ForecastInfos",
                schema: "Forecasting");

            migrationBuilder.DropTable(
                name: "Diseases",
                schema: "Disease");

            migrationBuilder.DropTable(
                name: "TestingAreas",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ForecastInfoLevels",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ForecastMethodologies",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Laboratories",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "ScopeOfTheForecasts",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "LaboratoryCategories",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "LaboratoryLevels",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Regions",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "CountryPeriods",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Continents",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "UserSubscriptionLevels",
                schema: "Lookup");
        }
    }
}
