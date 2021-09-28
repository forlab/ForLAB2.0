using ForLab.DTO.Common;
using ForLab.DTO.Forecasting.ForecastCategory;
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
using ForLab.DTO.Forecasting.ForecastTest;
using ForLab.DTO.Forecasting.ForecastTestingAssumptionValue;
using System;
using System.Collections.Generic;

namespace ForLab.DTO.Forecasting.ForecastInfo
{
    public class ForecastInfoDto : BaseEntityDto
    {
        public int ForecastInfoLevelId { get; set; }
        public int CountryId { get; set; }
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

        public List<ForecastInstrumentDto> ForecastInstrumentDtos { get; set; } // mandatory
        public List<ForecastPatientGroupDto> ForecastPatientGroupDtos { get; set; }
        public List<ForecastTestDto> ForecastTestDtos { get; set; } // mandatory
        public List<ForecastCategoryDto> ForecastCategoryDtos { get; set; }
        public List<ForecastLaboratoryConsumptionDto> ForecastLaboratoryConsumptionDtos { get; set; }
        public List<ForecastLaboratoryTestServiceDto> ForecastLaboratoryTestServiceDtos { get; set; }
        public List<ForecastLaboratoryDto> ForecastLaboratoryDtos { get; set; }
        public List<ForecastMorbidityWhoBaseDto> ForecastMorbidityWhoBaseDtos { get; set; }
        public List<ForecastMorbidityTargetBaseDto> ForecastMorbidityTargetBaseDtos { get; set; }
        public List<ForecastPatientAssumptionValueDto> ForecastPatientAssumptionValueDtos { get; set; }
        public List<ForecastProductAssumptionValueDto> ForecastProductAssumptionValueDtos { get; set; }
        public List<ForecastTestingAssumptionValueDto> ForecastTestingAssumptionValueDtos { get; set; }
        public List<ForecastMorbidityTestingProtocolMonthDto> ForecastMorbidityTestingProtocolMonthDtos { get; set; }
        public List<ForecastMorbidityProgramDto> ForecastMorbidityProgramDtos { get; set; }

        // UI
        public string ForecastInfoLevelName { get; set; }
        public string CountryName { get; set; }
        public string ForecastMethodologyName { get; set; }
        public string ScopeOfTheForecastName { get; set; }
        public List<HistoicalConsumptionDto> HistoicalConsumptionDtos { get; set; }
        public List<HistoicalServiceDataDto> HistoicalServiceDataDtos { get; set; }
        public List<HistoicalTargetBaseDto> HistoicalTargetBaseDtos { get; set; }
        public List<HistoicalWhoBaseDto> HistoicalWhoBaseDtos { get; set; }
    }
    public class ForecastInfoDrp : DropdownDrp
    {
    }
}
