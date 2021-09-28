using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.ArticleImage
{
    public class ArticleImageDto : BaseEntityDto
    {
        public int ArticleId { get; set; }
        public string AttachmentUrl { get; set; }
        public float? AttachmentSize { get; set; }
        public string AttachmentName { get; set; }
        public string ExtensionFormat { get; set; }
        public bool IsDefault { get; set; }
        public bool IsExternalResource { get; set; } // false if the user broese and upload the resource itself

        //UI
        public string ArticleName { get; set; }
    }
}
