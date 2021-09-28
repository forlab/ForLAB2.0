using System;

namespace ForLab.DTO.Product.LaboratoryProductPrice
{
    public class ExportLaboratoryProductPriceDto
    {
        public string ProductName { get; set; }
        public string LaboratoryName { get; set; }
        public decimal Price { get; set; }
        public decimal PackSize { get; set; }
        public DateTime FromDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
