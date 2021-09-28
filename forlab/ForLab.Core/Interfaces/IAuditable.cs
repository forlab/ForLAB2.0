using System;
namespace ForLab.Core.Interfaces
{
    public interface IAuditable
    {
        int CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        int? UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}
