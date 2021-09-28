using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.SecuritySchema;
using ForLab.Data.Enums;
using ForLab.Data.ThirdPartyInfo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ForLab.Data.DataContext
{
    public class DataSeedingIntilization
    {
        private static AppDbContext _appDbContext;
        private static UserManager<ApplicationUser> _userManager;
        private static IServiceProvider _serviceProvider;

        public static void Seed(AppDbContext appDbContext, IServiceProvider serviceProvider)
        {
            _appDbContext = appDbContext;
            _appDbContext.Database.EnsureCreated();
            //_appDbContext.Database.Migrate();
            _serviceProvider = serviceProvider;

            var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            // call functions
            SeedContactInfo();
            SeedFeatures();
            SeedConfiguration();
            SeedApplicationRoles();
            SeedUserSubscriptionLevels();
            SeedApplicationSuperAdmin();
            SeedUserTransactionTypes();

            #region Static Lookups
            SeedCalculationPeriods();
            SeedContinents();
            SeedControlRequirementUnits();
            SeedCountryPeriods();
            SeedEntityTypes();
            SeedForecastInfoLevels();
            SeedForecastMethodologies();
            SeedProductBasicUnits();
            SeedProductTypes();
            SeedReagentSystems();
            SeedScopeOfTheForecasts();
            SeedThroughPutUnits();
            SeedVariableTypes();
            #endregion

            #region Country Info
            SeedCountries();
            #endregion

            // save to the database
            _appDbContext.SaveChanges();

            // Seed Admin Subscriptions
            SeedSuperAdminSubscriptions();
        }

        private static void SeedConfiguration()
        {
            var config = _appDbContext.Configurations.FirstOrDefault();
            if (config == null)
            {
                var defaultConfig = new DbModels.ConfigurationSchema.Configuration()
                {
                    AccountLoginAttempts = 5,
                    NumOfDaysToChangePassword = 30,
                    PasswordExpiryTime = 24 * 60,
                    TimeToSessionTimeOut = 60 * 60,
                    UserPhotosize = 80000,
                    AttachmentsMaxSize = 80000,
                };

                _appDbContext.Configurations.Add(defaultConfig);
            }
        }
        private static void SeedContactInfo()
        {
            var contactInfo = _appDbContext.ContactInfos.FirstOrDefault();
            if (contactInfo == null)
            {
                var defaultContactInfo = new DbModels.CMSSchema.ContactInfo()
                {
                    Email = "Zelalem@Opianhealth.com",
                    Phone= "+251911430408",
                    Address = "Cape Verdi Street, Across Rakan Mall, KT Building, 10th Floor",
                    CreatedBy = null,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };

                _appDbContext.ContactInfos.Add(defaultContactInfo);
            }
        }
        private static void SeedFeatures()
        {
            var features = _appDbContext.Features.ToList();
            if (features == null || features.Count == 0)
            {
                var defaultFeatures = new List <DbModels.CMSSchema.Feature>()
                {
                    new DbModels.CMSSchema.Feature
                    {
                        Title = "Planning and Preparation",
                        Description = " Focuses on data collection efforts to populate a standardized template",
                        LogoPath = "\\SeedingResources\\Features\\data-analytics.svg",
                        CreatedBy = null,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    },
                    new DbModels.CMSSchema.Feature
                    {
                        Title = "Reliability and Reduction of time",
                        Description = "Reduces forecasting analysis time to minutes and improves the reliability of the forecast",
                        LogoPath = "\\SeedingResources\\Features\\consult.svg",
                        CreatedBy = null,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    },
                    new DbModels.CMSSchema.Feature
                    {
                        Title = "Demand and Supply Planning",
                        Description = "Promotes development of evidence -based supply plans",
                        LogoPath = "\\SeedingResources\\Features\\data-analytics.svg",
                        CreatedBy = null,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    },
                    new DbModels.CMSSchema.Feature
                    {
                        Title = "Resource analysis",
                        Description = "Provides the opportunity to identify inefficiencies within a laboratory network",
                        LogoPath = "\\SeedingResources\\Features\\internet-things.svg",
                        CreatedBy = null,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    },
                    new DbModels.CMSSchema.Feature
                    {
                        Title = "Validates Consumption",
                        Description = "Consumption with service rates",
                        LogoPath = "\\SeedingResources\\Features\\machine-learning.svg",
                        CreatedBy = null,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    }
                };

                _appDbContext.Features.AddRange(defaultFeatures);
            }
        }
        private static void SeedApplicationRoles()
        {
            var items = _appDbContext.Roles.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ApplicationRolesEnum));
                ApplicationRolesEnum[] values = (ApplicationRolesEnum[])Enum.GetValues(typeof(ApplicationRolesEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.Roles.Add(new ApplicationRole() { Id = (int)values[i], Name = names[i], NormalizedName = names[i].ToUpper() });
                }
                _appDbContext.SaveChanges();
            }

        }
        private static void SeedUserSubscriptionLevels()
        {
            var items = _appDbContext.UserSubscriptionLevels.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(UserSubscriptionLevelsEnum));
                UserSubscriptionLevelsEnum[] values = (UserSubscriptionLevelsEnum[])Enum.GetValues(typeof(UserSubscriptionLevelsEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.UserSubscriptionLevels.Add(new UserSubscriptionLevel() { Name = names[i], Id = (int)values[i] });
                }
                _appDbContext.SaveChanges();
            }
        }
        private static void SeedApplicationSuperAdmin()
        {
            var superAdmin = _userManager.FindByNameAsync("admin@gmail.com");
            if (superAdmin.Result == null)
            {
                var applicationUser = new ApplicationUser()
                {
                    EmailConfirmed = true,
                    Status = UserStatusEnum.Active.ToString(),
                    FirstName = "Admin",
                    LastName = "User",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    LockoutEnabled = false,
                    CreatedBy = null,
                    CreatedOn = DateTime.Now,
                    NextPasswordExpiryDate = DateTime.Now.AddDays(30),
                    UserSubscriptionLevelId = (int)UserSubscriptionLevelsEnum.CountryLevel,
                };

                var result = _userManager.CreateAsync(applicationUser, "Admin@2010");
                if (result.Result.Succeeded)
                {
                    superAdmin = _userManager.FindByEmailAsync("admin@gmail.com");
                    _appDbContext.UserRoles.Add(new ApplicationUserRole { RoleId = (int)ApplicationRolesEnum.SuperAdmin, UserId = superAdmin.Result.Id });
                }
            }

        }
        private static void SeedUserTransactionTypes()
        {
            var items = _appDbContext.UserTransactionTypes.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(UserTransactionTypesEnum));
                UserTransactionTypesEnum[] values = (UserTransactionTypesEnum[])Enum.GetValues(typeof(UserTransactionTypesEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.UserTransactionTypes.Add(new UserTransactionType() { Name = names[i], Id = (int)values[i] });
                }
            }
        }

        #region Static Lookups
        private static void SeedCalculationPeriods()
        {
            var items = _appDbContext.CalculationPeriods.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(CalculationPeriodEnum));
                CalculationPeriodEnum[] values = (CalculationPeriodEnum[])Enum.GetValues(typeof(CalculationPeriodEnum));

                // Generate CalculationPeriodMonths
                var oneYearPeriodMonths = Enumerable.Range(1, 12).ToList().ConvertAll(x =>
                {
                    return new CalculationPeriodMonth
                    {
                        Id = x,
                        Name = $"M{x}",
                    };
                });
                var twoYearPeriodMonths = Enumerable.Range(1, 24).ToList().ConvertAll(x =>
                {
                    return new CalculationPeriodMonth
                    {
                        Id = 12 + x,
                        Name = $"M{x}",
                    };
                });

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.CalculationPeriods.Add(new CalculationPeriod() 
                    { 
                        Name = names[i], 
                        Id = (int)values[i],
                        CalculationPeriodMonths = names[i] == "OneYear" ? oneYearPeriodMonths : twoYearPeriodMonths
                    });
                }
            }
        }
        private static void SeedContinents()
        {
            var items = _appDbContext.Continents.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ContinentEnum));
                ContinentEnum[] values = (ContinentEnum[])Enum.GetValues(typeof(ContinentEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.Continents.Add(new Continent() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedControlRequirementUnits()
        {
            var items = _appDbContext.ControlRequirementUnits.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ControlRequirementUnitEnum));
                ControlRequirementUnitEnum[] values = (ControlRequirementUnitEnum[])Enum.GetValues(typeof(ControlRequirementUnitEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ControlRequirementUnits.Add(new ControlRequirementUnit() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedCountryPeriods()
        {
            var items = _appDbContext.CountryPeriods.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(CountryPeriodEnum));
                CountryPeriodEnum[] values = (CountryPeriodEnum[])Enum.GetValues(typeof(CountryPeriodEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.CountryPeriods.Add(new CountryPeriod() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedEntityTypes()
        {
            var items = _appDbContext.EntityTypes.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(EntityTypeEnum));
                EntityTypeEnum[] values = (EntityTypeEnum[])Enum.GetValues(typeof(EntityTypeEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.EntityTypes.Add(new EntityType() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedForecastInfoLevels()
        {
            var items = _appDbContext.ForecastInfoLevels.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ForecastInfoLevelEnum));
                ForecastInfoLevelEnum[] values = (ForecastInfoLevelEnum[])Enum.GetValues(typeof(ForecastInfoLevelEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ForecastInfoLevels.Add(new ForecastInfoLevel() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedForecastMethodologies()
        {
            var items = _appDbContext.ForecastMethodologies.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ForecastMethodologyEnum));
                ForecastMethodologyEnum[] values = (ForecastMethodologyEnum[])Enum.GetValues(typeof(ForecastMethodologyEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ForecastMethodologies.Add(new ForecastMethodology() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedProductBasicUnits()
        {
            var items = _appDbContext.ProductBasicUnits.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ProductBasicUnitEnum));
                ProductBasicUnitEnum[] values = (ProductBasicUnitEnum[])Enum.GetValues(typeof(ProductBasicUnitEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ProductBasicUnits.Add(new ProductBasicUnit() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedProductTypes()
        {
            var items = _appDbContext.ProductTypes.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ProductTypeEnum));
                ProductTypeEnum[] values = (ProductTypeEnum[])Enum.GetValues(typeof(ProductTypeEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ProductTypes.Add(new ProductType() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedReagentSystems()
        {
            var items = _appDbContext.ReagentSystems.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ReagentSystemEnum));
                ReagentSystemEnum[] values = (ReagentSystemEnum[])Enum.GetValues(typeof(ReagentSystemEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ReagentSystems.Add(new ReagentSystem() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedScopeOfTheForecasts()
        {
            var items = _appDbContext.ScopeOfTheForecasts.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ScopeOfTheForecastEnum));
                ScopeOfTheForecastEnum[] values = (ScopeOfTheForecastEnum[])Enum.GetValues(typeof(ScopeOfTheForecastEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ScopeOfTheForecasts.Add(new ScopeOfTheForecast() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedThroughPutUnits()
        {
            var items = _appDbContext.ThroughPutUnits.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(ThroughPutUnitEnum));
                ThroughPutUnitEnum[] values = (ThroughPutUnitEnum[])Enum.GetValues(typeof(ThroughPutUnitEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.ThroughPutUnits.Add(new ThroughPutUnit() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        private static void SeedVariableTypes()
        {
            var items = _appDbContext.VariableTypes.ToList();
            if (items == null || items.Count == 0)
            {
                string[] names = Enum.GetNames(typeof(VariableTypeEnum));
                VariableTypeEnum[] values = (VariableTypeEnum[])Enum.GetValues(typeof(VariableTypeEnum));

                for (int i = 0; i < names.Length; i++)
                {
                    _appDbContext.VariableTypes.Add(new VariableType() { Name = names[i], Id = (int)values[i] });
                }
            }
        }
        #endregion

        #region Country Info
        private static void SeedCountries()
        {
            List<Country> allcountry = _appDbContext.Countries.ToList();

            if (allcountry == null || allcountry.Count() == 0)
            {
                // Get all regions
                var regions = SeedRegionCountries();

                HttpClient http = new HttpClient();
                var data = http.GetAsync("https://restcountries.eu/rest/v2/all").Result.Content.ReadAsStringAsync().Result;
                var model = JsonConvert.DeserializeObject<List<CountryInfoApi>>(data);
                foreach (var country in model)
                {
                    int continentId = GetContinentId(country.Region, country.Subregion);
                    if (continentId <= 0)
                    {
                        continue;
                    }
                    _appDbContext.Countries.Add(new Country()
                    {
                        ContinentId = continentId,
                        CountryPeriodId = (int)CountryPeriodEnum.Monthly,
                        IsDeleted = false,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        Name = country.Name,
                        ShortCode = country.Alpha3Code,
                        ShortName = country.Alpha3Code,
                        Flag = country.Flag,
                        NativeName = country.NativeName,
                        Population = country.Population,
                        Latitude = country.Latlng.Count() > 0 ? country.Latlng[0].ToString() : null,
                        Longitude = country.Latlng.Count() > 0 ? country.Latlng[1].ToString() : null,
                        CurrencyCode = country.Currencies.Count() > 0 ? country.Currencies[0].Code : null,
                        CallingCode = country.CallingCodes.Count() > 0 && !string.IsNullOrEmpty(country.CallingCodes[0]) ? $"+{country.CallingCodes[0]}" : null,
                        Regions = regions.Where(x => x.Key == country.Name).SelectMany(x => x.Value).ToList().ConvertAll(x =>
                        {
                            return new Region
                            {
                                Name = x,
                                ShortName = x,
                                IsDeleted = false,
                                IsActive = true,
                                CreatedOn = DateTime.Now,
                            };
                        })
                    });
                }

            }

        }
        private static Dictionary<string, List<string>> SeedRegionCountries()
        {
            HttpClient http = new HttpClient();
            var data = http.GetAsync("https://raw.githubusercontent.com/russ666/all-countries-and-cities-json/6ee538beca8914133259b401ba47a550313e8984/countries.json").Result.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(data);
            return model;
        }
        // Helper Functions
        private static int GetContinentId(string region, string subregion)
        {
            if (region == ContinentEnum.Asia.ToString())
            {
                return (int)ContinentEnum.Asia;
            }
            else if (region == ContinentEnum.Africa.ToString())
            {
                return (int)ContinentEnum.Africa;
            }
            else if (region == "Americas" && subregion == "Northern America")
            {
                return (int)ContinentEnum.NorthAmerica;
            }
            else if (region == "Americas" && subregion == "South America")
            {
                return (int)ContinentEnum.SouthAmerica;
            }
            else if (region == "Polar")
            {
                return (int)ContinentEnum.Antarctica;
            }
            else if (region == ContinentEnum.Europe.ToString())
            {
                return (int)ContinentEnum.Europe;
            }
            else if (region == "Oceania" && subregion.Trim().ToLower().Contains(ContinentEnum.Australia.ToString().ToLower()))
            {
                return (int)ContinentEnum.Australia;
            }
            return 0;
        }
        #endregion

        private static void SeedSuperAdminSubscriptions()
        {
            var items = _appDbContext.UserCountrySubscriptions.ToList();
            if (items == null || items.Count == 0)
            {
                var superAdmin = _userManager.FindByNameAsync("admin@gmail.com");
                if (superAdmin.Result != null)
                {
                    var countriesIds = _appDbContext.Countries.Where(x => !x.IsDeleted).Select(x => x.Id).ToList();
                    var userCountrySubscriptions = countriesIds.ConvertAll(countryId =>
                    {
                        return new UserCountrySubscription
                        {
                            CountryId = countryId,
                            ApplicationUserId = superAdmin.Result.Id,
                            CreatedBy = superAdmin.Result.Id,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                        };
                    });
                    _appDbContext.UserCountrySubscriptions.AddRange(userCountrySubscriptions);
                    // save to the database
                    _appDbContext.SaveChanges();
                }
            }
        }
    }
}
