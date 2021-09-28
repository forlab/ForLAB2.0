using ForLab.Data.BaseModeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("UsefulResources", Schema = "CMS")]
    public class UsefulResource : BaseEntity
    {
        public string Title { get; set; }
        public string AttachmentUrl { get; set; }
        public float? AttachmentSize { get; set; }
        public string AttachmentName { get; set; }
        public string ExtensionFormat { get; set; }
        public bool IsExternalResource { get; set; } // false if the user broese and upload the resource itself
        public int DownloadCount { get; set; }
    }
}
