namespace ForLab.DTO.Disease.CountryDiseaseIncident
{
    public class ExportCountryDiseaseIncidentDto
    {
        public string CountryName { get; set; }
        public string DiseaseName { get; set; }
        public int Year { get; set; }
        public decimal Incidence { get; set; }
        public decimal IncidencePer1kPopulation { get; set; }
        public decimal IncidencePer100kPopulation { get; set; }
        public decimal PrevalenceRate { get; set; }
        public decimal PrevalenceRatePer1kPopulation { get; set; }
        public decimal PrevalenceRatePer100kPopulation { get; set; }
    }
}
