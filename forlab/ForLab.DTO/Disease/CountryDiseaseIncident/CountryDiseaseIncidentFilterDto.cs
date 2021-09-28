using ForLab.DTO.Common;

namespace ForLab.DTO.Disease.CountryDiseaseIncident
{
    public class CountryDiseaseIncidentFilterDto : BaseFilterDto
    {
        public int CountryId { get; set; }
        public int DiseaseId { get; set; }
        public int Year { get; set; }
        public decimal? Incidence { get; set; }
        public decimal? IncidencePer1kPopulation { get; set; }
        public decimal? IncidencePer100kPopulation { get; set; }
        public decimal? PrevalenceRate { get; set; }
        public decimal? PrevalenceRatePer1kPopulation { get; set; }
        public decimal? PrevalenceRatePer100kPopulation { get; set; }
    }
}
