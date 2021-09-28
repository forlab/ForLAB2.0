using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.DiseaseSchema
{
    [Table("CountryDiseaseIncidents", Schema = "Disease")]
    public class CountryDiseaseIncident : BaseEntity
    {
        // this object will have no create screen , it will be fetched from global api
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
        public virtual Country Country { get; set; }
        public virtual Disease Disease { get; set; }
    }
}
