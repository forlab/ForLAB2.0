using ForLab.DTO.CMS.ArticleImage;
using ForLab.DTO.Common;
using System;
using System.Collections.Generic;

namespace ForLab.DTO.CMS.Article
{
    public class ArticleDto : BaseEntityDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ProvidedBy { get; set; }
        public DateTime ProvidedDate { get; set; }

        public List<ArticleImageDto> ArticleImageDtos { get; set; }
    }
}
