using AutoMapper;
using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.CMSSchema;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.DTO.CMS.Article;
using ForLab.DTO.CMS.ArticleImage;
using ForLab.DTO.CMS.ChannelVideo;
using ForLab.DTO.CMS.ContactInfo;
using ForLab.DTO.CMS.Feature;
using ForLab.DTO.CMS.FrequentlyAskedQuestion;
using ForLab.DTO.CMS.InquiryQuestion;
using ForLab.DTO.CMS.InquiryQuestionReply;
using ForLab.DTO.CMS.UsefulResource;
using ForLab.DTO.Common;
using ForLab.DTO.Configuration;
using ForLab.DTO.Configuration.ConfigurationAudit;
using ForLab.DTO.Disease.CountryDiseaseIncident;
using ForLab.DTO.Disease.Disease;
using ForLab.DTO.Disease.DiseaseTestingProtocol;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.DTO.DiseaseProgram.Program;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using ForLab.DTO.Forecasting.ForecastCategory;
using ForLab.DTO.Forecasting.ForecastInfo;
using ForLab.DTO.Forecasting.ForecastInstrument;
using ForLab.DTO.Forecasting.ForecastLaboratory;
using ForLab.DTO.Forecasting.ForecastLaboratoryConsumption;
using ForLab.DTO.Forecasting.ForecastLaboratoryTestService;
using ForLab.DTO.Forecasting.ForecastMorbidityProgram;
using ForLab.DTO.Forecasting.ForecastMorbidityTargetBase;
using ForLab.DTO.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.DTO.Forecasting.ForecastMorbidityWhoBase;
using ForLab.DTO.Forecasting.ForecastPatientAssumptionValue;
using ForLab.DTO.Forecasting.ForecastPatientGroup;
using ForLab.DTO.Forecasting.ForecastProductAssumptionValue;
using ForLab.DTO.Forecasting.ForecastResult;
using ForLab.DTO.Forecasting.ForecastTest;
using ForLab.DTO.Forecasting.ForecastTestingAssumptionValue;
using ForLab.DTO.Laboratory.LaboratoryConsumption;
using ForLab.DTO.Laboratory.LaboratoryInstrument;
using ForLab.DTO.Laboratory.LaboratoryPatientStatistic;
using ForLab.DTO.Laboratory.LaboratoryTestService;
using ForLab.DTO.Laboratory.LaboratoryWorkingDay;
using ForLab.DTO.Lookup.Country;
using ForLab.DTO.Lookup.Laboratory;
using ForLab.DTO.Lookup.LaboratoryCategory;
using ForLab.DTO.Lookup.LaboratoryLevel;
using ForLab.DTO.Lookup.PatientGroup;
using ForLab.DTO.Lookup.ProductBasicUnit;
using ForLab.DTO.Lookup.Region;
using ForLab.DTO.Lookup.TestingArea;
using ForLab.DTO.Lookup.ThroughPutUnit;
using ForLab.DTO.Product.CountryProductPrice;
using ForLab.DTO.Product.Instrument;
using ForLab.DTO.Product.LaboratoryProductPrice;
using ForLab.DTO.Product.Product;
using ForLab.DTO.Product.ProductUsage;
using ForLab.DTO.Product.RegionProductPrice;
using ForLab.DTO.Security.ApplicationRole;
using ForLab.DTO.Security.User;
using ForLab.DTO.Security.UserSubscription;
using ForLab.DTO.Testing.Test;
using ForLab.DTO.Testing.TestingProtocol;
using ForLab.DTO.Testing.TestingProtocolCalculationPeriodMonth;
using ForLab.DTO.Vendor.Vendor;
using ForLab.DTO.Vendor.VendorContact;
using System;

