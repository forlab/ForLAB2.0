using System;

namespace ForLab.DTO.Lookup.Laboratory
{
   public class ExportLaboratoryDto
    {
        public string Name { get; set; }
        public string RegionName { get; set; }
        public string LaboratoryCategoryName { get; set; }
        public string LaboratoryLevelName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
