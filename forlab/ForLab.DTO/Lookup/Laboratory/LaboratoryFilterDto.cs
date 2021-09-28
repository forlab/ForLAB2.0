using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.Laboratory
{
   public class LaboratoryFilterDto:BaseFilterDto
    {
        public string Name { get; set; }
        public int RegionId { get; set; }
        public int LaboratoryCategoryId { get; set; }
        public int LaboratoryLevelId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
