using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastInfos", Schema = "Forecasting")]
    public class ForecastInfo : BaseEntity
    {
        public ForecastInfo()
        {
            ForecastInstruments = new HashSet<ForecastInstrument>();
            ForecastPatientGroups = new HashSet<ForecastPatientGroup>();
            ForecastTests = new HashSet<ForecastTest>();
            ForecastCategories = new HashSet<ForecastCategory>();
            ForecastLaboratoryConsumptions = new HashSet<ForecastLaboratoryConsumption>();
            ForecastLaboratoryTestServices = new HashSet<ForecastLaboratoryTestService>();
            ForecastLaboratories = new HashSet<ForecastLaboratory>();
            ForecastMorbidityWhoBases = new HashSet<ForecastMorbidityWhoBase>();
            ForecastPatientAssumptionValues = new HashSet<ForecastPatientAssumptionValue>();
            ForecastProductAssumptionValues = new HashSet<ForecastProductAssumptionValue>();
            ForecastTestingAssumptionValues = new HashSet<ForecastTestingAssumptionValue>();
            ForecastMorbidityPrograms = new HashSet<ForecastMorbidityProgram>();
            ForecastMorbidityTestingProtocolMonths = new HashSet<ForecastMorbidityTestingProtocolMonth>();
            ForecastMorbidityTargetBases = new HashSet<ForecastMorbidityTargetBase>();
            ForecastResults = new HashSet<ForecastResult>();
        }

        public int ForecastInfoLevelId { get; set; }
        public int CountryId { get; set; } // this will be country, region, laboratory id
        public int ForecastMethodologyId { get; set; }
        public int ScopeOfTheForecastId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }// date format is MM/YYYY
        public int Duration { get; set; } // duration should be based on country level (daily,weekly, monthly, quarterly) and then translate the count to get end date
        public bool IsAggregate { get; set; } // if selected then forecast category should be used to add categories for this forecast, then each category should be assigned to one or more lab
        public bool IsSiteBySite { get; set; }
        public bool IsWorldHealthOrganization { get; set; } // if morbidity
        public bool IsTargetBased { get; set; } // if morbidity
        public decimal WastageRate { get; set; }

        public virtual ForecastInfoLevel ForecastInfoLevel { get; set; }
        public virtual Country Country { get; set; }
        public virtual ForecastMethodology ForecastMethodology { get; set; }
        public virtual ScopeOfTheForecast ScopeOfTheForecast { get; set; }
        public virtual ICollection<ForecastInstrument> ForecastInstruments { get; set; } // mandatory
        public virtual ICollection<ForecastPatientGroup> ForecastPatientGroups { get; set; }
        public virtual ICollection<ForecastTest> ForecastTests { get; set; } // mandatory
        public virtual ICollection<ForecastCategory> ForecastCategories { get; set; }
        public virtual ICollection<ForecastLaboratoryConsumption> ForecastLaboratoryConsumptions { get; set; }
        public virtual ICollection<ForecastLaboratoryTestService> ForecastLaboratoryTestServices { get; set; }
        public virtual ICollection<ForecastLaboratory> ForecastLaboratories { get; set; }
        public virtual ICollection<ForecastMorbidityWhoBase> ForecastMorbidityWhoBases { get; set; }
        public virtual ICollection<ForecastPatientAssumptionValue> ForecastPatientAssumptionValues { get; set; }
        public virtual ICollection<ForecastProductAssumptionValue> ForecastProductAssumptionValues { get; set; }
        public virtual ICollection<ForecastTestingAssumptionValue> ForecastTestingAssumptionValues { get; set; }
        public virtual ICollection<ForecastMorbidityProgram> ForecastMorbidityPrograms { get; set; }
        public virtual ICollection<ForecastMorbidityTargetBase> ForecastMorbidityTargetBases { get; set; }
        public virtual ICollection<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
        public virtual ICollection<ForecastResult> ForecastResults { get; set; }
    }
}
