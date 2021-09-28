using ForLab.DTO.Common;
using System;

namespace ForLab.DTO.Product.RegionProductPrice
{
    public class RegionProductPriceFilterDto : BaseFilterDto
    {
        public int ProductId { get; set; }
        public int RegionId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PackSize { get; set; }
        public DateTime? FromDate { get; set; }
    }
}