namespace ForLab.Services
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            #region Configuration
            CreateMap<Data.DbModels.ConfigurationSchema.Configuration, ConfigurationDto>().ReverseMap();
            CreateMap<Data.DbModels.ConfigurationSchema.Configuration, Data.DbModels.ConfigurationSchema.ConfigurationAudit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                .ReverseMap();
            CreateMap<Data.DbModels.ConfigurationSchema.ConfigurationAudit, ConfigurationAuditDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.ConfigurationSchema.ConfigurationAudit, ExportConfigurationAuditDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            #endregion


            #region Security Schema
            // Location
            CreateMap<Location, LocationDto>().ReverseMap();
            // User
            CreateMap<ApplicationUser, UserDrp>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.UserCountrySubscriptionDtos, opt => opt.MapFrom(src => src.UserCountrySubscriptions))
                .ForMember(dest => dest.UserRegionSubscriptionDtos, opt => opt.MapFrom(src => src.UserRegionSubscriptions))
                .ForMember(dest => dest.UserLaboratorySubscriptionDtos, opt => opt.MapFrom(src => src.UserLaboratorySubscriptions))
                .ReverseMap();
            CreateMap<ApplicationUser, UserDetailsDto>()
                .ForMember(dest => dest.UserCountrySubscriptionDtos, opt => opt.MapFrom(src => src.UserCountrySubscriptions))
                .ForMember(dest => dest.UserRegionSubscriptionDtos, opt => opt.MapFrom(src => src.UserRegionSubscriptions))
                .ForMember(dest => dest.UserLaboratorySubscriptionDtos, opt => opt.MapFrom(src => src.UserLaboratorySubscriptions))
                .ReverseMap();
            CreateMap<ApplicationUser, AuthorizedUserDto>().ReverseMap();
            CreateMap<ApplicationUser, ExportUserDto>().ReverseMap();
            // Role
            CreateMap<ApplicationRole, ApplicationRoleDto>().ReverseMap();
            // UserSubscription
            CreateMap<UserCountrySubscription, UserCountrySubscriptionDto>().ReverseMap();
            CreateMap<UserRegionSubscription, UserRegionSubscriptionDto>().ReverseMap();
            CreateMap<UserLaboratorySubscription, UserLaboratorySubscriptionDto>().ReverseMap();
            #endregion
            
            
            #region CMS Schema
            // Article
            CreateMap<ArticleImage, ArticleImageDto>().ReverseMap();
            CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.ArticleImageDtos, opt => opt.MapFrom(src => src.ArticleImages))
                .ReverseMap();

            //UsefulResource
            CreateMap<UsefulResource, UsefulResourceDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();

            //FrequentlyAskedQuestion
            CreateMap<FrequentlyAskedQuestion, FrequentlyAskedQuestionDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <FrequentlyAskedQuestion, FrequentlyAskedQuestionDrp> ().ReverseMap();

            //ChannelVideo
            CreateMap<ChannelVideo, ChannelVideoDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();

            //InquiryQuestion
            CreateMap<InquiryQuestion, InquiryQuestionDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ForMember(dest => dest.InquiryQuestionReplyDtos, opt => opt.MapFrom(src => src.InquiryQuestionReplies))
              .ReverseMap();
            CreateMap <InquiryQuestion, InquiryQuestionDrp> ().ReverseMap();

            //InquiryQuestionReply
            CreateMap<InquiryQuestionReply, InquiryQuestionReplyDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <InquiryQuestionReply, InquiryQuestionReplyDrp> ().ReverseMap();

            //ContactInfo
            CreateMap<ContactInfo, ContactInfoDto>()
             .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
             .ReverseMap();

            //Feature
            CreateMap<Feature, FeatureDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            #endregion


            #region Lookup Schema
            // Country
            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Country, ExportCountryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Country, CountryDrp> ().ReverseMap();

            // Region
            CreateMap<Region, RegionDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Region, ExportRegionDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            CreateMap<Region, RegionDrp>().ReverseMap();

            // Laboratory
            CreateMap<Data.DbModels.LookupSchema.Laboratory, LaboratoryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.LookupSchema.Laboratory, ExportLaboratoryDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            CreateMap <Data.DbModels.LookupSchema.Laboratory, LaboratoryDrp> ().ReverseMap();

            // TestingArea
            CreateMap<TestingArea, TestingAreaDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<TestingArea, ExportTestingAreaDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <TestingArea, TestingAreaDrp> ().ReverseMap();

            // PatientGroup
            CreateMap<PatientGroup, PatientGroupDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<PatientGroup, ExportPatientGroupDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <PatientGroup, PatientGroupDrp > ().ReverseMap();

            // LaboratoryCategory
            CreateMap<LaboratoryCategory, LaboratoryCategoryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<LaboratoryCategory, ExportLaboratoryCategoryDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            CreateMap <LaboratoryCategory, LaboratoryCategoryDrp> ().ReverseMap();

            // LaboratoryLevel 
            CreateMap<LaboratoryLevel, LaboratoryLevelDto> ()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<LaboratoryLevel, ExportLaboratoryLevelDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            CreateMap <LaboratoryLevel, LaboratoryLevelDrp> ().ReverseMap();

            // ProductBasicUnit 
            CreateMap<ProductBasicUnit, ProductBasicUnitDto>().ReverseMap();

            // ThroughPutUnit 
            CreateMap<ThroughPutUnit, ThroughPutUnitDto>().ReverseMap();
            #endregion


            #region Vendor Schema
            // Vendor Contact
            CreateMap<Data.DbModels.VendorSchema.VendorContact, VendorContactDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.VendorSchema.VendorContact, ExportVendorContactDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <Data.DbModels.VendorSchema.VendorContact, VendorContactDrp > ().ReverseMap();

            // Vendor
            CreateMap<Data.DbModels.VendorSchema.Vendor, VendorDto>()
                .ForMember(dest => dest.VendorContactDtos, opt => opt.MapFrom(src => src.VendorContacts))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.VendorSchema.Vendor, ExportVendorDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.VendorSchema.Vendor, VendorDrp> ().ReverseMap();
            #endregion


            #region Product 
            // Product
            CreateMap<Data.DbModels.ProductSchema.Product, ProductDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.ProductSchema.Product, ProductDrp> ().ReverseMap();

            CreateMap<Data.DbModels.ProductSchema.Product, ExportProductDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();

            // Instrument
            CreateMap<Data.DbModels.ProductSchema.Instrument, InstrumentDto>()
                    .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                    .ReverseMap();
            CreateMap<Data.DbModels.ProductSchema.Instrument, ExportInstrumentDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <Instrument, InstrumentDrp> ().ReverseMap();


            // CountryProductPrice
            CreateMap<Data.DbModels.ProductSchema.CountryProductPrice, CountryProductPriceDto>()
                 .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                 .ReverseMap();

            CreateMap<CountryProductPrice, ExportCountryProductPriceDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <CountryProductPrice, CountryProductPriceDrp> ().ReverseMap();

            // LaboratoryProductPrice
            CreateMap<Data.DbModels.ProductSchema.LaboratoryProductPrice, LaboratoryProductPriceDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();

            CreateMap<LaboratoryProductPrice, ExportLaboratoryProductPriceDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <LaboratoryProductPrice, LaboratoryProductPriceDrp> ().ReverseMap();

            // RegionProductPrice
            CreateMap<Data.DbModels.ProductSchema.RegionProductPrice, RegionProductPriceDto>()
                 .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                 .ReverseMap();

            CreateMap<RegionProductPrice, ExportRegionProductPriceDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <RegionProductPrice, RegionProductPriceDrp> ().ReverseMap();


            //ProductUsage
            CreateMap<Data.DbModels.ProductSchema.ProductUsage, ProductUsageDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();

            CreateMap<ProductUsage, ExportProductUsageDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            CreateMap<ProductUsage, ExportTestUsageDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <ProductUsage, ProductUsageDrp> ().ReverseMap();

            #endregion


            #region Testing Schema
            // Test
            CreateMap<Data.DbModels.TestingSchema.Test, TestDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <Data.DbModels.TestingSchema.Test, TestDrp> ().ReverseMap();

            CreateMap<Data.DbModels.TestingSchema.Test, ExportTestDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();

            // TestingProtocol
            CreateMap<Data.DbModels.TestingSchema.TestingProtocol, TestingProtocolDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ForMember(dest => dest.TestingProtocolCalculationPeriodMonthDtos, opt => opt.MapFrom(src => src.TestingProtocolCalculationPeriodMonths))
              .ReverseMap();

            CreateMap<Data.DbModels.TestingSchema.TestingProtocol, ExportTestingProtocolDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.TestingSchema.TestingProtocol, TestingProtocolDrp> ().ReverseMap();

            // TestingProtocolPatientGroup
            CreateMap<Data.DbModels.TestingSchema.TestingProtocolCalculationPeriodMonth, TestingProtocolCalculationPeriodMonthDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            #endregion


            #region Laboratory Schema

            //LaboratoryTestService
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryTestService, LaboratoryTestServiceDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();

            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryTestService, ExportLaboratoryTestServiceDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <Data.DbModels.LaboratorySchema.LaboratoryTestService, LaboratoryTestServiceDrp> ().ReverseMap();

            //LaboratoryPatientStatistic 
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic, LaboratoryPatientStatisticDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic, ExportLaboratoryPatientStatisticDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic, LaboratoryPatientStatisticDrp> ().ReverseMap();

            //LaboratoryConsumption 
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryConsumption, LaboratoryConsumptionDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryConsumption, ExportLaboratoryConsumptionDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            CreateMap <Data.DbModels.LaboratorySchema.LaboratoryConsumption, LaboratoryConsumptionDrp> ().ReverseMap();

            //LaboratoryInstrument 
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryInstrument, LaboratoryInstrumentDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryInstrument, ExportLaboratoryInstrumentDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.LaboratorySchema.LaboratoryInstrument, LaboratoryInstrumentDrp> ().ReverseMap();

            //LaboratoryWorkingDay 
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay, LaboratoryWorkingDayDto>()
                .ForMember(dest => dest.FormatedFromTime, opt => opt.MapFrom(src => DateTime.Today.Add(src.FromTime).ToString("hh:mm tt")))
                .ForMember(dest => dest.FormatedToTime, opt => opt.MapFrom(src => DateTime.Today.Add(src.ToTime).ToString("hh:mm tt")))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryWorkingDay, ExportLaboratoryWorkingDayDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.LaboratorySchema.LaboratoryWorkingDay, LaboratoryWorkingDayDrp> ().ReverseMap();


            #endregion


            #region DiseaseProgram Schema
            // Program
            CreateMap<Data.DbModels.DiseaseProgramSchema.Program, ProgramDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ForMember(dest => dest.ProgramTestDtos, opt => opt.MapFrom(src => src.ProgramTests))
                .ForMember(dest => dest.PatientAssumptionParameterDtos, opt => opt.MapFrom(src => src.PatientAssumptionParameters))
                .ForMember(dest => dest.ProductAssumptionParameterDtos, opt => opt.MapFrom(src => src.ProductAssumptionParameters))
                .ForMember(dest => dest.TestingAssumptionParameterDtos, opt => opt.MapFrom(src => src.TestingAssumptionParameters))
                .ReverseMap();
            CreateMap<Data.DbModels.DiseaseProgramSchema.Program, ExportProgramDto>()
                   .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                   .ReverseMap();
            CreateMap <Data.DbModels.DiseaseProgramSchema.Program, ProgramDrp> ().ReverseMap();

            // PatientAssumptionParameter
            CreateMap<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter, PatientAssumptionParameterDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter, ExportPatientAssumptionParameterDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.DiseaseProgramSchema.PatientAssumptionParameter, PatientAssumptionParameterDrp> ().ReverseMap();

            // ProductAssumptionParameter
            CreateMap<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter, ProductAssumptionParameterDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter, ExportProductAssumptionParameterDto>()
                 .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                 .ReverseMap();
            CreateMap <Data.DbModels.DiseaseProgramSchema.ProductAssumptionParameter, ProductAssumptionParameterDrp> ().ReverseMap();

            // TestingAssumptionParameter
            CreateMap<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter, TestingAssumptionParameterDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter, ExportTestingAssumptionParameterDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter, TestingAssumptionParameterDrp> ().ReverseMap();

            // ProgramTest
            CreateMap<Data.DbModels.DiseaseProgramSchema.ProgramTest, ProgramTestDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ForMember(dest => dest.TestingProtocolDto, opt => opt.MapFrom(src => src.TestingProtocol))
              .ReverseMap();
            CreateMap<ProgramTestDto, Data.DbModels.DiseaseProgramSchema.ProgramTest>()
                .ForMember(dest => dest.TestingProtocol, opt => opt.MapFrom(src => src.TestingProtocolDto))
                .ReverseMap();
            CreateMap<Data.DbModels.DiseaseProgramSchema.ProgramTest, ExportProgramTestDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap <Data.DbModels.DiseaseProgramSchema.ProgramTest, ProgramTestDrp> ().ReverseMap();

            #endregion


            #region Disease Schema
            // DiseaseTestingProtocol
            CreateMap<Data.DbModels.DiseaseSchema.DiseaseTestingProtocol, DiseaseTestingProtocolDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <Data.DbModels.DiseaseSchema.DiseaseTestingProtocol, DiseaseTestingProtocolDrp> ().ReverseMap();

            // CountryDiseaseIncident
            CreateMap<Data.DbModels.DiseaseSchema.CountryDiseaseIncident, CountryDiseaseIncidentDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <Data.DbModels.DiseaseSchema.CountryDiseaseIncident, CountryDiseaseIncidentDrp> ().ReverseMap();

            // Disease
            CreateMap<Data.DbModels.DiseaseSchema.Disease, DiseaseDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
           CreateMap<Data.DbModels.DiseaseSchema.Disease, ExportDiseaseDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            CreateMap <Data.DbModels.DiseaseSchema.Disease, DiseaseDrp> ().ReverseMap();

            #endregion


            #region Forecasting
            // ForecastInfo
            CreateMap<ForecastInfo, ForecastInfoDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ForMember(dest => dest.Updator, opt => opt.MapFrom(src => src.Updator == null ? "System" : $"{src.Updator.FirstName} {src.Updator.LastName}"))
                .ForMember(dest => dest.ForecastInstrumentDtos, opt => opt.MapFrom(src => src.ForecastInstruments))
                .ForMember(dest => dest.ForecastPatientGroupDtos, opt => opt.MapFrom(src => src.ForecastPatientGroups))
                .ForMember(dest => dest.ForecastTestDtos, opt => opt.MapFrom(src => src.ForecastTests))
                .ForMember(dest => dest.ForecastCategoryDtos, opt => opt.MapFrom(src => src.ForecastCategories))
                .ForMember(dest => dest.ForecastLaboratoryConsumptionDtos, opt => opt.MapFrom(src => src.ForecastLaboratoryConsumptions))
                .ForMember(dest => dest.ForecastLaboratoryTestServiceDtos, opt => opt.MapFrom(src => src.ForecastLaboratoryTestServices))
                .ForMember(dest => dest.ForecastLaboratoryDtos, opt => opt.MapFrom(src => src.ForecastLaboratories))
                .ForMember(dest => dest.ForecastMorbidityWhoBaseDtos, opt => opt.MapFrom(src => src.ForecastMorbidityWhoBases))
                .ForMember(dest => dest.ForecastPatientAssumptionValueDtos, opt => opt.MapFrom(src => src.ForecastPatientAssumptionValues))
                .ForMember(dest => dest.ForecastProductAssumptionValueDtos, opt => opt.MapFrom(src => src.ForecastProductAssumptionValues))
                .ForMember(dest => dest.ForecastTestingAssumptionValueDtos, opt => opt.MapFrom(src => src.ForecastTestingAssumptionValues))
                .ForMember(dest => dest.ForecastMorbidityTestingProtocolMonthDtos, opt => opt.MapFrom(src => src.ForecastMorbidityTestingProtocolMonths))
                .ForMember(dest => dest.ForecastMorbidityProgramDtos, opt => opt.MapFrom(src => src.ForecastMorbidityPrograms))
                .ForMember(dest => dest.ForecastMorbidityTargetBaseDtos, opt => opt.MapFrom(src => src.ForecastMorbidityTargetBases))
                .ReverseMap();
            CreateMap <ForecastInfo, ForecastInfoDrp> ().ReverseMap();

            // ForecastInfo
            CreateMap<ForecastInfo, ExportForecastInfoDto>()
              .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
              .ReverseMap();
            // ForecastInstrument
            CreateMap<ForecastInstrument, ForecastInstrumentDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastInstrument, ExportForecastInstrumentDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastPatientGroup
            CreateMap<ForecastPatientGroup, ForecastPatientGroupDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastPatientGroup, ExportForecastPatientGroupDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastTest
            CreateMap<ForecastTest, ForecastTestDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastTest, ExportForecastTestDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            // ForecastCategory
            CreateMap<ForecastCategory, ForecastCategoryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ForMember(dest => dest.ForecastLaboratoryDtos, opt => opt.MapFrom(src => src.ForecastLaboratories))
                .ReverseMap();
            CreateMap<ForecastCategory, ExportForecastCategoryDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastLaboratoryConsumption
            CreateMap<ForecastLaboratoryConsumption, ForecastLaboratoryConsumptionDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastLaboratoryConsumption, ExportForecastLaboratoryConsumptionDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            // ForecastLaboratoryTestService
            CreateMap<ForecastLaboratoryTestService, ForecastLaboratoryTestServiceDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastLaboratoryTestService, ExportForecastLaboratoryTestServiceDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            // ForecastLaboratory
            CreateMap<ForecastLaboratory, ForecastLaboratoryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastLaboratory, ExportForecastLaboratoryDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            // ForecastMorbidityWhoBase
            CreateMap<ForecastMorbidityWhoBase, ForecastMorbidityWhoBaseDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastMorbidityWhoBase, ExportForecastMorbidityWhoBaseDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastPatientAssumptionValue
            CreateMap<ForecastPatientAssumptionValue, ForecastPatientAssumptionValueDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastPatientAssumptionValue, ExportForecastPatientAssumptionValueDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastProductAssumptionValue
            CreateMap<ForecastProductAssumptionValue, ForecastProductAssumptionValueDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastProductAssumptionValue, ExportForecastProductAssumptionValueDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastTestingAssumptionValue
            CreateMap<ForecastTestingAssumptionValue, ForecastTestingAssumptionValueDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastTestingAssumptionValue, ExportForecastTestingAssumptionValueDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastMorbidityTestingProtocolMonth
            CreateMap<ForecastMorbidityTestingProtocolMonth, ForecastMorbidityTestingProtocolMonthDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastMorbidityTestingProtocolMonth, ExportForecastMorbidityTestingProtocolMonthDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastMorbidityTestingProtocolMonth
            CreateMap<ForecastMorbidityProgram, ForecastMorbidityProgramDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastMorbidityProgram, ExportForecastMorbidityProgramDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastMorbidityTargetBase
            CreateMap<ForecastMorbidityTargetBase, ForecastMorbidityTargetBaseDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            CreateMap<ForecastMorbidityTargetBase, ExportForecastMorbidityTargetBaseDto>()
               .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
               .ReverseMap();
            // ForecastResult
            CreateMap<ForecastResult, ForecastResultDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator == null ? "System" : $"{src.Creator.FirstName} {src.Creator.LastName}"))
                .ReverseMap();
            #endregion

            #region Histoical Data
            // Get Forecast Laboratories from Excel File
            CreateMap<HistoicalServiceDataDto, ForecastLaboratoryDto>().ReverseMap();
            CreateMap<HistoicalConsumptionDto, ForecastLaboratoryDto>().ReverseMap();
            CreateMap<HistoicalTargetBaseDto, ForecastLaboratoryDto>().ReverseMap();
            CreateMap<HistoicalTargetBaseDto, ForecastMorbidityTargetBaseDto>().ReverseMap();
            CreateMap<HistoicalWhoBaseDto, ForecastMorbidityWhoBaseDto>().ReverseMap();
            // Consumption
            CreateMap<ForecastLaboratoryConsumption, HistoicalConsumptionDto>()
                .ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.Laboratory == null ? 0 : src.Laboratory.RegionId))
                .ReverseMap();
            // ServiceData
            CreateMap<ForecastLaboratoryTestService, HistoicalServiceDataDto>()
                .ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.Laboratory == null ? 0 : src.Laboratory.RegionId))
                .ReverseMap();
            // Target Base
            CreateMap<ForecastMorbidityTargetBase, HistoicalTargetBaseDto>()
                .ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.ForecastLaboratory.Laboratory == null ? 0 : src.ForecastLaboratory.Laboratory.RegionId))
                .ForMember(dest => dest.LaboratoryId, opt => opt.MapFrom(src => src.ForecastLaboratory == null ? 0 : src.ForecastLaboratory.LaboratoryId))
                .ForMember(dest => dest.ProgramId, opt => opt.MapFrom(src => src.ForecastMorbidityProgram == null ? 0 : src.ForecastMorbidityProgram.ProgramId))
                .ReverseMap();
            // WHO Base
            CreateMap<ForecastMorbidityWhoBase, HistoicalWhoBaseDto>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.DiseaseId, opt => opt.MapFrom(src => src.DiseaseId))
                .ReverseMap();
            #endregion

            #region Create Forecast & ML
            // ML Request Body
            CreateMap<HistoicalServiceDataDto, MLBodyDto>()
                .ForMember(dest => dest.product, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.region, opt => opt.MapFrom(src => src.RegionId))
                .ForMember(dest => dest.site, opt => opt.MapFrom(src => src.LaboratoryId))
                .ForMember(dest => dest.test, opt => opt.MapFrom(src => src.TestId))
                .ReverseMap();
            CreateMap<HistoicalConsumptionDto, MLBodyDto>()
                .ForMember(dest => dest.test, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.region, opt => opt.MapFrom(src => src.RegionId))
                .ForMember(dest => dest.site, opt => opt.MapFrom(src => src.LaboratoryId))
                .ForMember(dest => dest.product, opt => opt.MapFrom(src => src.ProductId))
                .ReverseMap();
            CreateMap<HistoicalWhoBaseDto, MLBodyDto>()
                .ForMember(dest => dest.site, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.test, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.product, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.country, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.disease, opt => opt.MapFrom(src => src.DiseaseId))
                .ReverseMap();
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryTestService, MLBodyHistoryDataDto>()
                .ForMember(dest => dest.history, opt => opt.MapFrom(src => src.TestPerformed))
                .ForMember(dest => dest.timeStamps, opt => opt.MapFrom(src => src.ServiceDuration.ToString("M/d/yy")))
                .ReverseMap();
            CreateMap<Data.DbModels.LaboratorySchema.LaboratoryConsumption, MLBodyHistoryDataDto>()
                .ForMember(dest => dest.history, opt => opt.MapFrom(src => src.AmountUsed))
                .ForMember(dest => dest.timeStamps, opt => opt.MapFrom(src => src.ConsumptionDuration.ToString("M/d/yy")))
                .ReverseMap();
            CreateMap<Data.DbModels.DiseaseSchema.CountryDiseaseIncident, MLBodyHistoryDataDto>()
                .ForMember(dest => dest.history, opt => opt.MapFrom(src => src.Incidence))
                .ForMember(dest => dest.timeStamps, opt => opt.MapFrom(src => src.Year.ToString()))
                .ReverseMap();
            // Get Prediction Data from ML Response
            CreateMap<MLResponsePredictionDataDto, ForecastLaboratoryConsumptionDto>()
                .ForMember(dest => dest.AmountForecasted, opt => opt.MapFrom(src => src.Forecast))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => FormatMLPeriodResponse(src.TimeStamps)))
                .ReverseMap();
            CreateMap<MLResponsePredictionDataDto, ForecastLaboratoryTestServiceDto>()
                 .ForMember(dest => dest.AmountForecasted, opt => opt.MapFrom(src => src.Forecast))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => FormatMLPeriodResponse(src.TimeStamps)))
                .ReverseMap();
            CreateMap<MLResponsePredictionDataDto, ForecastMorbidityWhoBaseDto>()
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Forecast))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => FormatMLPeriodResponse(src.TimeStamps)))
                .ReverseMap();
            // Target Base
            CreateMap<MLResponsePredictionDataDto, ForecastLaboratoryTestService>()
                .ForMember(dest => dest.AmountForecasted, opt => opt.MapFrom(src => src.Forecast))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => FormatMLPeriodResponse(src.TimeStamps)))
                .ReverseMap();
            CreateMap<ForecastLaboratoryTestService, MLBodyDto>()
                  .ForMember(dest => dest.product, opt => opt.MapFrom(src => 0))
                  .ForMember(dest => dest.site, opt => opt.MapFrom(src => src.LaboratoryId))
                  .ForMember(dest => dest.test, opt => opt.MapFrom(src => src.TestId))
                  .ReverseMap();
            #endregion
        }

        private string FormatMLPeriodResponse(string timeStamps)
        {
            DateTime dateTime;
            var isDateTime = DateTime.TryParse(timeStamps, out dateTime);
            if (!isDateTime) return timeStamps;
            DateTime oDate = Convert.ToDateTime(timeStamps);
            return oDate.ToString("MMM-yyyy");
        }
    }
}
