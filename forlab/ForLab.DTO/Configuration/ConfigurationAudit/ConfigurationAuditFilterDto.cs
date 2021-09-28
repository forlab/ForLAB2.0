using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Configuration.ConfigurationAudit 
{
    public class ConfigurationAuditFilterDto : BaseFilterDto
    {
        public DateTime? DateOfAction { get; set; }
        public string Action { get; set; }
        public int CreatedBy { get; set; }
    }
}
