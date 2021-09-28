using ForLab.DTO.Common;

namespace ForLab.DTO.Lookup.Laboratory
{
   public class LaboratoryDto:DynamicLookupDto
    {
        public int RegionId { get; set; }
        public int LaboratoryCategoryId { get; set; }
        public int LaboratoryLevelId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool Shared { get; set; }

        //UI
        public int? RegionCountryId { get; set; }
        public string RegionName { get; set; }
        public string LaboratoryCategoryName { get; set; }
        public string LaboratoryLevelName { get; set; }
    }
    public class LaboratoryDrp : DropdownDrp
    {
        public int RegionId { get; set; }
    }
}
