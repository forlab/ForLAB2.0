using ForLab.DTO.Common;

namespace ForLab.DTO.CMS.UsefulResource
{
    public class UsefulResourceDto : BaseEntityDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AttachmentUrl { get; set; }
        public decimal? AttachmentSize { get; set; }
        public string AttachmentName { get; set; }
        public string ExtensionFormat { get; set; }
        public bool IsExternalResource { get; set; } // false if the user broese and upload the resource itself
        public int DownloadCount { get; set; }
    }
}
