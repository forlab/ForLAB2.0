using ForLab.Core.Interfaces;
using ForLab.Data.DbModels.SecuritySchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.BaseModeling
{
    public class DynamicLookup : ILookupEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("Creator")]
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("Updator")]
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual ApplicationUser Creator { get; set; }
        public virtual ApplicationUser Updator { get; set; }
    }
}
