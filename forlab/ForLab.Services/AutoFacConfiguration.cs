using Autofac;
using ForLab.Repositories.Security.UserTransactionHistory;
using ForLab.Core.Interfaces;
using ForLab.DTO.Common;
using ForLab.DTO.Vendor.VendorContact;
using ForLab.Repositories.CMS.Article;
using ForLab.Repositories.CMS.ArticleImage;
using ForLab.Repositories.CMS.ChannelVideo;
using ForLab.Repositories.CMS.FrequentlyAskedQuestion;
using ForLab.Repositories.CMS.InquiryQuestion;
using ForLab.Repositories.CMS.InquiryQuestionReply;
using ForLab.Repositories.CMS.UsefulResource;
using ForLab.Repositories.Configuration.Configuration;
using ForLab.Repositories.Configuration.ConfigurationAudit;
using ForLab.Repositories.Disease.CountryDiseaseIncident;
using ForLab.Repositories.Disease.Disease;
using ForLab.Repositories.Disease.DiseaseTestingProtocol;
using ForLab.Repositories.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.Program;
using ForLab.Repositories.DiseaseProgram.ProgramTest;
using ForLab.Repositories.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Repositories.Forecasting.ForecastInfo;
using ForLab.Repositories.Laboratory.LaboratoryConsumption;
using ForLab.Repositories.Laboratory.LaboratoryInstrument;
using ForLab.Repositories.Laboratory.LaboratoryPatientStatistic;
using ForLab.Repositories.Laboratory.LaboratoryTestService;
using ForLab.Repositories.Laboratory.LaboratoryWorkingDay;
using ForLab.Repositories.Lookup.Country;
using ForLab.Repositories.Lookup.Laboratory;
using ForLab.Repositories.Lookup.LaboratoryCategory;
using ForLab.Repositories.Lookup.LaboratoryLevel;
using ForLab.Repositories.Lookup.PatientGroup;
using ForLab.Repositories.Lookup.Region;
using ForLab.Repositories.Lookup.TestingArea;
using ForLab.Repositories.Product.CountryProductPrice;
using ForLab.Repositories.Product.Instrument;
using ForLab.Repositories.Product.Instrumet;
using ForLab.Repositories.Product.LaboratoryProductPrice;
using ForLab.Repositories.Product.Product;
using ForLab.Repositories.Product.ProductUsage;
using ForLab.Repositories.Product.RegionProductPrice;
using ForLab.Repositories.Security.UserCountrySubscription;
using ForLab.Repositories.Security.UserLaboratorySubscription;
using ForLab.Repositories.Security.UserRegionSubscription;
using ForLab.Repositories.Security.UserRole;
using ForLab.Repositories.Testing.Test;
using ForLab.Repositories.Testing.TestingProtocol;
using ForLab.Repositories.UOW;
using ForLab.Repositories.Vendor.Vendor;
using ForLab.Repositories.Vendor.VendorContact;
using ForLab.Services.CMS.Article;
using ForLab.Services.CMS.ChannelVideo;
using ForLab.Services.CMS.FrequentlyAskedQuestion;
using ForLab.Services.CMS.InquiryQuestion;
using ForLab.Services.CMS.UsefulResource;
using ForLab.Services.Configuration;
using ForLab.Services.Disease.CountryDiseaseIncident;
using ForLab.Services.Disease.Disease;
using ForLab.Services.Disease.DiseaseTestingProtocol;
using ForLab.Services.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Services.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Services.DiseaseProgram.Program;
using ForLab.Services.DiseaseProgram.ProgramTest;
using ForLab.Services.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Services.Forecasting.ForecastInfo;
using ForLab.Services.Global.DataFilter;
using ForLab.Services.Global.FileService;
using ForLab.Services.Global.HangFire;
using ForLab.Services.Global.SendEmail;
using ForLab.Services.Global.Twilio;
using ForLab.Services.Global.UploadFiles;
using ForLab.Services.Laboratory.LaboratoryConsumption;
using ForLab.Services.Laboratory.LaboratoryInstrument;
using ForLab.Services.Laboratory.LaboratoryPatientStatistic;
using ForLab.Services.Laboratory.LaboratoryTestService;
using ForLab.Services.Laboratory.LaboratoryWorkingDay;
using ForLab.Services.Lookup.Country;
using ForLab.Services.Lookup.Laboratory;
using ForLab.Services.Lookup.LaboratoryCategory;
using ForLab.Services.Lookup.LaboratoryLevel;
using ForLab.Services.Lookup.PatientGroup;
using ForLab.Services.Lookup.Region;
using ForLab.Services.Lookup.TestingArea;
using ForLab.Services.Product.CountryProductPrice;
using ForLab.Services.Product.Instrument;
using ForLab.Services.Product.LaboratoryProductPrice;
using ForLab.Services.Product.Product;
using ForLab.Services.Product.ProductUsage;
using ForLab.Services.Product.RegionProductPrice;
using ForLab.Services.Security.Account;
using ForLab.Services.Security.User;
using ForLab.Services.Security.UserSubscription;
using ForLab.Services.Testing.Test;
using ForLab.Services.Testing.TestingProtocol;
using ForLab.Services.Vendor.Vendor;
using ForLab.Services.Vendor.VendorContact;
using ForLab.Repositories.Forecasting.ForecastCategory;
using ForLab.Repositories.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Repositories.Forecasting.ForecastInstrument;
using ForLab.Repositories.CMS.ContactInfo;
using ForLab.Services.CMS.ContactInfo;
using ForLab.Services.CMS.Feature;
using ForLab.Repositories.CMS.Feature;
using ForLab.Services.Forecasting.ForecastCategory;
using ForLab.Services.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Services.Forecasting.ForecastInstrument;
using ForLab.Repositories.Forecasting.ForecastLaboratory;
using ForLab.Services.Forecasting.ForecastLaboratory;
using ForLab.Services.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Repositories.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Services.Forecasting.ForecastLaboratoryTestService;
using ForLab.Repositories.Forecasting.ForecastLaboratoryTestService;
using ForLab.Services.Forecasting.ForecastMorbidityProgram;
using ForLab.Repositories.Forecasting.ForecastMorbidityProgram;
using ForLab.Repositories.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Services.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Services.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Repositories.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Services.Forecasting.ForecastPatientAssumptionValue;
using ForLab.Repositories.Forecasting.ForecastPatientAssumptionValue;
using ForLab.Services.Forecasting.ForecastPatientGroup;
using ForLab.Repositories.Forecasting.ForecastPatientGroup;
using ForLab.Services.Forecasting.ForecastProductAssumptionValue;
using ForLab.Repositories.Forecasting.ForecastProductAssumptionValue;
using ForLab.Services.Forecasting.ForecastTest;
using ForLab.Repositories.Forecasting.ForecastTest;
using ForLab.Services.Forecasting.ForecastTestingAssumptionValue;
using ForLab.Repositories.Forecasting.ForecastTestingAssumptionValue;
using ForLab.Services.Dashboard;
using ForLab.Repositories.Forecasting.ForecastResult;
using ForLab.Services.Forecasting.ForecastResult;
using ForLab.Services.Lookup.ProductBasicUnit;
using ForLab.Repositories.Lookup.ProductBasicUnit;
using ForLab.Services.Lookup.ThroughPutUnit;
using ForLab.Repositories.Lookup.ThroughPutUnit;
using ForLab.Services.Global.General;

