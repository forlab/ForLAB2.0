using ForLab.DTO.Common;
using System;


namespace ForLab.DTO.Product.CountryProductPrice
{
    public class CountryProductPriceFilterDto : BaseFilterDto
    {
        public int ProductId { get; set; }
        public int CountryId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PackSize { get; set; }
        public DateTime? FromDate { get; set; }
    }
}
