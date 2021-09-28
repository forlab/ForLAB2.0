using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("Features", Schema = "CMS")]
    public class Feature : NullableBaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string LogoPath { get; set; }
    }
}
