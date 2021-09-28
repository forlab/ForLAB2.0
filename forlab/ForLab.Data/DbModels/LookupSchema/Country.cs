using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseSchema;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.SecuritySchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("Countries", Schema = "Lookup")]
    public class Country: DynamicLookup
    {
        public Country()
        {
            Regions = new HashSet<Region>();
            CountryDiseaseIncidents = new HashSet<CountryDiseaseIncident>();
            CountryProductPrices = new HashSet<CountryProductPrice>();
            UserCountrySubscribtions = new HashSet<UserCountrySubscription>();
            ForecastMorbidityWhoBases = new HashSet<ForecastMorbidityWhoBase>();
        }
        public int ContinentId { get; set; }
        public int CountryPeriodId { get; set; }
        public string ShortCode { get; set; }
        public string ShortName { get; set; }
        public string NativeName { get; set; }
        public string Flag { get; set; }
        public string CurrencyCode { get; set; }
        public string CallingCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public decimal Population { get; set; }


        public virtual Continent Continent { get; set; }
        public virtual CountryPeriod CountryPeriod { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
        public virtual ICollection<CountryDiseaseIncident> CountryDiseaseIncidents { get; set; }
        public virtual ICollection<CountryProductPrice> CountryProductPrices { get; set; }
        public virtual ICollection<UserCountrySubscription> UserCountrySubscribtions { get; set; }
        public virtual ICollection<ForecastMorbidityWhoBase> ForecastMorbidityWhoBases { get; set; }
    }
}
