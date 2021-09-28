using ForLab.Data.DbModels.CMSSchema;
using ForLab.Data.DbModels.ConfigurationSchema;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using ForLab.Data.DbModels.DiseaseSchema;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LaboratorySchema;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.Data.DbModels.TestingSchema;
using ForLab.Data.DbModels.VendorSchema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ForLab.Data.DataContext
{
    public class AppDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, int, 
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, 
        ApplicationRoleClaim, ApplicationUserToken>
    {
        public AppDbContext()
        {
            Database.SetCommandTimeout(150000);
        }
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set application user relations
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                // user country subscribtions
                b.HasMany(e => e.UserCountrySubscriptions)
                    .WithOne(e => e.ApplicationUser)
                    .HasForeignKey(ur => ur.ApplicationUserId);

                // user region subscribtion
                b.HasMany(e => e.UserRegionSubscriptions)
                   .WithOne(e => e.ApplicationUser)
                   .HasForeignKey(ur => ur.ApplicationUserId);

                // user laboratory subscribtion
                b.HasMany(e => e.UserLaboratorySubscriptions)
                   .WithOne(e => e.ApplicationUser)
                   .HasForeignKey(ur => ur.ApplicationUserId);
            });

            // set application role relations
            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // set application role primary key
                b.HasKey(u => u.Id);
                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            // set application user role primary key
            modelBuilder.Entity<ApplicationUserRole>(b =>
            {
                b.HasKey(u => u.Id);
            });

            // Update Identity Schema
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "Security");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles", "Security");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles", "Security");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins", "Security");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims", "Security");
            modelBuilder.Entity<ApplicationUserToken>().ToTable("UserTokens", "Security");
            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims", "Security");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property in modelBuilder.Model.GetEntityTypes()
                           .SelectMany(t => t.GetProperties())
                           .Where(p => p.ClrType == typeof(decimal)))
            {
                property.SetDefaultValue(0);
                property.SetColumnType("decimal(18, 4)");
            }

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                                    .SelectMany(t => t.GetProperties())
                                    .Where(p => p.ClrType == typeof(bool)))
            {
                property.SetDefaultValue(false);
            }

            // Make Amount accept up to 8 digits after decimal point
            modelBuilder.Entity<ProductUsage>()
              .Property(p => p.Amount)
              .HasColumnType("decimal(20,8)");
        }

        #region Lookup Schema
        public DbSet<UserSubscriptionLevel> UserSubscriptionLevels { get; set; }
        public DbSet<Continent> Continents { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<LaboratoryCategory> LaboratoryCategories { get; set; }
        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<TestingArea> TestingAreas { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<PatientGroup> PatientGroups { get; set; }
        public DbSet<LaboratoryLevel> LaboratoryLevels { get; set; }
        public DbSet<ThroughPutUnit> ThroughPutUnits { get; set; }
        public DbSet<ReagentSystem> ReagentSystems { get; set; }
        public DbSet<ControlRequirementUnit> ControlRequirementUnits { get; set; }
        public DbSet<ProductBasicUnit> ProductBasicUnits { get; set; }
        public DbSet<ForecastMethodology> ForecastMethodologies { get; set; }
        public DbSet<CountryPeriod> CountryPeriods { get; set; }
        public DbSet<ScopeOfTheForecast> ScopeOfTheForecasts { get; set; }
        public DbSet<VariableType> VariableTypes { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<ForecastInfoLevel> ForecastInfoLevels { get; set; }
        public DbSet<UserTransactionType> UserTransactionTypes { get; set; }
        public DbSet<CalculationPeriod> CalculationPeriods { get; set; }
        public DbSet<CalculationPeriodMonth> CalculationPeriodMonths { get; set; }

        #endregion

        #region Security Schema
        public DbSet<UserTransactionHistory> UserTransactionHistories { get; set; }
        public DbSet<UserCountrySubscription> UserCountrySubscriptions { get; set; }
        public DbSet<UserRegionSubscription> UserRegionSubscriptions { get; set; }
        public DbSet<UserLaboratorySubscription> UserLaboratorySubscriptions { get; set; }
        #endregion

        #region Disease
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<CountryDiseaseIncident> CountryDiseaseIncidents { get; set; }
        public DbSet<DiseaseTestingProtocol> DiseaseTestingProtocols { get; set; }
        #endregion

        #region Product Schema
        public DbSet<Product> Products { get; set; }
        public DbSet<CountryProductPrice> CountryProductPrices { get; set; }
        public DbSet<RegionProductPrice> RegionProductPrices { get; set; }
        public DbSet<LaboratoryProductPrice> LaboratoryProductPrices { get; set; }
        public DbSet<ProductUsage> ProductUsages { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        #endregion

        #region Testing Schema
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestingProtocol> TestingProtocols { get; set; }
        public DbSet<TestingProtocolCalculationPeriodMonth> TestingProtocolCalculationPeriodMonths { get; set; }
        #endregion

        #region Laboratory Schema
        public DbSet<LaboratoryWorkingDay> LaboratoryWorkingDays { get; set; }
        public DbSet<LaboratoryInstrument> LaboratoryInstruments { get; set; }
        public DbSet<LaboratoryConsumption> LaboratoryConsumptions { get; set; }
        public DbSet<LaboratoryTestService> LaboratoryTestServices { get; set; }
        public DbSet<LaboratoryPatientStatistic> LaboratoryPatientStatistics { get; set; }
        #endregion

        #region Forecasting Schema
        public DbSet<ForecastInfo> ForecastInfos { get; set; }
        public DbSet<ForecastPatientGroup> ForecastPatientGroups { get; set; }
        public DbSet<ForecastInstrument> ForecastInstruments { get; set; }
        public DbSet<ForecastTest> ForecastTests { get; set; }
        public DbSet<ForecastCategory> ForecastCategories { get; set; }
        public DbSet<ForecastLaboratory> ForecastLaboratories { get; set; }
        public DbSet<ForecastLaboratoryTestService> ForecastLaboratoryTestServices { get; set; }
        public DbSet<ForecastLaboratoryConsumption> ForecastLaboratoryConsumptions { get; set; }
        public DbSet<ForecastMorbidityTargetBase> ForecastMorbidityTargetBases { get; set; }
        public DbSet<ForecastMorbidityWhoBase> ForecastMorbidityWhoBases { get; set; }
        public DbSet<ForecastPatientAssumptionValue> ForecastPatientAssumptionValues { get; set; } 
        public DbSet<ForecastProductAssumptionValue> ForecastProductAssumptionValues { get; set; }
        public DbSet<ForecastTestingAssumptionValue> ForecastTestingAssumptionValues { get; set; }
        public DbSet<ForecastMorbidityProgram> ForecastMorbidityPrograms { get; set; }
        public DbSet<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
        public DbSet<ForecastResult> ForecastResults { get; set; }
        #endregion

        #region Vendor Schema
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorContact> VendorContacts { get; set; }
        #endregion

        #region Program
        public DbSet<Program> Programs { get; set; }
        public DbSet<ProgramTest> ProgramTests { get; set; }
        public DbSet<PatientAssumptionParameter> PatientAssumptionParameters { get; set; }
        public DbSet<ProductAssumptionParameter> ProductAssumptionParameters { get; set; }
        public DbSet<TestingAssumptionParameter> TestingAssumptionParameters { get; set; }
        #endregion

        #region Configuration
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<ConfigurationAudit> ConfigurationAudits { get; set; }
        #endregion

        #region CMS Schema
        public DbSet<UsefulResource> UsefulResources { get; set; }
        public DbSet<InquiryQuestion> InquiryQuestions { get; set; }
        public DbSet<InquiryQuestionReply> InquiryQuestionReplies { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }
        public DbSet<FrequentlyAskedQuestion> FrequentlyAskedQuestions { get; set; }
        public DbSet<ChannelVideo> ChannelVideos { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        #endregion
    }
}
