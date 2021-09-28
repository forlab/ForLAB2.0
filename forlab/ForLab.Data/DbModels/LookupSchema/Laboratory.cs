using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.LaboratorySchema;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.SecuritySchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    [Table("Laboratories", Schema = "Lookup")]
    public class Laboratory : DynamicLookup
    {
        public Laboratory()
        {
            LaboratoryWorkingDays = new HashSet<LaboratoryWorkingDay>();
            LaboratoryInstruments = new HashSet<LaboratoryInstrument>();
            ForecastInfos = new HashSet<ForecastInfo>();
            LaboratoryPatientStatistics = new HashSet<LaboratoryPatientStatistic>();
            LaboratoryTestServices = new HashSet<LaboratoryTestService>();
            LaboratoryConsumptions = new HashSet<LaboratoryConsumption>();
            LaboratoryProductPrices = new HashSet<LaboratoryProductPrice>();
            ForecastLaboratories = new HashSet<ForecastLaboratory>();
            UserLaboratorySubscriptions = new HashSet<UserLaboratorySubscription>();
            ForecastResults = new HashSet<ForecastResult>();
        }
        public int RegionId { get; set; }
        public int LaboratoryCategoryId { get; set; }
        public int LaboratoryLevelId { get; set; } 
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool Shared { get; set; }

        public virtual Region Region { get; set; }
        public virtual LaboratoryCategory LaboratoryCategory { get; set; }
        public virtual LaboratoryLevel LaboratoryLevel { get; set; }
        public virtual ICollection<LaboratoryWorkingDay> LaboratoryWorkingDays { get; set; }
        public virtual ICollection<LaboratoryInstrument> LaboratoryInstruments { get; set; }
        public virtual ICollection<ForecastInfo> ForecastInfos { get; set; }
        public virtual ICollection<LaboratoryPatientStatistic> LaboratoryPatientStatistics { get; set; }
        public virtual ICollection<LaboratoryTestService> LaboratoryTestServices { get; set; }
        public virtual ICollection<LaboratoryConsumption> LaboratoryConsumptions { get; set; }
        public virtual ICollection<LaboratoryProductPrice> LaboratoryProductPrices { get; set; }
        public virtual ICollection<ForecastLaboratory> ForecastLaboratories { get; set; }
        public virtual ICollection<UserLaboratorySubscription> UserLaboratorySubscriptions { get; set; }
        public virtual ICollection<ForecastResult> ForecastResults { get; set; }
    }
}