namespace ForLab.Services
{
    public class AutoFacConfiguration: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register unit of work
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerDependency();
            // Register DTO
            builder.RegisterType<ResponseDTO>().As<IResponseDTO>().InstancePerLifetimeScope();
            // Register Generic Services
            builder.RegisterGeneric(typeof(FileService<>)).As(typeof(IFileService<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(DataFilterService<>)).As(typeof(IDataFilterService<>)).InstancePerDependency();
            // Register general services
            builder.RegisterType<UploadFilesService>().As<IUploadFilesService>().InstancePerDependency();
            builder.RegisterType<TwilioService>().As<ITwilioService>().InstancePerDependency();
            builder.RegisterType<HangfireService>().As<IHangfireService>().InstancePerDependency();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerDependency();
            // Dashboard
            builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerDependency();
            // General
            builder.RegisterType<GeneralService>().As<IGeneralService>().InstancePerDependency();


            #region Configuration
            // Configuration
            builder.RegisterType<ConfigurationRepository>().As<IConfigurationRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigurationAuditRepository>().As<IConfigurationAuditRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            #endregion
            
            
            #region CMS Schema

            // Article
            builder.RegisterType<ArticleRepository>().As<IArticleRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleService>().As<IArticleService>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleImageRepository>().As<IArticleImageRepository>().InstancePerLifetimeScope();

            //UsefulResource
            builder.RegisterType<UsefulResourceRepository>().As<IUsefulResourceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UsefulResourceService>().As<IUsefulResourceService>().InstancePerLifetimeScope();

            //FrequentlyAskedQuestion
            builder.RegisterType<FrequentlyAskedQuestionRepository>().As<IFrequentlyAskedQuestionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<FrequentlyAskedQuestionService>().As<IFrequentlyAskedQuestionService>().InstancePerLifetimeScope();

            //ChannelVideo
            builder.RegisterType<ChannelVideoRepository>().As<IChannelVideoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ChannelVideoService>().As<IChannelVideoService>().InstancePerLifetimeScope();

            //InquiryQuestion
            builder.RegisterType<InquiryQuestionRepository>().As<IInquiryQuestionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryQuestionService>().As<IInquiryQuestionService>().InstancePerLifetimeScope();

            //InquiryQuestionReply
            builder.RegisterType<InquiryQuestionReplyRepository>().As<IInquiryQuestionReplyRepository>().InstancePerLifetimeScope();

            //ContactInfo
            builder.RegisterType<ContactInfoRepository>().As<IContactInfoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ContactInfoService>().As<IContactInfoService>().InstancePerLifetimeScope();

            // Feature
            builder.RegisterType<FeatureRepository>().As<IFeatureRepository>().InstancePerLifetimeScope();
            builder.RegisterType<FeatureService>().As<IFeatureService>().InstancePerLifetimeScope();
            #endregion


            #region Security Schema
            // Account
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            // User
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            // User Roles
            builder.RegisterType<UserRoleRepository>().As<IUserRoleRepository>().InstancePerLifetimeScope();
            // History
            builder.RegisterType<UserTransactionHistoryRepository>().As<IUserTransactionHistoryRepository>().InstancePerLifetimeScope();
            // UserSubscription
            builder.RegisterType<UserCountrySubscriptionRepository>().As<IUserCountrySubscriptionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRegionSubscriptionRepository>().As<IUserRegionSubscriptionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserLaboratorySubscriptionRepository>().As<IUserLaboratorySubscriptionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserSubscriptionService>().As<IUserSubscriptionService>().InstancePerLifetimeScope();
            #endregion


            #region Lookup Schema

            // Country
            builder.RegisterType<CountryRepository>().As<ICountryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();

            // Region
            builder.RegisterType<RegionRepository>().As<IRegionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RegionService>().As<IRegionService>().InstancePerLifetimeScope();

            // Laboratory
            builder.RegisterType<LaboratoryRepository>().As<ILaboratoryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryService>().As<ILaboratoryService>().InstancePerLifetimeScope();
            // TestingArea
            builder.RegisterType<TestingAreaRepository>().As<ITestingAreaRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TestingAreaService>().As<ITestingAreaService>().InstancePerLifetimeScope();
            // PatientGroup
            builder.RegisterType<PatientGroupRepository>().As<IPatientGroupRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PatientGroupService>().As<IPatientGroupService>().InstancePerLifetimeScope();
            // LaboratoryCategory
            builder.RegisterType<LaboratoryCategoryRepository>().As<ILaboratoryCategoryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryCategoryService>().As<ILaboratoryCategoryService>().InstancePerLifetimeScope();
            // LaboratoryLevel
            builder.RegisterType<LaboratoryLevelRepository>().As<ILaboratoryLevelRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryLevelService>().As<ILaboratoryLevelService>().InstancePerLifetimeScope();
            // ProductBasicUnit
            builder.RegisterType<ProductBasicUnitRepository>().As<IProductBasicUnitRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProductBasicUnitService>().As<IProductBasicUnitService>().InstancePerLifetimeScope();
            // ThroughPutUnit
            builder.RegisterType<ThroughPutUnitRepository>().As<IThroughPutUnitRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ThroughPutUnitService>().As<IThroughPutUnitService>().InstancePerLifetimeScope();

            #endregion


            #region Vendor Schema

            // Vendor
            builder.RegisterType<VendorRepository>().As<IVendorRepository>().InstancePerLifetimeScope();
            builder.RegisterType<VendorService>().As<IVendorService>().InstancePerLifetimeScope();

            // Vendor Contact
            builder.RegisterType<VendorContactRepository>().As<IVendorContactRepository>().InstancePerLifetimeScope();
            builder.RegisterType<VendorContactService>().As<IVendorContactService>().InstancePerLifetimeScope();

            #endregion


            #region Product Schema
            // Instrument
            builder.RegisterType<InstrumentRepository>().As<IInstrumentRepository>().InstancePerLifetimeScope();
            builder.RegisterType<InstrumentService>().As<IInstrumentService>().InstancePerLifetimeScope();

            // Product
            builder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();

            // CountryProductPrice
            builder.RegisterType<CountryProductPriceRepository>().As<ICountryProductPriceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CountryProductPriceService>().As<ICountryProductPriceService>().InstancePerLifetimeScope();

            // LaboratoryProductPrice
            builder.RegisterType<LaboratoryProductPriceRepository>().As<ILaboratoryProductPriceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryProductPriceService>().As<ILaboratoryProductPriceService>().InstancePerLifetimeScope();

            // RegionProductPrice
            builder.RegisterType<RegionProductPriceRepository>().As<IRegionProductPriceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RegionProductPriceService>().As<IRegionProductPriceService>().InstancePerLifetimeScope();

            // ProductUsage
            builder.RegisterType<ProductUsageRepository>().As<IProductUsageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProductUsageService>().As<IProductUsageService>().InstancePerLifetimeScope();
            #endregion


            #region Testing Schema
            // Test
            builder.RegisterType<TestRepository>().As<ITestRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TestService>().As<ITestService>().InstancePerLifetimeScope();

            // TestingProtocol
            builder.RegisterType<TestingProtocolRepository>().As<ITestingProtocolRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TestingProtocolService>().As<ITestingProtocolService>().InstancePerLifetimeScope();
            #endregion


            #region Laboratory Schema

            //LaboratoryTestService
            builder.RegisterType<LaboratoryTestServiceRepository>().As<ILaboratoryTestServiceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryTestService>().As<ILaboratoryTestService>().InstancePerLifetimeScope();
            //LaboratoryPatientStatistic
            builder.RegisterType<LaboratoryPatientStatisticRepository>().As<ILaboratoryPatientStatisticRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryPatientStatisticService>().As<ILaboratoryPatientStatisticService>().InstancePerLifetimeScope();
            //LaboratoryConsumption
            builder.RegisterType<LaboratoryConsumptionRepository>().As<ILaboratoryConsumptionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryConsumptionService>().As<ILaboratoryConsumptionService>().InstancePerLifetimeScope();
            //LaboratoryInstrument
            builder.RegisterType<LaboratoryInstrumentRepository>().As<ILaboratoryInstrumentRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryInstrumentService>().As<ILaboratoryInstrumentService>().InstancePerLifetimeScope();
            //LaboratoryWorkingDay
            builder.RegisterType<LaboratoryWorkingDayeRepository>().As<ILaboratoryWorkingDayRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LaboratoryWorkingDayService>().As<ILaboratoryWorkingDayService>().InstancePerLifetimeScope();
            #endregion


            #region DiseaseProgram Schema
            // PatientAssumptionParameter
            builder.RegisterType<PatientAssumptionParameterRepository>().As<IPatientAssumptionParameterRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PatientAssumptionParameterService>().As<IPatientAssumptionParameterService>().InstancePerLifetimeScope();


            // ProductAssumptionParameter
            builder.RegisterType<ProductAssumptionParameterRepository>().As<IProductAssumptionParameterRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAssumptionParameterService>().As<IProductAssumptionParameterService>().InstancePerLifetimeScope();


            // Program
            builder.RegisterType<ProgramRepository>().As<IProgramRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProgramService>().As<IProgramService>().InstancePerLifetimeScope();

            // TestingAssumptionParameter
            builder.RegisterType<TestingAssumptionParameterRepository>().As<ITestingAssumptionParameterRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TestingAssumptionParameterService>().As<ITestingAssumptionParameterService>().InstancePerLifetimeScope();

            // ProgramTest
            builder.RegisterType<ProgramTestRepository>().As<IProgramTestRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProgramTestService>().As<IProgramTestService>().InstancePerLifetimeScope();
            #endregion


            #region Disease  Schema
            // DiseaseTestingProtocol
            builder.RegisterType<DiseaseTestingProtocolRepository>().As<IDiseaseTestingProtocolRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DiseaseTestingProtocolService>().As<IDiseaseTestingProtocolService>().InstancePerLifetimeScope();

            // CountryDiseaseIncident
            builder.RegisterType<CountryDiseaseIncidentRepository>().As<ICountryDiseaseIncidentRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CountryDiseaseIncidentService>().As<ICountryDiseaseIncidentService>().InstancePerLifetimeScope();

            // Disease
            builder.RegisterType<DiseaseRepository>().As<IDiseaseRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DiseaseService>().As<IDiseaseService>().InstancePerLifetimeScope();
            #endregion


            #region Forecasting
            // ForecastInfo
            builder.RegisterType<ForecastInfoRepository>().As<IForecastInfoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastInfoService>().As<IForecastInfoService>().InstancePerLifetimeScope();
            // ForecastCategory
            builder.RegisterType<ForecastCategoryRepository>().As<IForecastCategoryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastCategoryService>().As<IForecastCategoryService>().InstancePerLifetimeScope();
            // ForecastMorbidityTargetBase
            builder.RegisterType<ForecastMorbidityTargetBaseRepository>().As<IForecastMorbidityTargetBaseRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastMorbidityTargetBaseService>().As<IForecastMorbidityTargetBaseService>().InstancePerLifetimeScope();
            // ForecastInstrument
            builder.RegisterType<ForecastInstrumentRepository>().As<IForecastInstrumentRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastInstrumentService>().As<IForecastInstrumentService>().InstancePerLifetimeScope();
            // ForecastLaboratory
            builder.RegisterType<ForecastLaboratoryRepository>().As<IForecastLaboratoryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastLaboratoryService>().As<IForecastLaboratoryService>().InstancePerLifetimeScope();
            // ForecastLaboratoryConsumption
            builder.RegisterType<ForecastLaboratoryConsumptionRepository>().As<IForecastLaboratoryConsumptionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastLaboratoryConsumptionService>().As<IForecastLaboratoryConsumptionService>().InstancePerLifetimeScope();
            // ForecastLaboratoryTestService
            builder.RegisterType<ForecastLaboratoryTestServiceRepository>().As<IForecastLaboratoryTestServiceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastLaboratoryTestServiceService>().As<IForecastLaboratoryTestServiceService>().InstancePerLifetimeScope();
            // ForecastMorbidityProgram
            builder.RegisterType<ForecastMorbidityProgramRepository>().As<IForecastMorbidityProgramRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastMorbidityProgramService>().As<IForecastMorbidityProgramService>().InstancePerLifetimeScope();
            // ForecastMorbidityTestingProtocolMonth
            builder.RegisterType<ForecastMorbidityTestingProtocolMonthRepository>().As<IForecastMorbidityTestingProtocolMonthRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastMorbidityTestingProtocolMonthService>().As<IForecastMorbidityTestingProtocolMonthService>().InstancePerLifetimeScope();
            // ForecastMorbidityWhoBase
            builder.RegisterType<ForecastMorbidityWhoBaseRepository>().As<IForecastMorbidityWhoBaseRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastMorbidityWhoBaseService>().As<IForecastMorbidityWhoBaseService>().InstancePerLifetimeScope();
            // ForecastPatientAssumptionValue
            builder.RegisterType<ForecastPatientAssumptionValueRepository>().As<IForecastPatientAssumptionValueRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastPatientAssumptionValueService>().As<IForecastPatientAssumptionValueService>().InstancePerLifetimeScope();
            // ForecastPatientGroup
            builder.RegisterType<ForecastPatientGroupRepository>().As<IForecastPatientGroupRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastPatientGroupService>().As<IForecastPatientGroupService>().InstancePerLifetimeScope();
            // ForecastProductAssumptionValue
            builder.RegisterType<ForecastProductAssumptionValueRepository>().As<IForecastProductAssumptionValueRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastProductAssumptionValueService>().As<IForecastProductAssumptionValueService>().InstancePerLifetimeScope();
            // ForecastTest
            builder.RegisterType<ForecastTestRepository>().As<IForecastTestRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastTestService>().As<IForecastTestService>().InstancePerLifetimeScope();
            // ForecastTestingAssumptionValue
            builder.RegisterType<ForecastTestingAssumptionValueRepository>().As<IForecastTestingAssumptionValueRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastTestingAssumptionValueService>().As<IForecastTestingAssumptionValueService>().InstancePerLifetimeScope();
            // ForecastResult
            builder.RegisterType<ForecastResultRepository>().As<IForecastResultRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ForecastResultService>().As<IForecastResultService>().InstancePerLifetimeScope();
            #endregion
        }
    }
}
