using System;
using System.Collections.Generic;

namespace ForLab.DTO.Forecasting.ForecastInfo
{
    public class HistoicalConsumptionDto
    {
        public int RegionId { get; set; }
        public int LaboratoryId { get; set; }
        public int ProductId { get; set; }

        public string ForecastCategoryName { get; set; }
        public string RegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string ProductName { get; set; }
    }

    public class HistoicalServiceDataDto
    {
        public int RegionId { get; set; }
        public int LaboratoryId { get; set; }
        public int TestId { get; set; }

        public string ForecastCategoryName { get; set; }
        public string RegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string TestName { get; set; }
    }

    public class HistoicalTargetBaseDto
    {
        public int RegionId { get; set; }
        public int LaboratoryId { get; set; }
        public int ProgramId { get; set; }
        public decimal CurrentPatient { get; set; }
        public decimal TargetPatient { get; set; }

        public string ForecastCategoryName { get; set; }
        public string RegionName { get; set; }
        public string LaboratoryName { get; set; }
        public string ProgramName { get; set; }
    }

    public class HistoicalWhoBaseDto
    {
        public int CountryId { get; set; }
        public int DiseaseId { get; set; }

        public string CountryName { get; set; }
        public string DiseaseName { get; set; }
    }

    #region ML Body & Response
    public class MLBodyDto
    {
        public int region { get; set; }
        public int site { get; set; }
        public int product { get; set; } // Consumtion
        public int test { get; set; } // Service data
        public int country { get; set; } // Who
        public int disease { get; set; } // Who
        public string from { get; set; }
        public string to { get; set; }
        public List<MLBodyHistoryDataDto> historyData { get; set; }
    }
    public class MLBodyHistoryDataDto
    {
        public string timeStamps { get; set; }
        public int history { get; set; }
    }
    public class MLResponseDto
    {
        public int Region { get; set; }
        public int Site { get; set; }
        public int Product { get; set; }
        public int Test { get; set; }
        public int Country { get; set; } // Who
        public int Disease { get; set; } // Who
        public string From { get; set; }
        public string To { get; set; }
        public List<MLResponsePredictionDataDto> PredictionData { get; set; }
    }
    public class MLResponsePredictionDataDto
    {
        public string TimeStamps { get; set; }
        public decimal Forecast { get; set; }
    }
    #endregion

    #region Morbidity data format

    public class TestNumCycleForecast
    {
        public DateTime TimeStamp { get; set; }
        public decimal Forecast { get; set; }
    }

    public class TestNumCycleForecastByGroup
    {
        public int GroupId { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Forecast { get; set; }
    }
    #endregion
}
