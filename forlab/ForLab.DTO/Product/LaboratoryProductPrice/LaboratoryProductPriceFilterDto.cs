using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Product.LaboratoryProductPrice
{
    public class LaboratoryProductPriceFilterDto : BaseFilterDto
    {
        public int ProductId { get; set; }
        public int LaboratoryId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PackSize { get; set; }
        public DateTime? FromDate { get; set; }
    }
}
