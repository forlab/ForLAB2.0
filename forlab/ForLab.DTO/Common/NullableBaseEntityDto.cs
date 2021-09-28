using ForLab.Core.Interfaces;
using System;

namespace ForLab.DTO.Common
{
    public class NullableBaseEntityDto: IBaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }


        // UI
        public string Creator { get; set; }
        public string Updator { get; set; }
    }
}
