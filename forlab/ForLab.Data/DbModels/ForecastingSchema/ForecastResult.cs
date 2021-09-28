using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.ProductSchema;
using ForLab.Data.DbModels.TestingSchema;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    [Table("ForecastResults", Schema = "Forecasting")]
    public class ForecastResult : BaseEntity
    {
        public int ForecastInfoId { get; set; }
        public int LaboratoryId { get; set; }
        public int? TestId { get; set; }
        public int ProductId { get; set; }
        public decimal AmountForecasted { get; set; } // which will be fetched from the machine learning model
        public decimal TotalValue { get; set; }
        public DateTime DurationDateTime { get; set; }
        public string Period { get; set; }
        public decimal PackSize { get; set; }
        public int PackQty { get; set; }
        public decimal PackPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProductTypeId { get; set; }

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual Test Test { get; set; }
        public virtual Product Product { get; set; }
        public virtual Laboratory Laboratory { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}
