using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.CMS.Article
{
    public class ArticleFilterDto : BaseFilterDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ProvidedBy { get; set; }
        public DateTime? ProvidedDate { get; set; }
    }
}
