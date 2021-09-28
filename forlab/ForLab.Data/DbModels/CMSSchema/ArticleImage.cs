using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("ArticleImages", Schema = "CMS")]
    public class ArticleImage : BaseEntity
    {
        public int ArticleId { get; set; }
        public string AttachmentUrl { get; set; }
        public float? AttachmentSize { get; set; }
        public string AttachmentName { get; set; }
        public string ExtensionFormat { get; set; }
        public bool IsDefault { get; set; }
        public bool IsExternalResource { get; set; } // false if the user browse and upload the resource itself
        public virtual Article Article { get; set; }
    }
}
