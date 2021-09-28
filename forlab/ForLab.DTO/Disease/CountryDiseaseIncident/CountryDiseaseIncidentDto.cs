using ForLab.DTO.Common;

namespace ForLab.DTO.Disease.CountryDiseaseIncident
{
    public class CountryDiseaseIncidentDto : BaseEntityDto
    {
        public int CountryId { get; set; }
        public int DiseaseId { get; set; }
        public int Year { get; set; }
        public decimal Incidence { get; set; }
        public decimal IncidencePer1kPopulation { get; set; }
        public decimal IncidencePer100kPopulation { get; set; }
        public decimal PrevalenceRate { get; set; }
        public decimal PrevalenceRatePer1kPopulation { get; set; }
        public decimal PrevalenceRatePer100kPopulation { get; set; }
        public string Note { get; set; }


        //UI
        public string CountryName { get; set; }
        public string DiseaseName { get; set; }
    }

    public class CountryDiseaseIncidentDrp : DropdownDrp
    {
        public string CountryName { get; set; }
        public string DiseaseName { get; set; }
        public int Year { get; set; }
    }

}
