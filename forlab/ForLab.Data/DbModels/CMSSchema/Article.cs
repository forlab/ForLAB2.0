using ForLab.Data.BaseModeling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.CMSSchema
{
    [Table("Articles", Schema = "CMS")]
    public class Article : BaseEntity
    {
        public Article()
        {
            ArticleImages = new HashSet<ArticleImage>();
        }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ProvidedBy { get; set; }
        public DateTime ProvidedDate { get; set; }

        public virtual ICollection<ArticleImage> ArticleImages { get; set; }
    }
}
